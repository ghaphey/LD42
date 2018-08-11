using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreObject : MonoBehaviour {

    [SerializeField] private List<GameObject> storedObjects = new List<GameObject> { };
    [SerializeField] private int maxObjects = 3;
    [SerializeField] private float objectSizeOffset = 0.6f;

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

    private void ResetStoreTransforms()
    {
        for (int i = 0; i < storedObjects.Count; i++)
        {
            storedObjects[i].transform.localPosition = new Vector3(0, 0.6f + i * objectSizeOffset * 1.1f, 0);
        }
    }
}
