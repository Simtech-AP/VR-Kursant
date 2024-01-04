using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using TMPro;

public class UIObjectiveController : MonoBehaviour
{
    [SerializeField] private Transform UIObjectiveGrid;
    [SerializeField] private GameObject objectiveTextPrefab;

    [SerializeField] private Color completedTextColor;
    [SerializeField] private Color notCompletedTextColor;
    [SerializeField] private Color defaultTextColor;

    [SerializeField] private float maxFontSize = 0;

    private List<TextMeshProUGUI> objectives = new List<TextMeshProUGUI>();

    public void SetText(string objectivesText)
    {
        foreach (var o in objectives)
        {
            Destroy(o.gameObject);
        }

        objectives.Clear();

        var parsedObjectivesText = ParseObjectives(objectivesText);

        foreach (var text in parsedObjectivesText)
        {
            var objectiveObj = Instantiate(objectiveTextPrefab);
            objectiveObj.transform.parent = UIObjectiveGrid;
            //
            objectiveObj.transform.localPosition = new Vector3(0, 0, 0);
            objectiveObj.transform.localRotation = Quaternion.Euler(0, 0, 0);
            objectiveObj.transform.localScale = new Vector3(-1, 1, 1);
            //
            var objectiveTxt = objectiveObj.GetComponent<TextMeshProUGUI>();
            objectiveTxt.color = defaultTextColor;
            objectives.Add(objectiveTxt);
            objectiveTxt.text = text;
        }
    }

    public void UpdateTextLanguage(string objectivesText)
    {

        var parsedObjectivesText = ParseObjectives(objectivesText);

        if (parsedObjectivesText.Count != objectives.Count)
        {
            Debug.LogError("Something went wrong when updating a language. Parsed texts don't correspond to originals.");
            return;
        }

        for (int i = 0; i < objectives.Count; i++)
        {
            objectives[i].text = parsedObjectivesText[i];
        }
    }

    public void SetText(string text, int index)
    {
        objectives[index].text = text;
    }

    private List<string> ParseObjectives(string objectivesText)
    {
        var parsedResult = objectivesText.Split('#').ToList();
        var cleanedResult = new List<String>();

        parsedResult.ForEach(x => cleanedResult.Add(x.Trim()));
        cleanedResult.RemoveAll(x => x == String.Empty);

        return cleanedResult;
    }

    public void setObjectiveState(int objectiveIndex, bool state)
    {
        if (objectiveIndex < objectives.Count)
        {
            if (state)
            {
                objectives[objectiveIndex].color = completedTextColor;
            }
            else
            {
                objectives[objectiveIndex].color = notCompletedTextColor;
            }
        }
    }

    public void SetColorForBound(List<int> boundIdexes)
    {
        for (int i = 0; i < objectives.Count; ++i)
        {
            if (boundIdexes.Exists(x => x == i))
            {
                objectives[i].color = notCompletedTextColor;
            }
        }
    }
}