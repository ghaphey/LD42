using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Incinerator : MonoBehaviour {

    // ON TRIGGER ENTER
    // Destroy a box when it touches the incinerator
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Box")
        {
            print("destroying " + other.name);
            Destroy(other.gameObject);
        }
    }
}
