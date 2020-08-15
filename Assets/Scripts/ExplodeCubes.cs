using UnityEngine;

public class ExplodeCubes : MonoBehaviour
{
    public float power = 150f;
    public float distanse = 5f;
    private bool _collisionSet;

    public GameObject mainCamera;
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Cube" && !_collisionSet)
        {
            for (int i = collision.transform.childCount - 1; i >= 0; --i)
            {
                Transform child = collision.transform.GetChild(i);
                child.gameObject.AddComponent<Rigidbody>();
                child.gameObject.GetComponent<Rigidbody>().AddExplosionForce(power, Vector3.up, distanse);
                child.SetParent(null);
            }
            Destroy(collision.gameObject);
            _collisionSet = true;
            Camera.main.transform.localPosition += new Vector3(0, 3f, -5f);
        }
        
    }
}
