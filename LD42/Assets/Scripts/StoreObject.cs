using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreObject : MonoBehaviour {

    [SerializeField] private List<GameObject> storedObjects = new List<GameObject> { };
    [SerializeField] private int maxObjects = 3;
    [SerializeField] private float objectSizeOffset = 0.6f;

    private void Start()
    {
        ResetStoreTransforms();
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
            obj.transform.localPosition = new Vector3(0, storedObjects.Count * objectSizeOffset * 1.1f, 0);
            obj.transform.localRotation = Quaternion.Euler(0, 0, 0);
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
            // TODO: remove hardcoded numbers
            storedObjects[i].transform.localPosition = new Vector3(0, objectSizeOffset + i * objectSizeOffset * 1.1f, 0);
        }
    }
}
