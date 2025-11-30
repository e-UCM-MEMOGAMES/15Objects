using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GMTutorial : MonoBehaviour
{
    /// <summary>
    /// Panel que muestra el mensaje de error
    /// </summary> 
    public GameObject errorPanel;
    /// <summary>
    /// Texto que muestra la respuesta del jugador
    /// </summary> 
    public Text info;
    /// <summary>
    /// Texto de recordatorio de seleccionar un objeto
    /// </summary> 
    public Text noAnswer;
    /// <summary>
    /// Guarda el numero de veces en las que se ha hecho click en algo que no es un objeto
    /// </summary> 
    int contNoAnswer = 0;


    /// <summary>
    /// Imagen que indica la posicion en la que el jugador ha pinchado
    /// </summary> 
    public GameObject pointerPos;
    /// <summary>
    /// Game object que contiene el inputField
    /// </summary> 
    public InputField textBx;
    /// <summary>
    /// Array que contiene los paneles del tutorial
    /// </summary> 
    public GameObject[] tutorialPanels;
    /// <summary>
    /// Texto para el panel final
    /// </summary> 
    public Text points;
    /// <summary>
    /// Panel final
    /// </summary> 
    public GameObject finalPanel;

    /// <summary>
    /// Booleano que indica si ha terminado el tutorial o no
    /// </summary> 
    private bool tutorial = true;
    /// <summary>
    /// Contador de paneles mostrados del tutorial
    /// </summary> 
    private int contTutorial = 0;
    
    /// <summary>
    /// Indica si se ha terminado el juego
    /// </summary> 
    bool finished = false;

    void Start()
    {
        //GM.Instance.Level = "Tutorial";
        //LevelManager.Instance.initItems();
    }
    
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            contNoAnswer++;
            //Mientras hayan paneles se avanza el tutoria;
            if(contTutorial == tutorialPanels.Length - 2)
            {
                tutorialPanels[contTutorial].SetActive(false);
                contTutorial++;
                tutorialPanels[contTutorial].SetActive(false);
                contTutorial++;
            }
            if (tutorial) tutorialUpdate();
            //else gameUpdate();

        }

        //Si se hace click en algo que no es un objeto dos veces
        if(contNoAnswer == 2 && !finished)
        {
            noAnswer.gameObject.SetActive(true);
            contNoAnswer = 0;
        }

        //Si se ha respondido a todo
        //if (attempts == 5)
            //EndGame();

        
    }


    /// <summary>
    /// Este update será el que se ejecuta en el momento de tutorial para mostrar los paneles adecuados
    /// Funciona como el anterior.
    /// </summary> 
    void tutorialUpdate()
    {
        if(finished) return;
        bool error = true;
        if (contTutorial != 0)
        {
            Vector3 pointer = Input.mousePosition;
            Collider2D[] result = Physics2D.OverlapPointAll(Camera.main.ScreenToWorldPoint(pointer));
            int i = result.Length;
            if(i > 0)
            {
                contNoAnswer= 0;
            }
            while (i > 0)
            {
                pointerPos.SetActive(true);
                pointerPos.transform.position = pointer;
                i--;
                if (result[i].name == "Bottle")
                {
                    info.gameObject.SetActive(true);
                    error = false;
                    tutorialPanels[contTutorial].SetActive(false);
                    contTutorial++;
                    tutorialPanels[contTutorial].SetActive(true);

                    textBx.gameObject.SetActive(true);
                    textBx.Select();
                    textBx.ActivateInputField();

                    //int id;
                    //List<string> aux = result[i].GetComponent<LevelItem>().dameDic(out id);
                    //reverseDictionary.Add(id, result[i].name);
                    //if (!answered.ContainsValue(id))
                    //{
                    //    for (int w = 0; w < aux.Count; w++)
                    //    {
                    //        dictionary.Add(aux[w], id);
                    //    }
                    //}

                }
                
            }
            if (error)
                errorPanel.SetActive(true);
        }
        else
        {
            tutorialPanels[contTutorial].SetActive(false);
            contTutorial++;
            tutorialPanels[contTutorial].SetActive(true);
        }
    }


}
