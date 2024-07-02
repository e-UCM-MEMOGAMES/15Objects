using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[RequireComponent(typeof(Collider2D))]
public class Objeto : MonoBehaviour
{
    List<string> words;
    List <string> fillers;

    public void InitLists()
    {
        words = new List<string>();
        fillers = new List<string>();
    }

    public List<string> dameDic(out int id)
    {
        id = 0;
        for (int i = 0; i < this.name.Length; i++) id += (int)this.name[i];
        return words;
    }

    public List<string> dameFill()
    {
        return fillers;
    }

    public void addWord(string word)
    {
        words.Add(word);
    }
    public void addFiller(string word)
    {
        fillers.Add(word);
    }
}
