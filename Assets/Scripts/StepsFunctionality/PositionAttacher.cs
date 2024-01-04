using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Allows attaching object to specified position
/// </summary>
[System.Serializable]
public class Attacher
{
    /// <summary>
    /// Object to attach to
    /// </summary>
    public GameObject attacher;
    /// <summary>
    /// Position to attach to
    /// </summary>
    public Vector3 position;

    /// <summary>
    /// Default constructor
    /// </summary>
    /// <param name="attacher">Object to attach to</param>
    /// <param name="position">Position to attach to</param>
    public Attacher(GameObject attacher, Vector3 position)
    {
        this.attacher = attacher;
        this.position = position;
    }

    /// <summary>
    /// Sets position of attached object
    /// </summary>
    public void SetPosition()
    {
        attacher.transform.localPosition = position;
        attacher.transform.localRotation = Quaternion.identity;
    }
}

/// <summary>
/// Allows parenting of attached positions objects
/// </summary>
[System.Serializable]
public class Attachment
{
    /// <summary>
    /// List of objects to parent
    /// </summary>
    public List<Attacher> attachers;
    /// <summary>
    /// Parent to attch the objects to
    /// </summary>
    public Transform parent;

    /// <summary>
    /// Default constructor
    /// </summary>
    /// <param name="attachers">List of attached objects</param>
    /// <param name="parent">Parent to attach objects to</param>
    public Attachment(List<Attacher> attachers, Transform parent)
    {
        this.attachers = attachers;
        this.parent = parent;
    }

    /// <summary>
    /// Attaches all objects to parent
    /// </summary>
    public void AttachToParent()
    {
        for (int i = 0; i < attachers.Count; ++i)
        {
            attachers[i].attacher.transform.SetParent(parent);
            attachers[i].SetPosition();
        }
    }

    /// <summary>
    /// Cleans parent from all ojects
    /// </summary>
    public void CleanParent()
    {
        for (int i = 0; i < attachers.Count; ++i)
        {
            attachers[i].attacher.transform.SetParent(null);
        }
    }
}

/// <summary>
/// Controls logic of attaching objects to parent
/// </summary>
public class PositionAttacher : MonoBehaviour
{
    /// <summary>
    /// List of all lists of objects to attach 
    /// </summary>
    [SerializeField]
    private List<Attachment> attachments;

    /// <summary>
    /// Attaches all objects to respective parents
    /// </summary>
    public void SetUpParents()
    {
        for (int i = 0; i < attachments.Count; ++i)
        {
            attachments[i].AttachToParent();
        }
    }

    /// <summary>
    /// Detaches objects from parents
    /// </summary>
    public void CleanUpParents()
    {
        for (int i = 0; i < attachments.Count; ++i)
        {
            attachments[i].CleanParent();
        }
    }

    /// <summary>
    /// Sets list of attached objects
    /// </summary>
    /// <param name="att">List of attachment objects</param>
    public void SetAttachments(List<Attachment> att) { attachments = att; }
}
