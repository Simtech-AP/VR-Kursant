using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolInteractedObjectMaterialAdjuster : MonoBehaviour
{
    [SerializeField]
    private LayerMask collisionLayers = default;

    [SerializeField]
    private List<Collider> collidersInCollision = new List<Collider>();

    private bool isColorAdjusting = default;
    private bool isRed = default;

    [SerializeField]
    private Renderer objectRenderer = default;

    [SerializeField]
    private BoxCollider col = default;


    public void OnInteractionStarted()
    {
        col.isTrigger = true;
        InitializeColorChangeListening();
    }

    public void OnInteractionEnded()
    {
        col.isTrigger = false;
        FinalizeColorChangeListening();
    }

    private void InitializeColorChangeListening()
    {
        isColorAdjusting = true;
        StartCoroutine(adjustColorOncollision());
    }

    private void FinalizeColorChangeListening()
    {
        isColorAdjusting = false;
        objectRenderer.material.color = new Color(1, 1, 1, 1);
        isRed = false;
    }

    private IEnumerator adjustColorOncollision()
    {
        while (isColorAdjusting)
        {
            if (collidersInCollision.Count >= 1 && !isRed)
            {
                objectRenderer.material.color = new Color(1, 0.5f, 0.5f, 1);
                isRed = true;
            }
            else if (collidersInCollision.Count == 0 && isRed)
            {
                objectRenderer.material.color = new Color(1, 1, 1, 1);
                isRed = false;
            }

            yield return null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        bool _isCollisionOnInteractiveLayer = (collisionLayers.value & (1 << other.gameObject.layer)) > 0;

        if (!collidersInCollision.Contains(other) && _isCollisionOnInteractiveLayer)
        {
            collidersInCollision.Add(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (collidersInCollision.Contains(other))
        {
            collidersInCollision.Remove(other);
        }
    }
}
