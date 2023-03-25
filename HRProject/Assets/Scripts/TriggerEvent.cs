using UnityEngine;
using UnityEngine.Events;

public class TriggerEvent : MonoBehaviour
{
    [System.Flags]
    private enum TriggerType
    {
        OnTriggerEnter = 1,
        OnTriggerExit = 2,
    }

    [SerializeField, Tooltip("This is for when the event gets called")]
    private TriggerType type = 0;
    [SerializeField, Tooltip("This is the tag of the collider that triggers this event")]
    private string targetTag = "Player";
    [SerializeField, Tooltip("This is whether the event can be trigger multiple times")]
    private bool repeatEvent = false;

    [Space(10)]
    public UnityEvent onTrigger = new UnityEvent();

    [SerializeField, Tooltip("Make debug data visible in scene")]
    private bool drawDebug = false;

    // Event trigger is only true when the event will never be called again
    private bool eventTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        AttemptTrigger(TriggerType.OnTriggerEnter, other);
    }

    private void OnTriggerExit(Collider other)
    {
        AttemptTrigger(TriggerType.OnTriggerExit, other);
    }

    private void AttemptTrigger(TriggerType triggerTest, Collider other)
    {
        if (eventTriggered)
            return;

        // Determine if type includes the relevant trigger called
        if ((type & triggerTest) == 0)
            return;

        if (other.CompareTag(targetTag))
            onTrigger.Invoke();

        eventTriggered = !repeatEvent;
    }

    private void OnDrawGizmos()
    {
        if (!drawDebug)
            return;

        // Get col for drawing
        if (!TryGetComponent(out BoxCollider col))
            return;

        // Set color
        Color color = Color.yellow;
        color.a = 0.2f;
        Gizmos.color = color;

        // Assign transform matrix for rotation
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
        Gizmos.DrawCube(col.center, col.size);
    }
}
