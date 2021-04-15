using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEditorInternal;
using UnityEngine;
using Random = UnityEngine.Random;

public enum Difficulty
{
    EASY,
    MEDIUM, 
    HARD
}

public enum PlayerSkill
{
    AMATEUR = 0,
    NOVICE = 1,
    APPRENTICE = 2, 
    EXPERT = 3,
    MASTER = 4
}

public class HackingTerminal : Singleton<HackingTerminal>
{
    public GameObject Tile;
    public InfoPanelController InfoPanelController; 
    public int gridX, gridY;
    public float spacing = 0.5f;

    public Vector3 easyPosition;
    public Vector3 medPosition;
    public Vector3 hardPosition;
    
    
    public TextAsset wordPool4Letters;
    public TextAsset wordPool5Letters;
    public TextAsset wordPool6Letters;
    
    public Difficulty HackDifficulty;
    public PlayerSkill PlayerSkill;
    
    private List<string> wordSet = new List<string>();
    private string[] letters = new string[26]
        {"!", "b", "c", "d", "+", "f", "g", "h", "~", "j", "k", "l", "m", "n", "?", "p", "q", "r", "s", "t", "=", "v", "w", "x", "y", "z"};
    private Dictionary<string, bool> phrase = new Dictionary<string, bool>();
    private int phraseLimit = 3;
    private string[,] _stringMatrix;

    private GameObject[,] _tileGOMatrix; 
    
    private float timer = 60;

    private int correctPasswords;
    [SerializeField] bool isPlaying = true; 

    private void Start()
    {
        SetDifficulty();
        
        _stringMatrix = new string[gridX, gridY];
        _tileGOMatrix = new GameObject[gridX, gridY];

        PickPhrase();
        InsertPhrase();
        FillMatrix();
        GenerateGrid();
    }

    private void Update()
    {
        CountDown();
    }

    public void GenerateGrid()
    {

        float startX = transform.position.x;
        float startY = transform.position.y;
        
        
        for (int i = 0; i < gridX; i++)
        {
            for (int j = 0; j < gridY; j++)
            {
                GameObject addedTile = Instantiate(Tile, new Vector3(startX + (spacing * i), startY + (spacing * j), 0),
                    Quaternion.identity);
                addedTile.transform.parent = transform;
                _tileGOMatrix[i, j] = addedTile;
                LetterBox addedLetter = addedTile.GetComponent<LetterBox>();
                // Instead of random letters replace this with grabbing letter from the string Matrix
                addedLetter.SetLetter(_stringMatrix[i,j]);
                addedLetter.SetGridPos(i, j);
            }
        }
    }

    public void PickPhrase()
    {
        List<string> availableWords = new List<string>();
        availableWords.AddRange(wordSet);
        int count = 0;

        while (count < phraseLimit)
        {
            var pickedWord = availableWords[UnityEngine.Random.Range(0, availableWords.Count)];
            if (!phrase.ContainsKey(pickedWord))
            {
                availableWords.Remove(pickedWord);
                phrase.Add(pickedWord.Trim(), false);
                count++; 
            }
        }
        
        InfoPanelController.SetPhrase(phrase.Keys.ToArray());
        
        Debug.Log("Key Phrase: ");
        foreach (var word in phrase)
        {
            Debug.Log(word.Key);
        }
    }

    public void SetDifficulty()
    {
        switch (HackDifficulty)
        {
            case Difficulty.EASY:
                gridX = gridY = 8;
                wordSet.AddRange(wordPool4Letters.text.Split('\n'));
                transform.position = easyPosition;
                timer = 60;
                break;
            case Difficulty.MEDIUM:
                gridX = gridY = 12;
                wordSet.AddRange(wordPool5Letters.text.Split('\n'));
                transform.position = medPosition;
                timer = 120;
                break;
            case Difficulty.HARD:
                gridX = gridY = 15;
                wordSet.AddRange(wordPool6Letters.text.Split('\n'));
                transform.position = hardPosition;
                timer = 240;
                break;
        }
        
        InfoPanelController.NumToReveal = (int)PlayerSkill;
    }

    public void InsertPhrase()
    {
        int mark = 0;
        foreach (var pair in phrase)
        {
            string word = pair.Key.Trim();
            bool placed = false;
            while (!placed)
            {
                int x = UnityEngine.Random.Range(0, gridX);
                int y = UnityEngine.Random.Range(0, gridY);
                placed = InsertWord(word, x, y);
                mark++;
                if (mark > 100) {
                    break;
                }
            }
        }
    }

    bool InsertWord(string word, int xPos, int yPos)
    {

        if (xPos + word.Length >= gridX)
        {
            return false; 
        }

        for (int i = 0; i < word.Length; i++)
        {
            if (!string.IsNullOrEmpty(_stringMatrix[xPos + i, yPos]))
            {
                return false;
            }
        }

        for (int i = 0; i < word.Length; i++)
        {
            _stringMatrix[xPos + i, yPos] = word[i].ToString(); 
        }
        
        return true; 
    }

    public void FillMatrix()
    {
        for (int i = 0; i < gridX; i++) {
            for (int j = 0; j < gridY; j++) {
                if (string.IsNullOrEmpty(_stringMatrix[i, j])) {
                    _stringMatrix[i, j] = letters[UnityEngine.Random.Range(0, letters.Length)];
                }
            }
        }
    }

    public void CheckPhraseGuess(string guess)
    {
        if (!isPlaying) return;
        string s = guess.ToLower().Trim();
        if (phrase.ContainsKey(s))
        {
            if (!phrase[s])
            {
                phrase[s] = true;
                InfoPanelController.RevealWord(s);
                correctPasswords++;
                if (correctPasswords == phraseLimit)
                {
                    isPlaying = false;
                    InfoPanelController.UnlockTerminal();
                }
            }
        }
    }

    void CountDown()
    {
        if (!isPlaying) return; 
        
        timer -= Time.deltaTime;
        int roundedTime = (int) timer;
        if (timer <= 0)
        {
            isPlaying = false; 
            roundedTime = 0;
            InfoPanelController.FailedHack.gameObject.SetActive(true);
        }
        InfoPanelController.CurrentTime = roundedTime; 
    }

    void ResetGrid()
    {
        for (int i = 0; i < gridX; i++)
        {
            for (int j = 0; j < gridY; j++)
            {
                _tileGOMatrix[i,j].GetComponent<LetterBox>().SetLetter(_stringMatrix[i,j]);
                _tileGOMatrix[i,j].GetComponent<LetterBox>().SetGridPos(i, j);
            }
        }
    }

    public void ResetTerminal()
    {
        isPlaying = true;
        correctPasswords = 0;
        wordSet.Clear();
        phrase.Clear();
        timer = 0;
        InfoPanelController.ResetInfoPanel();

        SetDifficulty();
        _stringMatrix = new string[gridX, gridY];
        PickPhrase();
        InsertPhrase();
        FillMatrix();
        ResetGrid();
    }
}
