using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProBuilder.Core;

public class Scrolling : MonoBehaviour {

    [SerializeField] private float vectorAddOffset = 0.1f;

    Vector2 offset = new Vector2(0f, 0f);

	// SCROLLING
    // adjusts UV of conveyor continously to give illision of a moving conveyor belt
	void Update ()
    {
        offset += new Vector2(0, vectorAddOffset) * Time.deltaTime;
        GetComponent<Renderer>().material.mainTextureOffset = offset;
    }
}
