using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class InputFieldHandler : MonoBehaviour
{

    public TMP_InputField InputField;  
    
    
    public void CheckInput(string s)
    {
        HackingTerminal.instance.CheckPhraseGuess(s);
        InputField.text = "";
    }
}
