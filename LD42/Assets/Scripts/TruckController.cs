using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TruckController : MonoBehaviour {

    [SerializeField] private List<GameObject> boxTypes;
    [SerializeField] private UIController trucksRemaining;
    [Header("Truck Properties")]
    [SerializeField] private float driveUpDistance = 0.4f;
    [SerializeField] private float driveAwayDistance = -0.5f;
    [SerializeField] private float movementSpeed = 2.0f;
    [SerializeField] private int maxSpawnTime = 30;
    [SerializeField] private int minSpawnTime = 1;
    [Header("Counter Properties")]
    [SerializeField] private TextMeshPro counter;
    [SerializeField] private int maxLoiterTime = 20;
    [Header("Sound Properties")]
    [SerializeField] private AudioClip arriveSFX;
    [SerializeField] private AudioClip leaveSFX;
    [SerializeField] private AudioClip hitSFX;
    [SerializeField] private AudioClip honkSFX;
    [SerializeField] private AudioClip successSFX;
    [SerializeField] private AudioClip clappingSFX;

    private StoreObject myTruck;

    private enum type { delivery, pickup };
    private type truckType;

    private Vector3 dest;
    private Vector3 oldPos;
    private float startLerpTime;
    private float distanceCovered = 0f;
    private float totalDistance;

    private float spawnTimer = 0.0f;
    private bool spawnWait = true;

    private float loiterTimer = 0.0f;

    private OrderLight displayLights;
    private AudioSource audSrc;


    void Start ()
    {
        myTruck = GetComponentInChildren<StoreObject>();
        audSrc = GetComponent<AudioSource>();
        dest = myTruck.transform.localPosition;
        displayLights = GetComponentInChildren<OrderLight>();
        counter.text = "-";
        SetSpawnCounter();
    }

    private void Update()
    {
            switch (truckType)
            {
                case type.delivery:
                    DeliveryTruck();
                    break;
                case type.pickup:
                    PickupTruck();
                    break;
            }

    }

    // DELIVERY TRUCK
    // move forward with boxes based on a spawn timer, which starts loiter timer
    // when loiter timer expires, eject remaining boxes and move backward
    // reset spawn timer
    private void DeliveryTruck()
    {
        if (myTruck.transform.localPosition != dest)
        {
            LerpTruckToDest();
        }
        // if the spawn timer has been reached, move forward with boxes
        if (spawnWait && Time.time >= spawnTimer)
        {
            StartDelivery();
        }
        // while in delivery mode
        else if (spawnWait == false)
        {
            if (loiterTimer - Time.time < 10)
                counter.faceColor = Color.red;
            if ((Time.time < loiterTimer) && (myTruck.NumObjects() > 0))
            {
                counter.text = Mathf.FloorToInt(loiterTimer - Time.time).ToString();
            }
            // if the loiter timer expires, drive away (and force the remaining boxes off)
            else
            {
                EndDelivery();
                counter.faceColor = Color.white;
                trucksRemaining.TruckDepart();
                SwitchTypes();
            }
        }
    }


    // PICKUP TRUCK
    // move forward with order based on spawn timer, start loiter timer
    // when loiter time expires, leave
    // if get the right cargo in time, leaves early and plays victory music
    private void PickupTruck()
    {
        if (myTruck.transform.localPosition != dest)
        {
            LerpTruckToDest();
        }
        if (spawnWait && Time.time >= spawnTimer)
        {
            StartPickup();
        }
        else if (spawnWait == false)
        {
            if (loiterTimer - Time.time < 10)
                counter.faceColor = Color.red;
            // if the cargo matches order, can leave right away
            if (Time.time < loiterTimer && !CompareCargo())
            {
                counter.text = Mathf.FloorToInt(loiterTimer - Time.time).ToString();
            }
            else
            {
                EndPickup();
                counter.faceColor = Color.white;
                trucksRemaining.TruckDepart();
                SwitchTypes();
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
    private void StartDelivery()
    {
        spawnWait = false;
        if (myTruck.NumObjects() > 0)
            myTruck.RemoveObjects();
        SpawnBoxes();
        DriveTo(driveUpDistance);
        loiterTimer = Time.time + maxLoiterTime;
        audSrc.PlayOneShot(arriveSFX);
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

    // END DELIVERY
    // sets the spawn counter, drives away from entry, and ejects remaining objects
    // CONDITION: should have been in delivery state before calling
    private void EndDelivery()
    {
        spawnWait = true;
        SetSpawnCounter();
        if (myTruck.EjectObjects())
        {
            audSrc.PlayOneShot(honkSFX);
            audSrc.PlayOneShot(hitSFX);
        }
        DriveTo(driveAwayDistance);
        counter.text = "-";
        audSrc.PlayOneShot(leaveSFX);
    }

    // START PICKUP 
    // set loiter timer, drive up, and pick order of boxes
    // checks for previous objects in cargo and deletes them
    private void StartPickup()
    {
        spawnWait = false;
        DriveTo(driveUpDistance);
        if (myTruck.NumObjects() > 0)
            myTruck.RemoveObjects();
        PickOrderColors();
        loiterTimer = Time.time + maxLoiterTime;
        audSrc.PlayOneShot(arriveSFX);
    }

    // PICK ORDER COLORS
    // construct a list of colors, then apply that list to the order light
    // CONDITION: must be a delivery vehicle with a OrderLight attached to a child
    private void PickOrderColors()
    {
        List<GameObject> order = new List<GameObject>();
        for (int i = 0; i < myTruck.maxObjects; i++)
        {
            int whichBox = Mathf.FloorToInt(UnityEngine.Random.Range(0, boxTypes.Count));
            order.Add(boxTypes[whichBox]);
        }
        displayLights.SetLights(order);

    }

    // COMPARE CARGO
    // compares current cargo to order request
    // if match, return true
    private bool CompareCargo()
    {
        return displayLights.CompareLights(myTruck.GetStoredObjects());
    }

    // END PICKUP
    // resets spawn counter, drive away
    private void EndPickup()
    {
        spawnWait = true;
        SetSpawnCounter();
        if (CompareCargo())
        {
            audSrc.PlayOneShot(successSFX, 1f);
            audSrc.PlayOneShot(clappingSFX);
        }
        DriveTo(driveAwayDistance);
        counter.text = "-";
        displayLights.Reset();
        audSrc.PlayOneShot(leaveSFX);
    }

    // SWITCH TYPES
    // change the type of truck using a weighted chance
    private void SwitchTypes()
    {
        // delivery is 55%
        // pickup is 45%
        float[] prob = { 0.55f, 0.45f };
        // weighted choice will return 0 for most common, 1 for least out of two variables
        if (WeightedChoice(prob) <= 0)
            truckType = type.delivery;
        else
            truckType = type.pickup;

    }

    // WEIGHTEDCHOICE
    // Credit: unity manual
    // takes two float values, adds them tgether.
    // multiples value by unityengine random to get
    // a random value below the total (in case our %s dont add to 1
    // then it checks if that random number is less than the first element
    // (which should be highest chance item) if so, return index
    // else, you remove first element from random number, then check next element
    private float WeightedChoice(float[] prob)
    {
        float total = 0;

        foreach (float elem in prob)
            total += elem;

        float randomPoint = UnityEngine.Random.value * total;

        for (int i = 0; i < prob.Length; i++)
        {
            if (randomPoint < prob[i])
                return i;
            else
                randomPoint -= prob[i];
        }
        return prob.Length - 1;

    }
}
