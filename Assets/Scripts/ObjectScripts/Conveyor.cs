using System.Linq;
using UnityEngine;

/// <summary>
/// Class holding all logic for conveyor belts
/// </summary>
public class Conveyor : MonoBehaviour
{
    /// <summary>
    /// Speed of conveyor belt
    /// </summary>
    [Range(-1f, 1f)]
    [SerializeField]
    private float speed = 0f;
    /// <summary>
    /// Base conveor mesh renderer
    /// </summary>
    [SerializeField]
    private Renderer conveyorRenderer = default;
    /// <summary>
    /// Parent object of conveyor belt
    /// </summary>
    [SerializeField]
    private Transform attachedObjectsParent = default;
    /// <summary>
    /// Public accessor for conveyor belt parent
    /// </summary>
    public Transform ConveyorParent { get => attachedObjectsParent; }

    public bool IsBusy { get => speed != 0f; }

    /// <summary>
    /// Moves texture of conveyor according to specified speed
    /// </summary>
    private void Update()
    {
        float offset = Time.time * speed;
        conveyorRenderer.material.SetTextureOffset("_MainTex", new Vector2(0, offset));
        attachedObjectsParent.position += new Vector3(0, 0, -speed * Time.deltaTime);
    }

    /// <summary>
    /// Starts the conveyor with specified speed
    /// </summary>
    /// <param name="_speed"></param>
    public void Launch(float _speed)
    {
        speed = _speed;
    }

    /// <summary>
    /// Stops the conveyor from moving
    /// </summary>
    public void Stop()
    {
        speed = 0f;
    }
}
