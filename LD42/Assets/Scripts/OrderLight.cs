using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderLight : MonoBehaviour {

    [SerializeField] private List<GameObject> lights;
    [SerializeField] private int maxLength;


    public List<GameObject> GetLightList()
    {
        return lights;
    }


    // SET LIGHTS
    // given a list of gameobjects CONDITION: box prefabs
    // set this objects lights based on the color, in order
    // the list
    public void SetLights(List<GameObject> nLights)
    {
        int i = 0;
        foreach (GameObject light in nLights)
        {
            switch (light.name)
            {
                case "RedBox":
                    SetEmissionColor(lights[i], Color.red);
                    lights[i].name = "RedBox(Clone)";
                    break;
                case "GreenBox":
                    SetEmissionColor(lights[i], Color.green);
                    lights[i].name = "GreenBox(Clone)";
                    break;
                case "BlueBox":
                    SetEmissionColor(lights[i], Color.blue);
                    lights[i].name = "BlueBox(Clone)";
                    break;
            }
            i++;
        }
    }

    // COMPARE LIGHTS
    // if not enough objects, return false
    // if each object matches in order, return true
    public bool CompareLights(List<GameObject> nLights)
    {
        if (nLights.Count < maxLength)
            return false;
        for (int i = 0; i < nLights.Count; i++)
        {
            if (nLights[i].name != lights[i].name)
                return false;
        }
        return true;
    }

    public void Reset()
    {
        foreach(GameObject light in lights)
        {
            SetEmissionColor(light, Color.gray);
        }
    }

    private void SetEmissionColor(GameObject nlight, Color color)
    {
        nlight.GetComponent<MeshRenderer>().material.color = color;
    }
}
