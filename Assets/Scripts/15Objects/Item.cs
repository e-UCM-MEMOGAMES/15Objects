using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    /// <summary>
    /// Palabras correctas para nombrar el objeto (sin repeticiones)
    /// </summary> 
    HashSet<string> correctWords = new HashSet<string>();
    public HashSet<string> CorrectWords
    {
        get { return correctWords; }
        set { correctWords = value; }
    }


    /// <summary>
    /// Palabras de relleno incorrectas para este objeto (sin repeticiones)
    /// </summary> 
    HashSet<string> fillerWords = new HashSet<string>();
    public HashSet<string> FillerWords
    {
        get { return fillerWords; }
        set { fillerWords = value; }
    }
}
