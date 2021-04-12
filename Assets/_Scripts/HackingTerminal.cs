using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Difficulty
{
    EASY,
    MEDIUM, 
    HARD
}

public class HackingTerminal : Singleton<HackingTerminal>
{
    public GameObject Tile;
    public int gridX, gridY;
    public float spacing = 0.5f;
    
    public TextAsset wordPool4Letters;
    public TextAsset wordPool5Letters;
    public TextAsset wordPool6Letters;
    
    public Difficulty HackDifficulty;
    
    private List<string> wordSet = new List<string>();
    private string[] letters = new string[26]
        {"a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z"};
    private Dictionary<string, bool> phrase = new Dictionary<string, bool>();
    private int phraseLimit = 3;
    private string[,] _stringMatrix;
    private GameObject[,] _gameObjectMatrix; 

    private void Start()
    {
        
        SetDifficulty();
        PickPhrase();
        
        // Insert Words from phrase into Matrix
        // Fill Rest of Matrix with Random Characters
        
        GenerateGrid();
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
                LetterBox addedLetter = addedTile.GetComponent<LetterBox>();
                addedLetter.SetLetter(letters[UnityEngine.Random.Range(0, letters.Length)]);
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
                phrase.Add(pickedWord, false);
                count++; 
            }
        }
        // Debug.Log("Key Phrase: ");
        // foreach (var word in phrase)
        // {
        //     Debug.Log(word.Key);
        // }
    }

    public void SetDifficulty()
    {
        switch (HackDifficulty)
        {
            case Difficulty.EASY:
                gridX = gridY = 8;
                wordSet.AddRange(wordPool4Letters.text.Split('\n'));
                break;
            case Difficulty.MEDIUM:
                gridX = gridY = 12;
                wordSet.AddRange(wordPool5Letters.text.Split('\n'));
                break;
            case Difficulty.HARD:
                gridX = gridY = 15;
                wordSet.AddRange(wordPool6Letters.text.Split('\n'));
                break;
        }
        
        _stringMatrix = new string[gridX, gridY];
        _gameObjectMatrix = new GameObject[gridX, gridY];
    }
}
