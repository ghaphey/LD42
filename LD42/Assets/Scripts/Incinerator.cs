using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Incinerator : MonoBehaviour {

    [SerializeField] int incineratePoints = 1;
    private ScoreBoard score;

    private void Start()
    {
        score = GameObject.FindGameObjectWithTag("Score").GetComponent<ScoreBoard>();
    }

    // ON TRIGGER ENTER
    // Destroy a box when it touches the incinerator
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Box")
        {
            print("destroying " + other.name);
            Destroy(other.gameObject);
            score.AddIncineratorScore(incineratePoints);
        }
    }
}
