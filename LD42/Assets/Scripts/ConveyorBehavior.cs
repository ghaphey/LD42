using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorBehavior : MonoBehaviour {

    [SerializeField] private float conveyorForce = 50f;

    // ON COLLISION STAY
    // if this object collides with a conveyor, we want to move it along that conveyor belt
    // using a continous force. Additionally, we adjust and free part of its rotation so it doesnt
    // do flips accross the conveyor belt
    private void OnCollisionStay(Collision collision)
    {
        if (collision.transform.tag == "Conveyor")
        {
            transform.GetComponent<Rigidbody>().velocity = conveyorForce 
                                                            * collision.transform.forward  
                                                            * Time.deltaTime;
            transform.rotation = Quaternion.Euler(new Vector3(0.0f, transform.rotation.y, transform.rotation.z));
            transform.GetComponent<Rigidbody>().freezeRotation = true;
        }
        else
            transform.GetComponent<Rigidbody>().freezeRotation = false;
    }
}
