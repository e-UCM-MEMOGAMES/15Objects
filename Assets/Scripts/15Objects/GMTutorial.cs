using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GMTutorial : MonoBehaviour
{
    public GameObject errorPanel;
    public Text info;
    public Text noAnswer;
    int contNoAnswer = 0;
    public Color correctColor;
    public Color normalColor;

    public GameObject pointerPos;
    public InputField textBx;                                       //Game object que contiene el inputField
    public GameObject[] tutorialPanels;                             //Array que contiene los paneles del tutorial
    public Text points;                                             //Texto para el panel final;
    public GameObject finalPanel;                                   //Panel final;

    private bool tutorial = true;                                   //Booleano que indica si ha terminado el tutorial o no
    private int contTutorial = 0;                                   //Contador de paneles mostrados del tutorial.

    private SortedDictionary<string, int> dictionary;               //Diccionario que contendrá las palabras y sinónimos de los objetos seleccionados.
    private SortedDictionary<string, int> answered;                 //Diccionario que contiene las palabras que se han respondido.
    private SortedDictionary<int, string> reverseDictionary;        //Diccionario que contiene las IDs con sus palabras correspondientes

    private int attempts=0;                                           //Entero que controla el número de intentos.
    private int mistakes = 0;                                       //Entero que controla el número de errores del usuario.

    void Start()
    {
        dictionary = new SortedDictionary<string, int>();
        answered = new SortedDictionary<string, int>();
        reverseDictionary = new SortedDictionary<int, string>();
        correctColor = new Color(0, 255, 0);
        normalColor = new Color(255, 255, 255);
        GM.Instance.Level = "Tutorial";
        LevelManager.Instance.initItems();
    }

   
    
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            contNoAnswer++;
            if(contTutorial == tutorialPanels.Length - 2)
            {
                tutorialPanels[contTutorial].SetActive(false);
                contTutorial++;
                tutorialPanels[contTutorial].SetActive(false);
                contTutorial++;
            }
            if (tutorial) tutorialUpdate();
            else gameUpdate();

        }

        if(contNoAnswer == 2)
        {
            noAnswer.gameObject.SetActive(true);
            contNoAnswer = 0;
        }

        if (attempts == 5)
        {
            info.text = "";
            finalPanel.SetActive(true);
            points.text = (attempts - mistakes).ToString() + "/5";
        }
    }

    #region Updates

    //Este es el update que ejecuta la lógica normal del juego
    void gameUpdate()
    {

        //Se comprueba si en el punto del mouse al hacer click hay colisión con algún objeto. Se devuelven todos los objetos en result.
        Vector3 pointer = Input.mousePosition;
        Collider2D[] result = Physics2D.OverlapPointAll(Camera.main.ScreenToWorldPoint(pointer));

        int i = result.Length;
        if (i > 0)
        {
            pointerPos.SetActive(true);
            pointerPos.transform.position = pointer;
            contNoAnswer = 0;
            dictionary.Clear();
            reverseDictionary.Clear();
            textBx.gameObject.SetActive(true);
            textBx.Select();
            textBx.ActivateInputField();
        }
        while (i > 0)
        {
            i--;
            int id;
            
            List<string> aux = result[i].GetComponent<Objeto>().dameDic(out id);       //El método dameDic devuelve una vector de palabras y un identificador que nos servirá para comprobar si se había respondido ya esa palabra.

            if(!reverseDictionary.ContainsKey(id))
                reverseDictionary.Add(id, result[i].name);

            if (!answered.ContainsValue(id) && aux != null)                                        //Si no se había respondido ya añadimos las palabras de cada objeto al diccionario.
            {
                for (int w = 0; w < aux.Count; w++)
                {
                    if (!dictionary.ContainsKey(aux[w]))
                        dictionary.Add(aux[w], id);
                }


            }

        }

    }

    //Este update será el que se ejecuta en el momento de tutorial para mostrar los paneles adecuados
    //Funciona como el anterior.
    void tutorialUpdate()
    {
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

                    int id;
                    List<string> aux = result[i].GetComponent<Objeto>().dameDic(out id);
                    reverseDictionary.Add(id, result[i].name);
                    if (!answered.ContainsValue(id))
                    {
                        for (int w = 0; w < aux.Count; w++)
                        {
                            dictionary.Add(aux[w], id);
                        }
                    }

                }
                
            }
            if (error)
            {
                
                errorPanel.SetActive(true);
                
            }
        }
        else
        {
            tutorialPanels[contTutorial].SetActive(false);
            contTutorial++;
            tutorialPanels[contTutorial].SetActive(true);
        }
    }
    #endregion Updates

    //Este método es llamado cada vez que se pulsa enter en el inputField y recibe de parámetro la palabra introducida.
    public void OnFieldEnter(string word)
    {

        if (dictionary.ContainsKey(word.ToLower()))                             //Si la palabra se encuentra en el diccionario la añadimos al diccionario de respondidos
        {
            int value = -1;
            dictionary.TryGetValue(word.ToLower(), out value);
            answered.Add(word, value);
            string name;
            reverseDictionary.TryGetValue(value, out name);
            ChangeColor(name, correctColor);
            StartCoroutine(ChangeColor(name, normalColor, 2f));

            Debug.Log("Acertaste");
        }
        else
        {
            mistakes++;
            Debug.Log("Fallaste");
        }

        if (tutorial)
        {
            tutorialPanels[contTutorial].SetActive(false);
            contTutorial++;
            tutorialPanels[contTutorial].SetActive(true);
            tutorialPanels[contTutorial+1].SetActive(true);
            tutorial = false;
        }

        pointerPos.SetActive(false);

        dictionary.Clear();                                                    //Limpiamod el diccionario.
        textBx.gameObject.SetActive(false);
        attempts++;

        textBx.Select();
        textBx.text = "";
    }

    private IEnumerator ChangeColor(string obj, Color color, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        ChangeColor(obj, color);
    }

    private void ChangeColor(string obj, Color color)
    {
        Debug.Log(obj);
        GameObject c = GameObject.Find(char.ToUpper(obj[0]) + obj.Substring(1).ToLower());
        Debug.Log(c);
        SpriteRenderer sr;
        if (c != null)
        {
            sr = c.gameObject.GetComponent<SpriteRenderer>();
            if (sr != null) sr.color = color;
        }
    }
}
