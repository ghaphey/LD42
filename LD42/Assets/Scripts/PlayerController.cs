using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    [Header("Properties")]
    [SerializeField] private float movementSpeed = 1.0f;
    [SerializeField] private float hitForce = 2.0f;
    [SerializeField] private float throwForce = 10.0f;
    [SerializeField] private float interactRange = 1.0f;
    [SerializeField] private float interactDamping = 0.025f;
    [Header("Objects")]
    [SerializeField] private Vector3 boxHold;
    [SerializeField] private Transform bottomHalf;
    [SerializeField] private Transform topHalf;
    [Header("Sounds")]
    [SerializeField] private AudioClip placeBoxSFX;
    [SerializeField] private AudioClip hitBoxSFX;


    private CharacterController charCont;
    private Vector3 facingDirection;
    private bool notHolding = true;
    private Transform currObj = null;
    private Transform world;

    private AudioSource audSrc;
    private GameObject currHit;
    private float interactTimeDamp = 0f;


    void Start () {
        facingDirection = new Vector3(1, 0, -1);
        charCont = GetComponent<CharacterController>();
        world = GameObject.FindGameObjectWithTag("World").transform;
        audSrc = GetComponent<AudioSource>();
	}
	
	void Update () {
        if (Time.timeScale > 0f)
        {
            MovePlayer();
            RotateBottomHalf();
            RotateTopHalf();
            ShowTarget();
            Interact();
        }
	}


    // INTERACT
    // If the player is within interact range of a box, it should be picked up
    // if the player is holding a box, and interact is pressed, it should be dropped (for now)
    private void Interact()
    {
        //Debug.DrawRay(topHalf.position, topHalf.forward * interactRange, Color.yellow);
        if (Input.GetButtonDown("Interact") && Time.time >= interactTimeDamp)
        {
            interactTimeDamp = Time.time + interactDamping;
            RaycastHit hit;
            if(Physics.Raycast(topHalf.position, topHalf.forward, out hit, interactRange))
            //if (Vector3.Distance(topHalf.position, currHit.transform.position) <= interactRange)
            {
                currHit = hit.transform.gameObject;
                //print(currHit.name);
                if (currHit.tag == "Box" && notHolding)
                {
                    PickUp(currHit.transform);
                }
                else if (currHit.tag == "Box" && !notHolding)
                {
                    DropObject();
                    PickUp(currHit.transform);
                }
                else if (currHit.tag == "Shelf" && notHolding)
                {
                    TakeObject(currHit.transform);
                }
                else if (currHit.tag == "Shelf" && !notHolding)
                {
                    GiveObject(currHit.transform);
                }
            }
            else if (!notHolding)
            {
                DropObject();
            }
        }
    }


    private void ShowTarget()
    {
        /*
        if (currHit != null && Vector3.Distance(topHalf.position, currHit.transform.position) <= interactRange)
        {
            targetArrow.GetComponentInChildren<MeshRenderer>().enabled = true;
            targetArrow.transform.position = currHit.transform.position + Vector3.up * arrowOffset;
        }
        else
        {
            targetArrow.GetComponent<MeshRenderer>().enabled = false;

        } */
    }

    // PICK UP
    // takes a passed object and sets it in the holding position
    // in front of the player. NOTE - This makes object a child object
    // also disables rigidbody physics
    private void PickUp(Transform obj)
    {
        obj.parent = topHalf.transform;
        obj.GetComponent<Rigidbody>().isKinematic = true;
        obj.transform.localPosition = boxHold;
        obj.localRotation = Quaternion.Euler(0, 0, 0);
        currObj = obj;
        notHolding = false;
        audSrc.PlayOneShot(placeBoxSFX);

    }

    // DROP OBJECT
    // Lets go of an object, resets its parent to world, renables rigidbody physics
    private void DropObject()
    {
        currObj.parent = world;
        Rigidbody temp = currObj.GetComponent<Rigidbody>();
        temp.isKinematic = false;
        temp.AddForce(transform.forward * throwForce, ForceMode.Impulse);
        currObj = null;
        notHolding = true;
        audSrc.PlayOneShot(hitBoxSFX);
    }

    // TAKE OBJECT
    // Remove object from shelf or currently holding receptical
    // REQUIRES: StoreObject script on shelf object
    private void TakeObject(Transform t)
    {
        currObj = t.GetComponent<StoreObject>().DispenseObject();
        if (currObj != null)
        {
            PickUp(currObj);
            audSrc.PlayOneShot(placeBoxSFX);
        }
    }

    // GIVE OBJECT
    // place object on shelf
    // REQUIRES: StoreObject script
    private void GiveObject(Transform t)
    {
        if (t.GetComponent<StoreObject>().ReceiveObject(currObj.gameObject))
        {
            currObj = null;
            notHolding = true;
        }
        audSrc.PlayOneShot(placeBoxSFX);
    }

    // MOVE PLAYER
    // adjusts player position based on vert/horz access modified by movementspeed
    private void MovePlayer()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        facingDirection.x = horizontal;
        facingDirection.z = vertical;

        // -9.8 for gravity, look at this later
        charCont.Move( new Vector3(horizontal * Time.deltaTime * movementSpeed,
                                    -9.8f,
                                    vertical * Time.deltaTime * movementSpeed));
        
    }

    // ROTATE BOTTOM HALF
    // rotates the bottom half to face the current movement vector direction
    private void RotateBottomHalf()
    {
        bottomHalf.LookAt(new Vector3(bottomHalf.position.x + facingDirection.x,
                                    bottomHalf.position.y,
                                    bottomHalf.position.z + facingDirection.z));
    }

    // ROTATE TOP HALF
    // rotate the top half to face the current x, y mouse position
    private void RotateTopHalf()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        int layermask = 1 << 9;
        if (Physics.Raycast(ray, out hit, 100, layermask))
        {
            //currHit = hit.collider.gameObject;
            topHalf.LookAt(new Vector3(hit.point.x, topHalf.position.y, hit.point.z));
        }
    }

    // ON CONTROLLER COLLIDER HIT
    // if player runs into a rigidbody, need to apply force to move it out of the way
    // this will mostly be for moving boxes aside
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.transform.tag != "Untagged")
        {
            Rigidbody rb = hit.collider.attachedRigidbody;
            //currHit = hit.gameObject;
            //print("hit " + currHit.name);
            if (rb == null || rb.isKinematic)
                return;
            rb.velocity = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z) * hitForce;
        }
    }
}
