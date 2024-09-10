using UnityEngine;

public class Distance : MonoBehaviour
{
    private void Start()
    {
        gameObject.GetComponent<Rigidbody>().velocity = Vector3.right * 15f;
    }
}
