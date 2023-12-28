using UnityEngine;

public class EventOnCollision : MonoBehaviour
{
    public Events<Collider> onCollisionEnter = new();
    public Events<Collider> onCollisionExit = new();

    private void OnTriggerEnter(Collider other) => onCollisionEnter.Trigger(other);

    private void OnTriggerExit(Collider other) => onCollisionExit.Trigger(other);
}
