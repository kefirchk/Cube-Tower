using UnityEngine;

public class CameraRotator : MonoBehaviour
{
    [SerializeField] private float speed = 1.0f;
    private Transform rotator;

    private void Start()
    {
        rotator = this.GetComponent<Transform>();
    }

    private void Update()
    {
        rotator.Rotate(0, speed * Time.deltaTime, 0);
    }

}
