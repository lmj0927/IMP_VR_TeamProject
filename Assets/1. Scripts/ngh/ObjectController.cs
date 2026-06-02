using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class ObjectController : MonoBehaviour
{
    public GameObject bottle, cube;
    public GameObject bottlecollider; 
    private BottleCollide bottlecolliderscript; 

    private bool grab, set;
    private Vector3 pos;

    [SerializeField]
    public float ydir;

    private bool moveup,movedown;
    private int count;

    private void Start()
    {
        bottlecolliderscript = bottlecollider.GetComponent<BottleCollide>();
        set = bottlecolliderscript.BottleSet; 

    }

    void Update()
    {
        //Vector3 camerapos = Camera.main.transform.position;

        set = bottlecolliderscript.BottleSet; 
        if (grab && set)
        {

            ShakingCount(); 

        }
    }

    public void ShakingCount()
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
                bottlecolliderscript.BottleSet = false; 
                //Debug.Log("set : " + set); 
            }
        }
    }
  
 
    public void HandleSelectEnter(SelectEnterEventArgs args)
    {
        if (args.interactableObject.transform.CompareTag("Bottle"))
        {
            grab = true;
            pos = args.interactableObject.transform.position;
            //Debug.Log(pos);
        }
    }


    public void CanSelectEnter(SelectEnterEventArgs args)
    {
        Vector3 locpos = args.interactableObject.transform.position; 
        Instantiate(cube, new Vector3(locpos.x,locpos.y+0.02f,locpos.z), Quaternion.identity);
        //Debug.Log("a / " + locpos);
    }



}
