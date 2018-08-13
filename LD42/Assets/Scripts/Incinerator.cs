using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Incinerator : MonoBehaviour {

    [SerializeField] private int incineratePoints = 1;
    [SerializeField] private GameObject floatTextPrefab;
    [SerializeField] private float floatTextOffset = 1.5f;

    private ScoreBoard score;
    private GameObject nText;

    private void Start()
    {
        score = GameObject.FindGameObjectWithTag("Score").GetComponent<ScoreBoard>();
        CreateFloatText();
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
            DisplayFloatText(incineratePoints);
        }
    }



    private void CreateFloatText()
    {
        Vector3 textLocation = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * floatTextOffset);
        nText = Instantiate(floatTextPrefab, textLocation, Quaternion.identity, GameObject.FindGameObjectWithTag("Canvas").transform);
        nText.SetActive(false);
    }

    private void DisplayFloatText(int points)
    {
        nText.GetComponentInChildren<TextMeshProUGUI>().text = "+" + points.ToString();
        nText.SetActive(true);
    }
}
