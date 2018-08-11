using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderLight : MonoBehaviour {

    [SerializeField] private List<GameObject> lights;


    public List<GameObject> GetLightList()
    {
        return lights;
    }

    public void SetLights(List<GameObject> nLights)
    {
        int i = 0;
        foreach (GameObject light in nLights)
        {
            switch (light.name)
            {
                case "RedBox":
                    SetEmissionColor(lights[i], Color.red);
                    break;
                case "GreenBox":
                    SetEmissionColor(lights[i], Color.green);
                    break;
                case "BlueBox":
                    SetEmissionColor(lights[i], Color.blue);
                    break;
            }
            i++;
        }
    }

    private void SetEmissionColor(GameObject nlight, Color color)
    {
        nlight.GetComponent<MeshRenderer>().material.color = color;
    }
}
