using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TruckController : MonoBehaviour {

    [SerializeField] List<GameObject> boxTypes;
    [SerializeField] float driveUpDistance = 0.4f;
    [SerializeField] float driveAwayDistance = -0.5f;
    [SerializeField] float movementSpeed = 2.0f;

    private StoreObject myTruck;

    private Vector3 dest;
    private Vector3 oldPos;
    private float startTime;
    private float distanceCovered = 0f;
    private float totalDistance;


    // Use this for initialization
    void Start () {
        myTruck = GetComponentInChildren<StoreObject>();
        SpawnBoxes();
        dest = myTruck.transform.localPosition;
        //DriveTo(driveUpDistance);
	}

    private void Update()
    {
        if (myTruck.transform.localPosition != dest)
        {
            distanceCovered = (Time.time - startTime) * (movementSpeed);
            float fractDist = distanceCovered / totalDistance;
            myTruck.transform.localPosition = Vector3.Lerp(oldPos, dest, fractDist);
        }
    }

    private void SpawnBoxes()
    {
        for (int i = 0; i < myTruck.maxObjects; i++)
        {
            int whichBox = Mathf.FloorToInt(UnityEngine.Random.Range(0, boxTypes.Count));
            myTruck.ReceiveObject(Instantiate(boxTypes[whichBox]));
        }
    }

    private void DriveTo(float distance)
    {
        oldPos = myTruck.transform.localPosition;
        startTime = Time.time;
        dest.Set(distance, oldPos.y, oldPos.z);
        totalDistance = Vector3.Distance(oldPos, dest);
    }
}
