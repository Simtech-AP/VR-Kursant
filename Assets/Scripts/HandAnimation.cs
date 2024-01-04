using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// Class for animating hand when neat interactables
/// </summary>
public class HandAnimation : MonoBehaviour
{
    /// <summary>
    /// Default rotations of hand
    /// </summary>
    private List<Quaternion> defaultRotations = new List<Quaternion>();
    /// <summary>
    /// Rotations of joints when index finger is extended
    /// </summary>
    private List<Quaternion> fingerRotations = new List<Quaternion>();
    /// <summary>
    /// Parent object of default hand rotations
    /// </summary>
    [SerializeField]
    private Transform handBonesParent = default;
    /// <summary>
    /// All of the transforms of default hand bones
    /// </summary>
    private List<Transform> handBones = new List<Transform>();
    /// <summary>
    /// Parent object of hand rotations with index finger extended
    /// </summary>
    [SerializeField]
    private Transform fingerHandBonesParent = default;
    /// <summary>
    /// All of the transforms of extended index finger hand
    /// </summary>
    private List<Transform> fingerHandBones = new List<Transform>();
    /// <summary>
    /// Sequence allowing hand to smoothly transition to and from finger rotations
    /// </summary>
    private Sequence handMove;
    /// <summary>
    /// List of all interactables on scene
    /// </summary>
    private List<Interactable> interactables = new List<Interactable>();
    /// <summary>
    /// Is the index finger extended?
    /// </summary>
    private bool fingerOut = false;
    /// <summary>
    /// Minimum distance for animation to trigger
    /// </summary>
    [SerializeField]
    private float minimumDistance = 1f;

    /// <summary>
    /// Get all transforms from default and extended finger parent objects
    /// Collect all interactables available on scene
    /// </summary>
    private void Start()
    {
        foreach (Transform t in fingerHandBonesParent.GetComponentsInChildren<Transform>(true))
        {
            fingerRotations.Add(t.localRotation);
            fingerHandBones.Add(t);
        }
        foreach (Transform t in handBonesParent.GetComponentsInChildren<Transform>(true))
        {
            defaultRotations.Add(t.localRotation);
            handBones.Add(t);
        }
        CollectInteractables();
    }

    /// <summary>
    /// Start switch to default hand bone rotations
    /// </summary>
    public void SwitchToDefault()
    {
        fingerOut = false;
        handMove.Pause();
        handMove.Kill();
        handMove = DOTween.Sequence();
        int index = 0;
        foreach (Transform t in handBones)
        {
            handMove.Join(t.DOLocalRotateQuaternion(defaultRotations[index], 0.3f));
            index++;
        }
        handMove.Play();
    }

    /// <summary>
    /// Start switch to extended index finger hand bone rotations
    /// </summary>
    public void SwitchToFinger()
    {
        fingerOut = true;
        handMove.Pause();
        handMove.Kill();
        handMove = DOTween.Sequence();
        int index = 0;
        foreach (Transform t in handBones)
        {
            handMove.Join(t.DOLocalRotateQuaternion(fingerRotations[index], 0.3f));
            index++;
        }
        handMove.Play();
    }

    /// <summary>
    /// Gets all interactables on scene
    /// </summary>
    [ContextMenu("Collect interactables")]
    private void CollectInteractables()
    {
        interactables.Clear();
        foreach (Interactable i in InteractablesManager.Instance.Interactables)
        {
            interactables.Add(i);
        }
    }

    /// <summary>
    /// Continously check if we are near interactable
    /// </summary>
    private void Update()
    {
        float distance = float.MaxValue;
        foreach (Interactable i in interactables)
        {
            if (Vector3.Distance(transform.position, i.transform.position) < distance)
            {
                distance = Vector3.Distance(transform.position, i.transform.position);
            }
        }
        if (distance < minimumDistance && fingerOut == false)
        {
            SwitchToFinger();
        }
        else if (distance > minimumDistance && fingerOut == true)
        {
            SwitchToDefault();
        }
    }
}
