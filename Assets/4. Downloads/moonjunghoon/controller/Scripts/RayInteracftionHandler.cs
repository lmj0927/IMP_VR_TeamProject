using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class RayInteracftionHandler : MonoBehaviour
{
    
    public void HandleHoverEnter(HoverEnterEventArgs args)
    {
        if (args.interactableObject.transform.CompareTag("SimpleTarget"))
        {
            Debug.Log("Aiming at target: "+ args.interactableObject.transform.gameObject.name);
        }
    }

    public void HandleSelectEnter(SelectEnterEventArgs args)
    {
        if(args.interactableObject.transform.CompareTag("SimpleTarget"))
        {
            Color randomCol = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f); 

            args.interactableObject.transform.GetComponent<Renderer>().material.color = randomCol;
        }
    }

}
