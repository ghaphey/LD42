using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorBehavior : MonoBehaviour {

    [SerializeField] private float conveyorForce = 50f;

    private void OnCollisionStay(Collision collision)
    {
        //transform.GetComponent<Rigidbody>().AddForce(collision.transform.forward * conveyorForce, ForceMode.Force);
        if (collision.transform.tag == "Conveyor")
        {
            transform.GetComponent<Rigidbody>().velocity = conveyorForce 
                                                            * collision.transform.forward  
                                                            * Time.deltaTime;
            transform.GetComponent<Rigidbody>().freezeRotation = true;
        }
        else
            transform.GetComponent<Rigidbody>().freezeRotation = false;
    }
}
