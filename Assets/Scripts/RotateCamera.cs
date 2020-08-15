using UnityEngine;

public class RotateCamera : MonoBehaviour
{
    public float speed = 5f;

    private void Update() {
        transform.Rotate(0, speed * Time.deltaTime, 0);
    }
}
