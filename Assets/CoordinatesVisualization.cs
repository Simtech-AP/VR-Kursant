using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CoordinatesVisualization : MonoBehaviour
{
    [System.Serializable]
    private class AxisVisualization
    {
        public string name = default;

        [SerializeField]
        private List<MeshRenderer> objectRenderers = default;

        [SerializeField]
        private TextMeshProUGUI UItext = default;

        [SerializeField]
        private Material activeMat = default;

        [SerializeField]
        private Color activeCol = default;

        [SerializeField]
        private GameObject rotationAid = default;


        public void SetAxisOff(Material mat, Color col)
        {
            objectRenderers.ForEach(x => x.material = mat);
            UItext.color = col;
            rotationAid.SetActive(false);
        }

        public void SetAxisOn(bool withRotAid = false)
        {
            objectRenderers.ForEach(x => x.material = activeMat);
            UItext.color = activeCol;
            rotationAid.SetActive(withRotAid);
        }
    }


    [SerializeField]
    private Material inactiveMat = default;


    [SerializeField]
    private Color inactiveCol = default;

    [SerializeField]
    private List<AxisVisualization> axes = default;


    public void EnableAxisVisual(string axisName)
    {
        var axis = axes.Find(x => x.name == axisName);

        axis.SetAxisOn();
    }

    public void EnableAxisVisualRotation(string axisName)
    {
        var axis = axes.Find(x => x.name == axisName);

        axis.SetAxisOn(true);
    }

    public void DisableAxisVisual(string axisName)
    {
        var axis = axes.Find(x => x.name == axisName);

        axis.SetAxisOff(inactiveMat, inactiveCol);
    }

    public void DisableAll()
    {
        axes.ForEach(x => x.SetAxisOff(inactiveMat, inactiveCol));
    }
}
