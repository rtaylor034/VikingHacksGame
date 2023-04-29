using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class SustainDisplay : MonoBehaviour
{
    public TMP_Text SustainText;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        SustainText.text = GameManager.GAME.Sustain.ToString();
    }
}
