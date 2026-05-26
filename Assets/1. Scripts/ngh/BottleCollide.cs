using UnityEngine;

public class BottleCollide : MonoBehaviour
{
    private bool BottleSet; 
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("powder"))
        {
            Destroy(collision.gameObject);
            BottleSet = true; 
        }
    }
}
