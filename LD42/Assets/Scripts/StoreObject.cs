using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StoreObject : MonoBehaviour {

    [SerializeField] private List<GameObject> storedObjects = new List<GameObject> { };
    [SerializeField] public int maxObjects = 3;
    [SerializeField] private float objectSizeOffset = 0.6f;
    [SerializeField] private float ejectForce = 500.0f;
    [SerializeField] private int storedBoxPoints = 1;
    [SerializeField] private int boxPointsInterval = 1;
    [SerializeField] private GameObject floatTextPrefab;
    [SerializeField] private float floatTextOffset = 1.5f;

    [SerializeField] private List<Animator> braces = new List<Animator> { };

    private Transform world;
    private ScoreBoard score;
    private GameObject nText;

    private int timer = 0;

    private void Start()
    {
        ResetStoreTransforms();
        world = GameObject.FindGameObjectWithTag("World").transform;
        score = GameObject.FindGameObjectWithTag("Score").GetComponent<ScoreBoard>();
        if (braces.Count > 0 && score != null)
        {
            CreateFloatText();
        }
    }

    private void CreateFloatText()
    {
        Vector3 textLocation = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * floatTextOffset);
        nText = Instantiate(floatTextPrefab, textLocation, Quaternion.identity, GameObject.FindGameObjectWithTag("Canvas").transform);
        nText.SetActive(false);
    }

    private void Update()
    {
        if (braces.Count > 0 && Time.time > timer)
        {
            timer += boxPointsInterval;
            if (storedObjects.Count != 0 && score != null)
            {
                int calcBoxPoints = storedObjects.Count * storedBoxPoints;
                DisplayFloatText(calcBoxPoints);
                score.AddShelfScore(calcBoxPoints);
            }
        }
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
        if (storedObjects.Count <= 0)
            return null;
        int index = 0;
        // if this object has braces, then its a shelf, otherwise its on a truckbed
        // switches between FIFO (shelf) and truckbed (FILO)
        if (braces.Count ==  0)
            index = storedObjects.Count - 1;
        

        GameObject temp = storedObjects[index];
        storedObjects.RemoveAt(index);
        ResetStoreTransforms();
        temp.GetComponent<BoxCollider>().enabled = true;
        return temp.transform;
    }

    // RESET STORE TRANSFORMS
    // Moves objects currently in list into a neat column based on width
    private void ResetStoreTransforms()
    {
        for (int i = 0; i < storedObjects.Count; i++)
        {
            storedObjects[i].transform.localPosition = new Vector3(0,
                                                        objectSizeOffset + i * objectSizeOffset * 1.05f,
                                                        0);
            storedObjects[i].transform.rotation = Quaternion.identity;
        }
        if (braces.Count > 0)
        {
            for (int i = 0; i < maxObjects; i++)
            {
                if (i <= storedObjects.Count - 1)
                    braces[i].SetBool("BraceClosed", true);
                else
                    braces[i].SetBool("BraceClosed", false);
            }
        }
    }

    // EJECT OBJECTS
    // forcefully throws objects away
    // returns true if threw stuff away
    public bool EjectObjects()
    {
        if (storedObjects.Count <= 0)
            return false;
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
        return true;
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

    private void DisplayFloatText(int points)
    {
        if (score != null)
        {
            nText.GetComponentInChildren<TextMeshProUGUI>().text = "+" + points.ToString();
            nText.SetActive(true);
        }
    }

}
