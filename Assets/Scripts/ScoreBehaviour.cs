using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreBehaviour : MonoBehaviour
{
    [SerializeField] 
    private Text _text;

    [SerializeField]
    private Text _combo;

    public void updateText(int score, int movementLeft, int combo)
    {
        _text.text = "Movement left: " + movementLeft + "\nScore: " + score;
        if (combo < 3)
        {
            _combo.text = "Scores x1";
        }
        else
        {
            _combo.text = "Combo!\n Scores x2!!";
        }
    }
}
