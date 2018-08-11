﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreObject : MonoBehaviour {

    [SerializeField] private List<GameObject> storedObjects = new List<GameObject> { };
    [SerializeField] public int maxObjects = 3;
    [SerializeField] private float objectSizeOffset = 0.6f;
    [SerializeField] private float ejectForce = 500.0f;

    private Transform world;

    private void Start()
    {
        ResetStoreTransforms();
        world = GameObject.FindGameObjectWithTag("World").transform;
    }

    // RECEIVE OBJECT
    // given an object, if not at max capacity, add to the list
    // and move it into the correct position
    public bool ReceiveObject(GameObject obj)
    {
        if (storedObjects.Count < maxObjects)
        {
            storedObjects.Add(obj);
            obj.transform.parent = transform;
            obj.GetComponent<BoxCollider>().enabled = false;
            obj.transform.localRotation = Quaternion.Euler(0, 0, 0);
            ResetStoreTransforms();
            return true;
        }
        return false;
    }

    // DISPENSE OBJECT
    // If we have available objects, return object from index 0 of list
    // must positions after removal w/ resetstoretransforms
    public Transform DispenseObject()
    {
        if (storedObjects.Count > 0)
        {
            GameObject temp = storedObjects[0];
            storedObjects.RemoveAt(0);
            ResetStoreTransforms();
            temp.GetComponent<BoxCollider>().enabled = true;
            return temp.transform;
        }
        return null;
    }

    // RESET STORE TRANSFORMS
    // Moves objects currently in list into a neat column based on width
    private void ResetStoreTransforms()
    {
        for (int i = 0; i < storedObjects.Count; i++)
        {
            storedObjects[i].transform.localPosition = new Vector3(0, objectSizeOffset + i * objectSizeOffset * 1.05f, 0);
        }
    }

    // EJECT OBJECTS
    // forcefully throws objects away
    public void EjectObjects()
    {
        for (int i = storedObjects.Count - 1; i > -1; i--)
        {
            GameObject temp = storedObjects[i];
            storedObjects.RemoveAt(i);
            temp.transform.parent = world;
            temp.GetComponent<BoxCollider>().enabled = true;
            Rigidbody rb = temp.GetComponent<Rigidbody>();
            rb.isKinematic = false;
            rb.AddForce(new Vector3(1f,0.3f,0) * ejectForce, ForceMode.Impulse);
        }
    }

    public void RemoveObjects()
    {
        foreach (GameObject obj in storedObjects)
        {
            Destroy(obj);
        }
        storedObjects.Clear();
    }

    public List<GameObject> GetStoredObjects()
    {
        return storedObjects;
    }

    public int NumObjects()
    {
        return storedObjects.Count;
    }

}
