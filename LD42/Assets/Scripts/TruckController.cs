using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TruckController : MonoBehaviour {

    [SerializeField] private List<GameObject> boxTypes;
    [SerializeField] private float driveUpDistance = 0.4f;
    [SerializeField] private float driveAwayDistance = -0.5f;
    [SerializeField] private float movementSpeed = 2.0f;
    [SerializeField] [Range(10, 120)] private int maxSpawnTime;
    [SerializeField] [Range(1, 10)] private int minSpawnTime;
    [Header("Counter Properties")]
    [SerializeField] TextMeshPro counter;
    [SerializeField] private int maxLoiterTime = 20;

    private StoreObject myTruck;

    private Vector3 dest;
    private Vector3 oldPos;
    private float startLerpTime;
    private float distanceCovered = 0f;
    private float totalDistance;

    private float spawnTimer = 0.0f;
    private bool spawnWait = true;

    private float loiterTimer = 0.0f;


    void Start ()
    {
        myTruck = GetComponentInChildren<StoreObject>();
        dest = myTruck.transform.localPosition;
        counter.text = "-";
        SetSpawnCounter();
    }

    private void Update()
    {
        // move truck to the current destination
        if (myTruck.transform.localPosition != dest)
        {
            LerpTruckToDest();
        }
        // if the spawn timer has been reached, move forward with boxes
        if (spawnWait && Time.time >= spawnTimer)
        {
            DeliverBoxes();
        }
        // while in delivery mode
        else if (spawnWait == false)
        {
            if ((Time.time < loiterTimer) & (myTruck.NumObjects() > 0))
            {
                counter.text = Mathf.FloorToInt(loiterTimer - Time.time).ToString();
            }
            // if the loiter timer expires, drive away (and force the remaining boxes off)
            else
            {
                DriveAway();
            }
        }

    }

    // SET SPAWN COUNTER
    // pick a random number between min / max spawn time and set a timer
    private void SetSpawnCounter()
    {
        spawnTimer = Time.time + UnityEngine.Random.Range(minSpawnTime, maxSpawnTime);
    }

    // LERP TRUCK TO DEST
    // move the truck towards the destination vector using a lerp
    // CONDITION: dest should not equal current truck position
    private void LerpTruckToDest()
    {
        distanceCovered = (Time.time - startLerpTime) * (movementSpeed);
        float fractDist = distanceCovered / totalDistance;
        myTruck.transform.localPosition = Vector3.Lerp(oldPos, dest, fractDist);
    }

    // DELIVER BOXES
    // Spawn boxes within the truck, set the dest vector, set the loiter timer
    // CONDITION: should not already be delivering, truck should be empty
    private void DeliverBoxes()
    {
        spawnWait = false;
        SpawnBoxes();
        DriveTo(driveUpDistance);
        loiterTimer = Time.time + maxLoiterTime;
    }

    // DRIVE TO
    // pass in distance point (x value) to move the truck to (local position relative to truck spawner)
    // sets the necessary variables for a lerp, tweak movement speed to increase lerp movement speed
    private void DriveTo(float distance)
    {
        oldPos = myTruck.transform.localPosition;
        startLerpTime = Time.time;
        dest.Set(distance, oldPos.y, oldPos.z);
        totalDistance = Vector3.Distance(oldPos, dest);
    }

    // SPAWN BOXES
    // put a random assortment of boxes into the truck, up to the maximum number of boxes
    // CONDITION: truck should be empty before calling
    private void SpawnBoxes()
    {
        for (int i = 0; i < myTruck.maxObjects; i++)
        {
            int whichBox = Mathf.FloorToInt(UnityEngine.Random.Range(0, boxTypes.Count));
            myTruck.ReceiveObject(Instantiate(boxTypes[whichBox]));
        }
    }

    // DRIVE AWAY
    // sets the spawn counter, drives away from entry, and ejects remaining objects
    // CONDITION: should have been in delivery state before calling
    private void DriveAway()
    {
        spawnWait = true;
        SetSpawnCounter();
        myTruck.EjectObjects();
        DriveTo(driveAwayDistance);
        counter.text = "-";
    }

}
