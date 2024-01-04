using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

[Serializable]
public class UnityHdmQuad
{
    public Vector3 c0;
    public Vector3 c1;
    public Vector3 c2;
    public Vector3 c3;

    public UnityHdmQuad()
    {
        c0 = Vector3.zero;
        c1 = Vector3.zero;
        c2 = Vector3.zero;
        c3 = Vector3.zero;
    }

    public UnityHdmQuad(HmdQuad_t original)
    {
        SetQuad(original);
    }

    public void SetQuad(HmdQuad_t original)
    {
        c0 = new Vector3(original.vCorners0.v0, original.vCorners0.v1, original.vCorners0.v2);
        c1 = new Vector3(original.vCorners1.v0, original.vCorners1.v1, original.vCorners1.v2);
        c2 = new Vector3(original.vCorners2.v0, original.vCorners2.v1, original.vCorners2.v2);
        c3 = new Vector3(original.vCorners3.v0, original.vCorners3.v1, original.vCorners3.v2);
    }
}

public class TronVisibilityMode : MonoBehaviour
{
    [SerializeField] private Transform trackedCamera;

    [SerializeField] private float outOfBoundsBuffer = default;

    private UnityHdmQuad playAreaRect = default;
    private bool isTronModeEnabled = false;

    private void Awake()
    {
        playAreaRect = new UnityHdmQuad();
        CachePlayAreaRect();
        // Valve.VR.OpenVR.Chaperone.ForceBoundsVisible(true);

    }


    void Update()
    {
        CachePlayAreaRect();

        var isCameraOob = isCameraOutOfBounds();

        if (!isTronModeEnabled && isCameraOob)
        {
            SetTroneMode(true);
        }
        else if (isTronModeEnabled && !isCameraOob)
        {
            SetTroneMode(false);
        }
    }

    private void SetTroneMode(bool _mode)
    {
        isTronModeEnabled = _mode;
        Valve.VR.OpenVR.Chaperone.ForceBoundsVisible(isTronModeEnabled);
        // var e = SwitchHDMCameraVisibility(isTronModeEnabled);
    }

    private bool isCameraOutOfBounds()
    {

        bool result = false;

        if (trackedCamera.localPosition.x > playAreaRect.c0.x + outOfBoundsBuffer)
        {
            result = true;
        }
        else if (trackedCamera.localPosition.x < playAreaRect.c2.x - outOfBoundsBuffer)
        {
            result = true;
        }

        if (trackedCamera.localPosition.z > playAreaRect.c3.z + outOfBoundsBuffer)
        {
            result = true;
        }
        else if (trackedCamera.localPosition.z < playAreaRect.c1.z - outOfBoundsBuffer)
        {
            result = true;
        }

        return result;
    }

    private void CachePlayAreaRect()
    {
        HmdQuad_t output = new HmdQuad_t();
        SteamVR_PlayArea.GetBounds(SteamVR_PlayArea.Size.Calibrated, ref output);
        playAreaRect.SetQuad(output);
    }

    EVRSettingsError SetHDMCameraVisibility(bool enable)
    {
        EVRSettingsError e = EVRSettingsError.None;
        OpenVR.Settings.SetBool(OpenVR.k_pch_Camera_Section,
                                OpenVR.k_pch_Camera_EnableCameraForCollisionBounds_Bool,
                                enable,
                                ref e);
        OpenVR.Settings.Sync(true, ref e);
        return e;
    }
}
