using QuickOutline;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Template class for gazable targets
/// </summary>
public abstract class GazableTarget : StepEnabler
{
    /// <summary>
    /// List of outlines on object
    /// </summary>
    protected List<Outline> objectToHighlightOutline = new List<Outline>();
    /// <summary>
    /// List of all gazables on object
    /// </summary>
    protected List<Gazable> gazables = new List<Gazable>();
    /// <summary>
    /// List of all box colliders on object
    /// </summary>
    protected List<BoxCollider> colliders = new List<BoxCollider>();
    /// <summary>
    /// Object on scene to use gazer for
    /// </summary>
    protected GameObject objectToGaze;
    /// <summary>
    /// List of all the objects used in gazer target
    /// </summary>
    protected List<GameObject> objectsToGaze = new List<GameObject>();
    /// <summary>
    /// List of gameobjects and coresponding layer indexes for them
    /// </summary>
    private Dictionary<GameObject, int> layerIndexes = new Dictionary<GameObject, int>();
    /// <summary>
    /// Variable holding number of objects gazed at
    /// </summary>
    private int objectsGazed = 0;
    /// <summary>
    /// Number of targets we need to gaze at
    /// </summary>
    private int amountOfTargets;
    /// <summary>
    /// Temporary timer counting time we gazed at object
    /// </summary>
    private float timer = 0f;

    /// <summary>
    /// Sets up single gazable object
    /// </summary>

    protected void SetUpGazable()
    {
        timer = 0f;

        Enabled = false;

        amountOfTargets = 1;

        layerIndexes.Add(objectToGaze, objectToGaze.layer);

        var outline = objectToGaze.AddComponent<Outline>();
        objectToHighlightOutline.Add(outline);

        var gazable = objectToGaze.AddComponent<Gazable>();
        gazables.Add(gazable);

        var collider = objectToGaze.AddComponent<BoxCollider>();
        collider.isTrigger = true;
        collider.size *= 2;
        collider.center = Vector3.zero;
        colliders.Add(collider);

        gazable.finishedGazing.AddListener(() => { CheckEnd(outline, gazable, collider, objectToGaze); });

        objectToGaze.layer = 20;
    }

    protected void setCustomColliders(List<Vector3> customSizes, List<Vector3> customCenters)
    {
        for (int i = 0; i < Mathf.Min(colliders.Count, customSizes.Count, customCenters.Count); ++i)
        {
            colliders[i].size = customSizes[i];
            colliders[i].center = customCenters[i];
        }
    }

    /// <summary>
    /// Sets up gazable objects
    /// </summary>
    protected void SetUpGazables()
    {
        timer = 0f;

        Enabled = false;

        amountOfTargets = objectsToGaze.Count;

        for (int i = 0; i < objectsToGaze.Count; ++i)
        {
            layerIndexes.Add(objectsToGaze[i], objectsToGaze[i].layer);
        }

        for (int i = 0; i < objectsToGaze.Count; ++i)
        {
            var outline = objectsToGaze[i].gameObject.AddComponent<Outline>();
            objectToHighlightOutline.Add(outline);

            var gazable = objectsToGaze[i].gameObject.AddComponent<Gazable>();
            gazables.Add(gazable);

            var collider = objectsToGaze[i].AddComponent<BoxCollider>();
            collider.isTrigger = true;
            collider.center = Vector3.zero;
            collider.size = new Vector3(collider.size.x * 2f, collider.size.y * 2f, collider.size.z);
            if (objectsToGaze[i].GetComponentInChildren<PhysicalButton>())
            {
                collider.center = objectsToGaze[i].transform.forward * 0.1f;
                collider.size = new Vector3(collider.size.x * 5f,
                                            collider.size.y * 5f,
                                            collider.size.z * 7f);
            }
            colliders.Add(collider);

            var obj = objectsToGaze[i];
            obj.layer = 20;

            gazable.finishedGazing.AddListener(() => { CheckEnd(outline, gazable, collider, obj); });
        }
    }

    /// <summary>
    /// Removes outline from gazable
    /// </summary>
    /// <param name="outline">Outline reference</param>
    /// <param name="gazable">Gazable reference</param>
    /// <param name="collider">Collider of object</param>
    /// <param name="objectToGaze">Reference to gameobject with gazable and collider</param>
    private void CheckEnd(Outline outline, Gazable gazable, BoxCollider collider, GameObject objectToGaze)
    {
        objectToGaze.layer = layerIndexes[objectToGaze];
        objectsGazed++;
        RemoveComponents(outline, gazable, collider);
    }

    /// <summary>
    /// Removes added components from object
    /// </summary>
    /// <param name="outline">Outline reference</param>
    /// <param name="gazable">Gazable reference</param>
    /// <param name="collider">Collider of object</param>
    private void RemoveComponents(Outline outline, Gazable gazable, BoxCollider collider)
    {
        var outlineToDestroy = objectToHighlightOutline.Find(x => x == outline);
        var gazableToDestroy = gazables.Find(x => x == gazable);
        var colliderToDestroy = colliders.Find(x => x == collider);
        outlineToDestroy.StopAllCoroutines();

        objectToHighlightOutline.Remove(outlineToDestroy);
        Destroy(outlineToDestroy);
        gazables.Remove(gazableToDestroy);
        Destroy(gazableToDestroy);
        colliders.Remove(colliderToDestroy);
        Destroy(colliderToDestroy);
    }

    /// <summary>
    /// Cleans up all references from this object
    /// </summary>
    public void CleanObjects()
    {
        if (objectToGaze)
        {
            if (layerIndexes.ContainsKey(objectToGaze))
                objectToGaze.layer = layerIndexes[objectToGaze];
        }
        else
        {
            for (int i = 0; i < objectsToGaze.Count; ++i)
            {
                if (layerIndexes.ContainsKey(objectsToGaze[i]))
                    objectsToGaze[i].layer = layerIndexes[objectsToGaze[i]];
            }
        }

        objectToHighlightOutline.ForEach(x => { if (x) { x.StopAllCoroutines(); Destroy(x); } });
        gazables.ForEach(x => Destroy(x));
        colliders.ForEach(x => Destroy(x));

        objectToHighlightOutline.Clear();
        gazables.Clear();
        colliders.Clear();
    }

    /// <summary>
    /// Counts time looked at object
    /// </summary>
    protected override void onUpdate()
    {
        CheckForGazeCondition();
    }

    private void CheckForGazeCondition()
    {
        if (objectsGazed == amountOfTargets && !Enabled)
        {
            Enabled = true;
        }

        timer += Time.deltaTime;

        if (timer > 10f && timer < 13)
        {
            for (int i = 0; i < objectToHighlightOutline.Count; ++i)
            {
                objectToHighlightOutline[i].Blink();
            }
            timer = 14;
        }
    }

    protected override void cleanUp()
    {
        CleanObjects();
    }
}
