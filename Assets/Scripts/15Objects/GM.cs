using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;
using System.IO;
using System.Text;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UIElements;
using Xasu.HighLevel;
using Button = UnityEngine.UI.Button;

public class GM : MonoBehaviour {
    public Text feedbackResponse;
    public GameObject incorrectText;
    private GameState15O gameS;
    private Collider2D[] selected;
    public Color correctColor;
    public Color normalColor;

    //Lista y Contador
    private String level = "A";
    private bool isRandom = false;
    public GameObject lista;
    public Text listaText;
    public Text cont;
    private bool hayLista = false, hayCont = false;

    public Text points;                                             //Texto para el panel final;
    public GameObject finalPanel;                                   //Panel final;
    public InputField textBx;                                       //Game object que contiene el inputField
    public GameObject A, B;

    private Dictionary<string, int> diccionary;               //Diccionario que contendrá las palabras y sinónimos de los objetos seleccionados.
    private Dictionary<string, int> answered;                 //Diccionario que contiene las palabras que se han respondido.
    private Dictionary<string, int> simpleDictionary;         //Diccionario que contiene las palabras que se han respondido en su version simplificada.
    private Dictionary<int, string> reverseDictionary;        //Diccionario que contiene las IDs con sus palabras correspondientes

    private GameObject levelSelectorPanel;
    private GameObject gamemodePanel;
    private int attempts = 0;                                       //Entero que controla el número de intentos.
    private int totalAttempts = 15;                                 
    private int mistakes = 0;                                       //Entero que controla el número de errores del usuario.
    public GameObject pointerPos;
    private int gamemode;
    public GameObject[] selectorOptions;
    public GameObject leftButton;
    public GameObject rightButton;
    public string[] notifications;
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
        Initialize();

        string path;
        if (gameS.fileConfig)
        {
            path = @".\configFile15O.txt";
            if (!File.Exists(path))
            {
                // Note that no lock is put on the
                // file and the possibility exists
                // that another process could do
                // something with it between
                // the calls to Exists and Delete.
                fs = File.Create(path);
                Byte[] info = new UTF8Encoding(true).GetBytes("A");
                fs.Write(info, 0, info.Length);
                fs.Close();
            }
           
            StreamReader file = new StreamReader(path);
            string option = file.ReadLine();
            file.Close();
            //SetLevel(option);
            gameS.fileConfig = false;
        }

        path = @".\Resultados.txt";

        if (File.Exists(path))
        {
            // Note that no lock is put on the
            // file and the possibility exists
            // that another process could do
            // something with it between
            // the calls to Exists and Delete.
            File.Delete(path);
        }
        fs = File.Create(path);

    }
	
	// Update is called once per frame
	void Update () {

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
                pointerPos.SetActive(true);
                pointerPos.transform.position = inputPos;
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
        {
            gameS.fileConfig = false;
            finalPanel.SetActive(true);
            points.text = (attempts - mistakes).ToString() + "/" + totalAttempts;

            // Completed the 15 Objects level
            bool failed = (float)mistakes > ((float)totalAttempts / 2.0f);
            float score = 1.0f - ((float)mistakes / (float)totalAttempts);

            CompletableTracker.Instance.Completed(level, CompletableTracker.CompletableType.Level);
            //Tracker.T.Completable.Completed(level, CompletableTracker.Completable.Level, !failed, score);
        }
    }


    //Este método es llamado cada vez que se pulsa enter en el inputField y recibe de parámetro la palabra introducida.
    public void OnFieldEnter(string word)
    {
        string log = "";
        if (diccionary.ContainsKey(word.ToLower()))
        //Si la palabra se encuentra en el diccionario la añadimos al diccionario de respondidos
        {
            int value = -1;
            diccionary.TryGetValue(word.ToLower(), out value);
            answered.Add(word, value);
            log = "\t✔ Ha respondido correctamente con: " + word;
            feedbackResponse.text = notifications[0] + word;
            feedbackResponse.gameObject.SetActive(true);
            string name;
            reverseDictionary.TryGetValue(value, out name);
            //Feedback de respuesta
            ChangeColor(name, correctColor);
            StartCoroutine(ChangeColor(name, normalColor, 2f));
            selected = null;
            // Tracking
            Dictionary<String, bool> simpleVarDictionary = new Dictionary<string, bool>();
            foreach (KeyValuePair<string, int> attachStat in simpleDictionary)
            {
                int simpleValue = -1;
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
                extensions.Add(Application.identifier + "://" + "targets", simpleVarDictionary);

            foreach (KeyValuePair<string, int> attachStat in diccionary)
            {
                if (attachStat.Key != null)
                    extensions.Add(Application.identifier + "://" + attachStat.Key, attachStat.Value);
            }
            // No hubo cambio de objeto
            extensions.Add(Application.identifier + "://" + "object-changed", 0);
            // Respuesta correcta
            extensions.Add(Application.identifier + "://" + "correct", 1);
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
                    extensions.Add(Application.identifier + "://" + varKey, varValue);
            }
            if (simpleVarDictionary != null)
                extensions.Add(Application.identifier + "://" + "targets", simpleVarDictionary);

            foreach (KeyValuePair<string, int> attachStat in diccionary)
            {
                if (attachStat.Key != null)
                    extensions.Add(Application.identifier + "://" + attachStat.Key, attachStat.Value);
            }
            // No hubo cambio de objeto
            extensions.Add(Application.identifier + "://" + "object-changed", 0);
            // Respuesta incorrecta
            extensions.Add(Application.identifier + "://" + "correct", 0);
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
                    extensions.Add(Application.identifier + "://" + varKey, varValue);
            }
            if (simpleVarDictionary != null)
                extensions.Add(Application.identifier + "://" + "targets", simpleVarDictionary);

            foreach (KeyValuePair<string, int> attachStat in diccionary)
            {
                if (attachStat.Key != null)
                    extensions.Add(Application.identifier + "://" + attachStat.Key, attachStat.Value);
            }
            // Hubo cambio de objeto
            extensions.Add(Application.identifier + "://" + "object-changed", 1);
            // Respuesta desconocida
            extensions.Add(Application.identifier + "://" + "correct", -1);
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
        CompletableTracker.Instance.Progressed(level, CompletableTracker.CompletableType.Level, progress);
    }


    public void SetLevel(string level)
    {
        if(level == "rand")
        {
            isRandom = true;
            if (UnityEngine.Random.Range(0.0f, 100.0f) < 50) level = "A";
            else level = "B";
        } else
        {
            isRandom = false;
        }
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

    private IEnumerator ChangeColor(string obj, Color color, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        GameObject c = GameObject.Find(char.ToUpper(obj[0]) + obj.Substring(1));
        SpriteRenderer sr;

        if(c != null)
        {
            sr = c.gameObject.GetComponent<SpriteRenderer>();
            if (sr != null) sr.color = color;
        }
    }

    private void ChangeColor(string obj, Color color)
    {
        GameObject c = GameObject.Find(char.ToUpper(obj[0]) + obj.Substring(1));
        SpriteRenderer sr;

        if (c != null)
        {
            sr = c.gameObject.GetComponent<SpriteRenderer>();
            if (sr != null) sr.color = color;
        }
    }

    private void IncorrectFeedback()
    {
        incorrectText.SetActive(!incorrectText.activeSelf);
    }

    public void SelectGamemode(int m)
    {
        gamemode = m;
        gamemodePanel.SetActive(false);
        levelSelectorPanel.SetActive(true);
    }

    public void GoToSelectGamemode()
    {
        gamemodePanel.SetActive(true);
    }

    private void Initialize()
    {
        this.gameS = GameObject.FindObjectOfType<GameState15O>();

        correctColor = new Color(0, 255, 0);
        normalColor = new Color(255, 255, 255);
        levelSelectorPanel = GameObject.FindGameObjectWithTag("LevelSelector");
        levelSelectorPanel.SetActive(false);
        gamemodePanel = GameObject.FindGameObjectWithTag("PlaystyleSelector");
        gamemodePanel.SetActive(true);
        diccionary = new Dictionary<string, int>();
        answered = new Dictionary<string, int>();
        simpleDictionary = new Dictionary<string, int>();
        reverseDictionary = new Dictionary<int, string>();
        selectedList = new List<string>();
        lista.SetActive(false);
        finalPanel.SetActive(false);
        textBx.gameObject.SetActive(false);
        A.SetActive(false); B.SetActive(false);
    }
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

    public void NavigateOptions(int p)
    {
        selectedPageIndex += p;
        ShowOptions();
    }

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
            GameObject go = selectorOptions[selectedIndex].gameObject;
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
