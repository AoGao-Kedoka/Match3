using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndScreenScoreHandler : MonoBehaviour
{
    [SerializeField] 
    private Text _text;

    private int score;

    private void Start()
    {
        score = PlayerPrefs.GetInt("Score");
        _text.text = "Score: " + score;
    }
}
