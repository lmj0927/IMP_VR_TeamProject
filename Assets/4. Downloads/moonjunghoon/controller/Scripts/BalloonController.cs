using UnityEngine;
using UnityEngine.InputSystem;

public class BalloonController : MonoBehaviour
{
    [SerializeField]
    private InputActionAsset inputActions;


    [SerializeField]
    private GameObject balloonPrefab;
    private GameObject spawnedBalloon;

    private bool canRelease = false;

    [SerializeField]
    private float floatingStrength = 20f;

    private InputAction BalloonAction;

    private float inflateRate = 1.5f;

    [SerializeField]
    private Transform spawnTransform; 


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        BalloonAction = inputActions.FindActionMap("XRI Right Interaction").FindAction("Select");   
    }

    // Update is called once per frame
    void Update()
    {
        if(BalloonAction.WasPressedThisFrame()) // instantiate
        {
            CreateBalloon();
            Debug.Log("Balloon created");
        }
        else if(BalloonAction.WasReleasedThisFrame())  // release and start floating
        {
            canRelease = true; 
        } else if (BalloonAction.IsPressed())
        {
            InflateBalloon();
        }
    }

    private void CreateBalloon()
    {
        spawnedBalloon = Instantiate(balloonPrefab, spawnTransform.position, Quaternion.identity);
        spawnedBalloon.transform.localScale = new Vector3(.1f, .1f, .1f);
    }

    private void InflateBalloon()
    {
        if (spawnedBalloon.transform.localScale.y < 1.0f)
        {
            spawnedBalloon.transform.localScale += spawnedBalloon.transform.localScale * inflateRate * Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        // float
        if (canRelease)
        {
            Rigidbody rb = spawnedBalloon.GetComponent<Rigidbody>();

            Vector3 force = Vector3.up * floatingStrength;

            rb.AddForce(force);

            Destroy(spawnedBalloon, 10);

            spawnedBalloon = null; 
            canRelease = false;
        }
    }

}
