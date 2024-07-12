using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[RequireComponent(typeof(Collider2D))]
public class Objeto : MonoBehaviour
{
    /// <summary>
    /// Lista de palabras a las que se puede llamar este objeto
    /// </summary> 
    List<string> words;
    /// <summary>
    /// Lista de palabras incorrectas
    /// </summary> 
    List <string> fillers;

    /// <summary>
    /// Inicializa las listas
    /// </summary> 
    public void InitLists()
    {
        words = new List<string>();
        fillers = new List<string>();
    }

    /// <summary>
    /// Devuelve la lista de palabras correctas
    /// </summary> 
    public List<string> dameDic(out int id)
    {
        id = 0;
        for (int i = 0; i < this.name.Length; i++) id += (int)this.name[i];
        return words;
    }

    /// <summary>
    /// Devuelve la lista de palabras incorrectas
    /// </summary> 
    public List<string> dameFill()
    {
        return fillers;
    }

    /// <summary>
    /// Anade una palabra a la lista de palabras correctas
    /// </summary> 
    public void addWord(string word)
    {
        words.Add(word);
    }
    /// <summary>
    /// Anade una palabra a la lista de palabras incorrectas
    /// </summary> 
    public void addFiller(string word)
    {
        fillers.Add(word);
    }
}
