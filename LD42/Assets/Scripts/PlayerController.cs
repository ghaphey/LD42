using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    [SerializeField] private float movementSpeed = 1.0f;
    [SerializeField] private float hitForce = 2.0f;
    [SerializeField] private float throwForce = 10.0f;
    [SerializeField] private float interactRange = 1.0f;
    [SerializeField] private Vector3 boxHold;
    [SerializeField] private AudioClip placeBoxSFX;
    [SerializeField] private AudioClip hitBoxSFX;


    private CharacterController charCont;
    private Vector3 facingDirection;
    private bool notHolding = true;
    private Transform currObj = null;
    private Transform world;

    private AudioSource audSrc;


    void Start () {
        facingDirection = new Vector3(1, 0, -1);
        charCont = GetComponent<CharacterController>();
        world = GameObject.FindGameObjectWithTag("World").transform;
        audSrc = GetComponent<AudioSource>();
	}
	
	void Update () {
        MovePlayer();
        RotatePlayer();
        Interact();
	}


    // INTERACT
    // If the player is within interact range of a box, it should be picked up
    // if the player is holding a box, and interact is pressed, it should be dropped (for now)
    private void Interact()
    {
        if (Input.GetButtonDown("Interact"))
        {
            RaycastHit hit;
            if(Physics.Raycast(transform.position, transform.forward,out hit, interactRange))
            {
                if (hit.collider.tag == "Box" && notHolding)
                {
                    PickUp(hit.transform);
                }
                else if (hit.collider.tag == "Shelf" && notHolding)
                {
                    TakeObject(hit.transform);
                }
                else if (hit.collider.tag == "Shelf" && !notHolding)
                {
                    GiveObject(hit.transform);
                }
            }
            else if (!notHolding)
            {
                DropObject();
            }
        }
    }

    // PICK UP
    // takes a passed object and sets it in the holding position
    // in front of the player. NOTE - This makes object a child object
    // also disables rigidbody physics
    private void PickUp(Transform obj)
    {
        obj.parent = transform;
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
        }
        audSrc.PlayOneShot(placeBoxSFX);
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

    // ROTATE PLAYER
    // rotates the player to face the current movement vector direction
    private void RotatePlayer()
    {
        transform.LookAt(new Vector3(transform.position.x + facingDirection.x, 
                                    transform.position.y, 
                                    transform.position.z + facingDirection.z));
    }

    // ON CONTROLLER COLLIDER HIT
    // if player runs into a rigidbody, need to apply force to move it out of the way
    // this will mostly be for moving boxes aside
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody rb = hit.collider.attachedRigidbody;
        if (rb == null || rb.isKinematic)
            return;
        rb.velocity = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z) * hitForce;
    }
}
