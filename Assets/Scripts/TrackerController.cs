using System.Collections;
using System.Text;
using UnityEngine;
using Valve.VR;

public class TrackerController : Controller
{
    public int trackerBatteryLevelToCharge = default;
    [SerializeField] private int overchargeAmmount = default;
    [SerializeField] private int minimumPendantBetteryLevelRequired = default;
    public bool isTrackerCharging { private set; get; }
    public bool isTrackerConnected { private set; get; }
    private int trackerObjectId = -1;
    private int trackerBatteryLevel = default;
    private Coroutine chargingMessageCoroutine = default;

    private PendantUI pendantUI = default;
    private TcpReceiver tcpReceiver = default;

    private new void Awake()
    {
        pendantUI = FindObjectOfType<PendantUI>();
        tcpReceiver = FindObjectOfType<TcpReceiver>();

        onConnectionLost();
        // Debug.Log((pendantUI != null).ToString() + " " + (tcpReceiver != null).ToString());
    }

    public void PerformChargingDecision(int pendantBatteryLevel)
    {
        if (pendantBatteryLevel > minimumPendantBetteryLevelRequired
            && trackerBatteryLevel < trackerBatteryLevelToCharge
            && !isTrackerCharging)
        {
            startChargingTracker();
        }
        else if ((pendantBatteryLevel < minimumPendantBetteryLevelRequired
                  || trackerBatteryLevel > trackerBatteryLevelToCharge + overchargeAmmount)
                 && isTrackerCharging)
        {
            stopChargingTracker();
        }
    }

    private void startChargingTracker()
    {
        isTrackerCharging = true;
        pendantUI.ShowTrackerCharging(true);
        chargingMessageCoroutine = StartCoroutine(SendChargingMessage());
    }

    private void stopChargingTracker()
    {
        isTrackerCharging = false;
        pendantUI.ShowTrackerCharging(false);
        StopCoroutine(chargingMessageCoroutine);
    }

    private void attemptToEstablishTrackerConnection()
    {

        for (int i = 0; i < SteamVR.connected.Length; ++i)
        {
            if (SteamVR.connected[i])
            {
                ETrackedPropertyError error = new ETrackedPropertyError();
                StringBuilder sb = new StringBuilder();
                OpenVR.System.GetStringTrackedDeviceProperty((uint)i, ETrackedDeviceProperty.Prop_ControllerType_String, sb, OpenVR.k_unMaxPropertyStringSize, ref error);
                if (error == ETrackedPropertyError.TrackedProp_Success)
                {
                    if (sb.ToString().Contains("tracker"))
                    {
                        onConnectionFound(i);
                    }
                }
            }
        }

    }

    private void onConnectionFound(int id)
    {
        trackerObjectId = id;
        pendantUI.ShowTrackerConnectionError(false);
    }

    private bool checkTrackerConnectionStatus()
    {
        if (!SteamVR.connected[trackerObjectId])
        {
            onConnectionLost();
            return false;
        }
        return true;
    }

    private void onConnectionLost()
    {
        trackerObjectId = -1;
        trackerBatteryLevel = 100;
        pendantUI.ShowTrackerBatteryLevel(trackerBatteryLevel);
        pendantUI.ShowTrackerConnectionError(true);
    }

    private void updateTrackerBatteryLevel()
    {
        ETrackedPropertyError err = new ETrackedPropertyError();
        float f = OpenVR.System.GetFloatTrackedDeviceProperty((uint)trackerObjectId, ETrackedDeviceProperty.Prop_DeviceBatteryPercentage_Float, ref err);
        if (err == ETrackedPropertyError.TrackedProp_Success)
        {
            trackerBatteryLevel = (int)(f * 100);
            pendantUI.ShowTrackerBatteryLevel(trackerBatteryLevel);
        }
    }

    private IEnumerator SendChargingMessage()
    {
        while (true)
        {
            yield return new WaitForSeconds(2f);
            tcpReceiver.SendChargingMessage();
        }
    }

    private void FixedUpdate()
    {
        if (trackerObjectId == -1)
        {
            attemptToEstablishTrackerConnection();
        }
        else
        {
            var status = checkTrackerConnectionStatus();
            if (!status) return;
            updateTrackerBatteryLevel();
        }
    }
}