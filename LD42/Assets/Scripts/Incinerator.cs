using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Incinerator : MonoBehaviour {

    [SerializeField] private int incineratePoints = 1;
    [SerializeField] private float incinerateDelay = 2.0f;
    [SerializeField] private GameObject floatTextPrefab;
    [SerializeField] private float floatTextOffset = 1.5f;
    [SerializeField] private GameObject boxFireFX;

    private ScoreBoard score;
    private GameObject nText;

    private void Start()
    {
        score = GameObject.FindGameObjectWithTag("Score").GetComponent<ScoreBoard>();
        CreateFloatText();
    }
    // ON TRIGGER ENTER
    // Destroy a box when it touches the incinerator
    // also create box fix effect & update score
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Box")
        {
            Instantiate(boxFireFX, other.transform.position, Quaternion.identity, other.transform);
            Destroy(other.gameObject, incinerateDelay);
            score.AddIncineratorScore(incineratePoints);
            DisplayFloatText(incineratePoints);
        }
    }

    // CREATE FLOAT TEXT
    // create a floating text at offset, so we have one instead of 
    // needless re-instantiation throughout
    private void CreateFloatText()
    {
        Vector3 textLocation = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * floatTextOffset);
        nText = Instantiate(floatTextPrefab, textLocation, Quaternion.identity, GameObject.FindGameObjectWithTag("Canvas").transform);
        nText.SetActive(false);
    }

    // DISPLAY FLOAT TEXT
    // Sets the floating text as active to begin animation and sets its value
    private void DisplayFloatText(int points)
    {
        nText.GetComponentInChildren<TextMeshProUGUI>().text = "+" + points.ToString();
        nText.SetActive(true);
    }
}
