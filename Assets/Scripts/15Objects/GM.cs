using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;
using System.IO;
using System.Text;
using UnityEngine.SceneManagement;
using TMPro;
using Xasu.HighLevel;
using Button = UnityEngine.UI.Button;
using Xasu;

public class GM : MonoBehaviour {
    /// <summary>
    /// Texto que indica el objeto que se ha contestado
    /// </summary> 
    public Text feedbackResponse;
    /// <summary>
    /// Textp que indica que se ha dado un respeusta incorrecta
    /// </summary> 
    public GameObject incorrectText;
    /// <summary>
    /// Estado de juego
    /// </summary> 
    private GameState15O gameS;
    /// <summary>
    /// Array que tiene los objetos sobre los que hace click el jugador
    /// </summary> 
    private Collider2D[] selected;
    /// <summary>
    /// Color en el que se muestra el objeto cuando el jugador da una respuesta correcta
    /// </summary> 
    public Color correctColor;
    /// <summary>
    /// Color normal de los objetos
    /// </summary> 
    public Color normalColor;

    /// <summary>
    /// Nivel que ha seleccionado el jugador
    /// </summary> 
    private string level = "A";
    public GameObject lista;
    public Text listaText;
    public Text cont;
    private bool hayLista = false, hayCont = false;

    /// <summary>
    /// Texto que muestra los puntos que ha conseguido el jugador
    /// </summary> 
    public Text points;
    /// <summary>
    /// Panel final;
    /// </summary> 
    public GameObject finalPanel;
    /// <summary>
    /// Game object que contiene el inputField
    /// </summary> 
    public InputField textBx;
    /// <summary>
    /// Objetos del juego
    /// </summary> 
    public GameObject A, B;

    /// <summary>
    /// Diccionario que contendrá las palabras y sinónimos de los objetos seleccionados.
    /// </summary> 
    private Dictionary<string, int> diccionary;
    /// <summary>
    /// Diccionario que contiene las palabras que se han respondido.
    /// </summary> 
    private Dictionary<string, int> answered;
    /// <summary>
    /// Diccionario que contiene las palabras que se han respondido en su version simplificada.
    /// </summary> 
    private Dictionary<string, int> simpleDictionary;
    /// <summary>
    /// Diccionario que contiene las IDs con sus palabras correspondientes
    /// </summary> 
    private Dictionary<int, string> reverseDictionary;

    /// <summary>
    /// Panel para seleccionar nivel
    /// </summary> 
    private GameObject levelSelectorPanel;
    /// <summary>
    /// Panel para seleccionar modo de juego
    /// </summary> 
    private GameObject gamemodePanel;
    /// <summary>
    /// Entero que controla el número de intentos.
    /// </summary> 
    private int attempts = 0;                                       
    /// <summary>
    /// Indica el numero total de intentos para terminar el juego
    /// </summary> 
    private readonly int totalAttempts = 15;
    /// <summary>
    /// Entero que controla el número de errores del usuario.
    /// </summary> 
    private int mistakes = 0;                                       
    /// <summary>
    /// Imagen que indica la posicion en la que el jugador ha pinchado
    /// </summary> 
    public GameObject pointerPos;
    /// <summary>
    /// Indica el modo de juego seleccionado
    /// </summary> 
    private int gamemode;
    /// <summary>
    /// Array que tiene los botones para el modo de juego de seleccionar
    /// </summary> 
    public GameObject[] selectorOptions;
    /// <summary>
    /// Botones para navegar las opciones cuando hay demasiadas
    /// </summary> 
    public GameObject leftButton;
    public GameObject rightButton;
    /// <summary>
    /// Array con los textos del juego
    /// </summary> 
    public string[] notifications;
    /// <summary>
    /// Lista con los objetos seleccionados
    /// </summary> 
    private List<string> selectedList;

    private int selectedIndex = 0;
    private int selectedPageIndex = 0;

    FileStream fs;

    public static GM Instance { get; private set; }

    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    void Start () {
        correctColor = new Color(0, 255, 0);
        normalColor = new Color(255, 255, 255);
        diccionary = new Dictionary<string, int>();
        answered = new Dictionary<string, int>();
        simpleDictionary = new Dictionary<string, int>();
        reverseDictionary = new Dictionary<int, string>();
        selectedList = new List<string>();
    }
	
	void Update () {
        //si se hace click y estamos jugando
        if (Input.GetMouseButtonDown(0) && (A.activeSelf || B.activeSelf))
        {
            //Se comprueba si en el punto del mouse al hacer click hay colisión con algún objeto. Se devuelven todos los objetos en result.
            Vector3 inputPos = Input.mousePosition;
            selected = Physics2D.OverlapPointAll(Camera.main.ScreenToWorldPoint(inputPos));

            if (selected.Length > 0)
            {
                simpleDictionary.Clear();
                diccionary.Clear();
                reverseDictionary.Clear();
                if(gamemode == 1)
                {
                    textBx.gameObject.SetActive(true);
                    textBx.Select();
                    textBx.ActivateInputField();
                }
                else
                {
                    selectedIndex = 0;
                    selectedPageIndex = 0;
                    selectedList.Clear();
                }
            }

            string log = "Se ha pinchado en: ";

            foreach(Collider2D c in selected)
            {
                int id;
                log += c.name + " ";
                Debug.Log("manpinchao " + c.name);
                List<string> aux = c.GetComponent<Objeto>().dameDic(out id);       //El método dameDic devuelve una vector de palabras y un identificador que nos servirá para comprobar si se había respondido ya esa palabra.
                List<string> aux2 = c.GetComponent<Objeto>().dameFill();       //El método dameDic devuelve una vector de palabras y un identificador que nos servirá para comprobar si se había respondido ya esa palabra.
                reverseDictionary.Add(id, c.name);
                simpleDictionary.Add(c.name, id);
                if (!answered.ContainsValue(id))                                        //Si no se había respondido ya añadimos las palabras de cada objeto al diccionario.
                {
                    for (int w = 0; w < aux.Count; w++)
                    {
                        diccionary.Add(aux[w], id);
                        selectedList.Add(aux[w]);
                    }
                    foreach(string f in aux2)
                    {
                        selectedList.Add(f);
                    }
                }
            }

            if(selected.Length > 0 && gamemode == 0)
            {
                RandomizeList(selectedList);
                ShowOptions();
            }
            log += "\n";
            Byte[] info = new UTF8Encoding(true).GetBytes(log);
            if (selected.Length > 0) fs.Write(info, 0, info.Length);
        }

        if (attempts == totalAttempts)
            EndGame();
        
    }

    /// <summary>
    /// Termina la partida
    /// </summary> 
    public void EndGame() { }

    //Este método es llamado cada vez que se pulsa enter en el inputField y recibe de parámetro la palabra introducida.
    public void OnFieldEnter(string word)
    {
        string log;
        if (diccionary.ContainsKey(word.ToLower()))
        //Si la palabra se encuentra en el diccionario la añadimos al diccionario de respondidos
        {
            int value;
            diccionary.TryGetValue(word.ToLower(), out value);
            answered.Add(word, value);
            log = "\t✔ Ha respondido correctamente con: " + word;
            AudioManager.Instance.Play(GameSound.Success);

            feedbackResponse.text = notifications[0] + word;
            feedbackResponse.gameObject.SetActive(true);
            string name;
            reverseDictionary.TryGetValue(value, out name);
            //Feedback de respuesta
            ChangeColor(name, correctColor);
            StartCoroutine(ChangeColor(name, normalColor, 2f));
            selected = null;
            // Tracking
            Dictionary<string, bool> simpleVarDictionary = new Dictionary<string, bool>();
            foreach (KeyValuePair<string, int> attachStat in simpleDictionary)
            {
                int simpleValue;
                simpleDictionary.TryGetValue(attachStat.Key, out simpleValue);
                if (simpleValue == value)
                {
                    simpleVarDictionary.Add(attachStat.Key, true);
                }
                else
                {
                    simpleVarDictionary.Add(attachStat.Key, false);
                }
            }
            Dictionary<string, object> extensions = new Dictionary<string, object>();
            if (simpleVarDictionary != null)
                extensions.Add("https://" + "targets", simpleVarDictionary);

            foreach (KeyValuePair<string, int> attachStat in diccionary)
            {
                if (attachStat.Key != null)
                    extensions.Add("https://" + attachStat.Key, attachStat.Value);
            }
            // No hubo cambio de objeto
            extensions.Add("https://" + "object-changed", 0);
            // Respuesta correcta
            extensions.Add("https://" + "correct", 1);
            if (XasuTracker.Instance.Status.State != TrackerState.Uninitialized)
                AlternativeTracker.Instance.Selected(level, word).WithSuccess(true).WithResultExtensions(extensions);
        }
        else if (word != "")
        {
            mistakes++;
            Debug.Log("Fallaste");
            if (answered.ContainsKey(word.ToLower())) log = "\t✘ Ha respondido una palabra repetida: " + word;
            else log = "\t✘ Ha respondido con error: " + word;
            feedbackResponse.text = notifications[0] + word;
            feedbackResponse.gameObject.SetActive(true);
            IncorrectFeedback();
            AudioManager.Instance.Play(GameSound.Failed);
            Invoke("IncorrectFeedback", 1.5f);

            // Tracking
            Dictionary<string, object> extensions = new Dictionary<string, object>();
            Dictionary<String, bool> simpleVarDictionary = new Dictionary<string, bool>();
            foreach (KeyValuePair<string, int> attachStat in simpleDictionary)
            {
                simpleVarDictionary.Add(attachStat.Key, false);

                // Mappings por si hacen falta en el analysis
                String varKey = "mappings_" + attachStat.Key;
                String varValue = " ";
                foreach (KeyValuePair<string, int> dicKeyValues in diccionary)
                {
                    if (dicKeyValues.Value == attachStat.Value)
                    {
                        varValue += dicKeyValues.Key + ",";
                    }
                }
                if (varValue.EndsWith(","))
                {
                    varValue = varValue.Substring(0, varValue.Length - 1);
                }
                if (varKey != null && varValue != null)
                    extensions.Add("https://" + varKey, varValue);
            }
            if (simpleVarDictionary != null)
                extensions.Add("https://" + "targets", simpleVarDictionary);

            foreach (KeyValuePair<string, int> attachStat in diccionary)
            {
                if (attachStat.Key != null)
                    extensions.Add("https://" + attachStat.Key, attachStat.Value);
            }
            // No hubo cambio de objeto
            extensions.Add("https://" + "object-changed", 0);
            // Respuesta incorrecta
            extensions.Add("https://" + "correct", 0);
            if (XasuTracker.Instance.Status.State != TrackerState.Uninitialized)
                AlternativeTracker.Instance.Selected(level, word).WithSuccess(false).WithResultExtensions(extensions);
        }
        else
        {
            log = "\tHa cambiado de objeto";

            // Tracking object changed without answer
            Dictionary<string, object> extensions = new Dictionary<string, object>();
            Dictionary<string, bool> simpleVarDictionary = new Dictionary<string, bool>();
            foreach (KeyValuePair<string, int> attachStat in simpleDictionary)
            {
                simpleVarDictionary.Add(attachStat.Key, false);

                // Mappings por si hacen falta en el analysis
                String varKey = "mappings_" + attachStat.Key;
                String varValue = " ";
                foreach (KeyValuePair<string, int> dicKeyValues in diccionary)
                {
                    if (dicKeyValues.Value == attachStat.Value)
                    {
                        varValue += dicKeyValues.Key + ",";
                    }
                }
                if (varValue.EndsWith(","))
                {
                    varValue = varValue.Substring(0, varValue.Length - 1);
                }
                if (varKey != null && varValue != null)
                    extensions.Add("https://" + varKey, varValue);
            }
            if (simpleVarDictionary != null)
                extensions.Add("https://" + "targets", simpleVarDictionary);

            foreach (KeyValuePair<string, int> attachStat in diccionary)
            {
                if (attachStat.Key != null)
                    extensions.Add("https://" + attachStat.Key, attachStat.Value);
            }
            // Hubo cambio de objeto
            extensions.Add("https://" + "object-changed", 1);
            // Respuesta desconocida
            extensions.Add("https://" + "correct", -1);
            if (XasuTracker.Instance.Status.State != TrackerState.Uninitialized)
                AlternativeTracker.Instance.Selected(level, "empty").WithSuccess(false).WithResultExtensions(extensions);
        }
        log += "\n";

        Byte[] info = new UTF8Encoding(true).GetBytes(log);
        fs.Write(info, 0, info.Length);

        diccionary.Clear();                                                    //Limpiamos el diccionario.
        simpleDictionary.Clear();
        reverseDictionary.Clear();
        textBx.gameObject.SetActive(false);
        pointerPos.SetActive(false);
        rightButton.SetActive(false);
        leftButton.SetActive(false);
        foreach (GameObject go in selectorOptions)
            go.SetActive(false);
        
        attempts++;

        if (hayCont) cont.text = notifications[0] + attempts.ToString() + notifications[1] + (totalAttempts - attempts).ToString();
        if (hayLista)
            listaText.text += "\n- " + word;

        textBx.Select();
        textBx.text = "";

        // Progreso del nivel actual
        float progress = (float)attempts / (float)totalAttempts;
        if (XasuTracker.Instance.Status.State != TrackerState.Uninitialized)
            CompletableTracker.Instance.Progressed(level, CompletableTracker.CompletableType.Level, progress);
    }

    /// <summary>
    /// Se llama cuando se selecciona un nivel
    /// </summary> 
    public void SetLevel(string level)
    {
        if (level == "A")  A.SetActive(true);
        else if (level == "B") B.SetActive(true);
        else {
            gameS.fileConfig = true;
            SceneManager.LoadScene("Tutorial15O");
        } 

        this.level = level;
        LevelManager.Instance.initItems();
        levelSelectorPanel.SetActive(false);

        // Started the 15 Objects level
        if (XasuTracker.Instance.Status.State != TrackerState.Uninitialized)
            CompletableTracker.Instance.Initialized(level, CompletableTracker.CompletableType.Level);
    }

    public void Limpiatexto (Text txt)
    {
        txt.text = "";
    }
    public void HayLista(bool hay)
    {
        hayLista = hay;
        SwActive(lista);
    }
    public void HayCont(bool hay)
    {
        hayCont = hay;
        cont.text = notifications[2];
        SwActive(cont.gameObject);
    }
    public void SwActive(GameObject ob)
    {
        ob.SetActive(!ob.activeInHierarchy);
    }

    /// <summary>
    /// Cambia el color del objeto pasado como parametro tras un delay
    /// </summary>
    private IEnumerator ChangeColor(string obj, Color color, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        GameObject c = GameObject.Find(char.ToUpper(obj[0]) + obj.Substring(1));
        SpriteRenderer sr;

        if(c != null)
        {
            sr = c.GetComponent<SpriteRenderer>();
            if (sr != null) sr.color = color;
        }
    }

    /// <summary>
    /// Cambia el color del objeto pasado como parametro
    /// </summary> 
    private void ChangeColor(string obj, Color color)
    {
        GameObject c = GameObject.Find(char.ToUpper(obj[0]) + obj.Substring(1));
        SpriteRenderer sr;

        if (c != null)
        {
            sr = c.GetComponent<SpriteRenderer>();
            if (sr != null) sr.color = color;
        }
    }

    /// <summary>
    /// Muestra el texto que indica que la respuesta es incorrecta
    /// </summary> 
    private void IncorrectFeedback()
    {
        incorrectText.SetActive(!incorrectText.activeSelf);
    }


    /// <summary>
    /// 
    /// </summary> Randomiza la lista que se pasa como parametro
    private void RandomizeList(List<string> l) {
        int n = l.Count;
        var rng = new System.Random();
        while (n > 1)
        {
            int k = rng.Next(n--);
            string temp = l[n];
            l[n] = l[k];
            l[k] = temp;
        }
    }

    /// <summary>
    /// Para cambiar las opciones en modo de juego para seleccionar
    /// </summary> 
    public void NavigateOptions(int p)
    {
        selectedPageIndex += p;
        ShowOptions();
    }

    /// <summary>
    /// Muestra las opciones en modo de juego para seleccionar
    /// </summary> 
    private void ShowOptions()
    {
        leftButton.SetActive(false);
        rightButton.SetActive(false);
        selectedIndex = 0;
        foreach(GameObject go in selectorOptions)
            go.SetActive(false);
        
        int ind;
        while (selectedIndex < selectorOptions.Length &&
            (selectedPageIndex * selectorOptions.Length) + selectedIndex < selectedList.Count)
        {
            GameObject go = selectorOptions[selectedIndex];
            TextMeshProUGUI t = go.GetComponentInChildren<TextMeshProUGUI>();
            go.SetActive(true);
            Button but = go.GetComponent<Button>();
            if (but != null)
            {
                ind = (selectedPageIndex * selectorOptions.Length) + selectedIndex;
                but.onClick.RemoveAllListeners();
                //NO BORRAR ESTE INT, EL LAMBDA NECESITA LA VARIABLE
                int tempInt = ind;
                but.onClick.AddListener(delegate { 
                    OnFieldEnter(selectedList[tempInt]);
                });

                if (t != null)
                    t.text = selectedList[ind];
            }
            selectedIndex++;
        }

        if (selectedIndex == selectorOptions.Length && 
            selectedList.Count - (selectedPageIndex * selectorOptions.Length + selectedIndex) > 0)
            rightButton.SetActive(true);

        if(selectedPageIndex > 0)
            leftButton.SetActive(true);
    }

    public string Level
    {
        get { return level; }
        set { level = value; }
    }
}
