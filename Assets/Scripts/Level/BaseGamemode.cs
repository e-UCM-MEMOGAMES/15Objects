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
    /// Lista de los objetos seleccionados
    /// </summary> 
    protected List<Item> selectedItems = new List<Item>();

    /// <summary>
    /// Lista con todas las posibles palabras de los objetos seleccionados
    /// </summary> 
    protected List<string> possibleWords = new List<string>();


    /// <summary>
    /// Se llama al pulsar sobre algun objeto en el nivel
    /// </summary>
    public virtual void OnItemSelected(Collider2D[] items)
    {
        if (items.Length > 0)
        {
            // Se borran las palabras posibles y los objetos seleccionados
            // que hubiera guardados anteriormente y se guardan los nuevos
            selectedItems.Clear();
            possibleWords.Clear();
            foreach (Collider2D col in items)
            {
                Item it = col.gameObject.GetComponent<Item>();
                selectedItems.Add(it);

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
    public virtual void SendAnswer(string text)
    {
        // Busca el primer objeto que tenga la palabra respondida
        // en su lista de palabras correctas y lo guarda
        bool found = false;
        GameObject item = null;
        for (int i = 0; i < selectedItems.Count() && !found; i++)
        {
            if (selectedItems[i].CorrectWords.Contains(text.ToLower()))
            {
                item = selectedItems[i].gameObject;
                found = true;
            }

        }

        // Si ha encontrado algun objeto, es que la respuesta es correcta
        if (found && item != null)
        {
            levelManager.CorrectAnswer(item, text);
        }
        // Si no, la respuesta es incorrecta
        else
        {
            levelManager.IncorrectAnswer(item, text);
        }

        HideElements();
    }

}
