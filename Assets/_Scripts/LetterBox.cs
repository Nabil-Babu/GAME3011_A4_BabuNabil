using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LetterBox : MonoBehaviour
{
    public TextMeshPro letter;
    public SpriteRenderer background;
    public int x, y;

    public string CurrentLetter
    {
        get
        {
            return letter.text; 
        }
        set
        {
            letter.text = value; 
        }
    }
    
    public void SetLetter(string nLetter)
    {
        if (nLetter == null)
        {
            Debug.Log("String is Null");
            CurrentLetter = "!";
        }
        else
        {
            CurrentLetter = nLetter.ToUpper(); 
        }
        
    }

    public string GetLetter()
    {
        return CurrentLetter;
    }

    public void SetGridPos(int nX, int nY)
    {
        x = nX;
        y = nY;
    }

    public void ChangeBackgroundColor(Color color)
    {
        background.color = color; 
    }
    
}
