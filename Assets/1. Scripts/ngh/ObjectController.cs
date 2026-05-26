using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ObjectController : MonoBehaviour
{
    public GameObject bottle, cube;
    private bool grab;
    private Vector3 pos;

    [SerializeField]
    public float ydir;

    private bool moveup,movedown;
    private int count;

    private void Start()
    {
        
    }

    void Update()
    {
        Vector3 camerapos = Camera.main.transform.position;

        if (grab)
        {

            ShakingCount(); 

        }
    }

    private void ShakingCount()
    {
        if (bottle.transform.position.y - pos.y >= ydir)
        {
            moveup = true;


        }
        else if (moveup & bottle.transform.position.y - pos.y <= (-ydir))
        {
            movedown = true;

        }

        if (moveup & movedown)
        {
            
            
            count++;
            Debug.Log(count); 

            movedown = false;
            moveup = false; 
            
            if(count == 12)
            {
                //.
            }
        }
    }
  
 
    public void HandleSelectEnter(SelectEnterEventArgs args)
    {
        if (args.interactableObject.transform.CompareTag("Bottle"))
        {
            
            grab = true;
            pos = args.interactableObject.transform.position;
            Debug.Log(pos);
        }
    }


    public void CanSelectEnter(SelectEnterEventArgs args)
    {
        Vector3 locpos = args.interactableObject.transform.position; 
        Instantiate(cube, new Vector3(locpos.x,locpos.y,locpos.z), Quaternion.identity);
        Debug.Log("a / " + locpos);
    }

    

}
