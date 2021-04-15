using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class InfoPanelController : MonoBehaviour
{
    public GameObject letterBox;
    public int NumToReveal = 2;
    public TextMeshProUGUI timer;
    public TextMeshProUGUI FailedHack;
    public TextMeshProUGUI Status;
    public Color UnlockColor; 
    public Color LockColor; 
    public int CurrentTime
    {
        set
        {
            timer.text = value.ToString(); 
        }
    }
    
    
    [SerializeField] string[] _phrase;
    private Dictionary<string, List<LetterBox>> _hiddenPhrase = new Dictionary<string, List<LetterBox>>();


    private List<GameObject> allLetterBoxes = new List<GameObject>(); 
    
    
    public void SetPhrase(string[] phrase)
    {
        _phrase = phrase;
        GenerateLetterBoxes();
    }

    private void GenerateLetterBoxes()
    {
        float startX = transform.position.x;
        float startY = transform.position.y;
        
        for (int j = 0; j < _phrase.Length; j++)
        {
            string word = _phrase[j].Trim();
            _hiddenPhrase.Add(word, new List<LetterBox>());
            for (int i = 0; i < word.Length; i++)
            {
                
                GameObject addedTile = Instantiate(
                        letterBox, 
                        new Vector3(startX + (0.5f * i), startY + (0.5f * j), 0),
                        Quaternion.identity
                    );
                
                addedTile.transform.parent = transform;
                allLetterBoxes.Add(addedTile);
                LetterBox addedLetter = addedTile.GetComponent<LetterBox>();
                addedLetter.SetLetter("*");
                _hiddenPhrase[word].Add(addedLetter);
            }
        }
        
        RevealLetters(NumToReveal);
    }

    public void RevealLetters(int count)
    {
        foreach (var pair in _hiddenPhrase)
        {
            int counter = 0;
            while (counter < count)
            {
                int letterIndex = UnityEngine.Random.Range(0, pair.Value.Count);
                if (pair.Value[letterIndex].CurrentLetter == "*")
                {
                    pair.Value[letterIndex].CurrentLetter = pair.Key[letterIndex].ToString();
                    counter++;
                }
            }
        }
    }

    public void RevealWord(string word)
    {
        if (_hiddenPhrase.ContainsKey(word))
        {
            for (int i = 0; i < _hiddenPhrase[word].Count; i++)
            {
                _hiddenPhrase[word][i].SetLetter(word[i].ToString());
            }
        }
    }

    public void UnlockTerminal()
    {
        Status.text = "UNLOCKED";
        Status.color = UnlockColor;
    }

    public void ResetInfoPanel()
    {
        _hiddenPhrase.Clear();
        FailedHack.gameObject.SetActive(false);
        Status.text = "LOCKED";
        Status.color = LockColor;
        foreach (var letterBox in allLetterBoxes)
        {
            Destroy(letterBox);
        }
        allLetterBoxes.Clear();
        
    }
}
