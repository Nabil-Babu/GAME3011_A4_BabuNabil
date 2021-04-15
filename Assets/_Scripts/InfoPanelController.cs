using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class InfoPanelController : MonoBehaviour
{
    public GameObject letterBox;
    public int NumToReveal = 2;
    public TextMeshProUGUI timer;

    public int CurrentTime
    {
        set
        {
            timer.text = value.ToString(); 
        }
    }
    
    
    [SerializeField] string[] _phrase;
    private Dictionary<string, List<LetterBox>> _hiddenPhrase = new Dictionary<string, List<LetterBox>>(); 
    
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
}
