using System.IO;
using UnityEngine;


public class JsonLoadStructure
{
    public Vector3 position;
    public Vector3 rotation;
}

public class PendantRotationAdjuster : MonoBehaviour
{
    [SerializeField]
    private Vector3 defaultOrientation;

    [SerializeField]
    private Vector3 adjustedOrientation;
    [SerializeField]
    private Vector3 defaultPosition;

    [SerializeField]
    private Vector3 adjustedPosition;

    private bool currentOrientation = true;

    [SerializeField]
    private GameObject target;

    private Vector3 loadedPosition;
    private Vector3 loadedRotation;

    private void Awake()
    {
        StreamReader inp_stm = new StreamReader("pendantAdjustment.json");
        string loadFile = "";
        while (!inp_stm.EndOfStream)
        {
            string inp_ln = inp_stm.ReadLine();
            loadFile += inp_ln;
        }

        inp_stm.Close();

        var result = JsonUtility.FromJson<JsonLoadStructure>(loadFile);
        loadedPosition = result.position;
        loadedRotation = result.rotation;
    }


    public void AdjustRotation()
    {
        currentOrientation = !currentOrientation;
        target.transform.localRotation = Quaternion.Euler(currentOrientation ? defaultOrientation : loadedRotation);
        target.transform.localPosition = currentOrientation ? defaultPosition : loadedPosition;
    }
}