using System;
using UnityEngine;

public class CarBehaviour : MonoBehaviour
{
    [SerializeField] private EventOnCollision softCollider;
    [SerializeField] private EventOnCollision frontCollider;

    [SerializeField] private float acceleration = 1;

    bool closeToCar = false;
    bool stopped = false;
    private void Awake()
    {
        softCollider.onCollisionEnter.Insert((col) => OnSoftCollision(col.tag,true), this);
        softCollider.onCollisionExit.Insert((col) => OnSoftCollision(col.tag, false), this);
        frontCollider.onCollisionEnter.Insert((col) => OnFrontCollision(col.tag, true), this);
        frontCollider.onCollisionExit.Insert((col) => OnFrontCollision(col.tag, false), this);
    }

    private void OnSoftCollision(string tag, bool entering)
    {
        if (tag != "SoftCollider") return;

        closeToCar = entering;
    }
    private void OnFrontCollision(string tag, bool entering)
    {
        if (tag == "Deleter")
        {
            Destroy(gameObject);
            return;
        }

        if (tag != "StopLight") return;

        stopped = entering;
    }

    private void Update()
    {
        if (stopped) return;

        var movement = (Time.deltaTime * acceleration) * (closeToCar ? 0.3f : 1f);

        transform.position = transform.position + transform.forward * movement;
    }

    
}
