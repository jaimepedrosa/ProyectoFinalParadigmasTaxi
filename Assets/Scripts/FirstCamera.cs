using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    public Transform taxi;

    private Vector3 offset;

    private void Start()
    {
        offset = transform.position - taxi.position;
    }

    private void LateUpdate()
    {
        transform.position = taxi.position + offset;

        transform.rotation = taxi.rotation;
    }
}

