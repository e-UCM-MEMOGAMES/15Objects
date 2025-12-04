using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BaseGamemode : MonoBehaviour
{
    /// <summary>
    /// Instancia del LevelManager (setteado por el propio levelManager en su Start)
    /// </summary> 
    protected LevelManager levelManager;
    public LevelManager LevelManager
    {
        get { return levelManager; }
        set { levelManager = value; }
    }

    /// <summary>
    /// Lista con todas las posibles palabras de los objetos seleccionados
    /// </summary> 
    protected List<string> possibleWords = new List<string>();


    /// <summary>
    /// Se llama al pulsar sobre algun objeto en el nivel
    /// </summary>
    public virtual void OnItemSelected(List<Item> items)
    {
        if (items.Count > 0)
        {
            // Se borran las palabras posibles que hubiera guardadas anteriormente y se guardan las nuevas
            possibleWords.Clear();
            foreach (Item it in items)
            {
                possibleWords = (possibleWords.Concat(it.CorrectWords.ToList())).ToList();
                possibleWords = (possibleWords.Concat(it.FillerWords.ToList())).ToList();
            }
            ShowElements();
        }
    }


    /// <summary>
    /// Muestra los elementos especificos del modo de juego
    /// </summary> 
    public virtual void ShowElements() { }

    /// <summary>
    /// Oculta los elementos especificos del modo de juego
    /// </summary> 
    public virtual void HideElements() { }


    /// <summary>
    /// Se llama al responder el nombre del objeto
    /// </summary>
    public virtual void SendAnswer(string answer)
    {
        levelManager.Answer(answer);
        HideElements();
    }

}
