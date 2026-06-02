using UnityEngine;

public class BottleCollide : MonoBehaviour
{
    public bool BottleSet=false; 
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("powder"))
        {
            Destroy(collision.gameObject);
            BottleSet = true;
            Debug.Log("Set");
        }


    }
}

