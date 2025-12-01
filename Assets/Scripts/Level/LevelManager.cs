using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;
using Xasu;
using Xasu.HighLevel;

public class LevelManager : MonoBehaviour
{
    /// <summary>
    /// Objeto con el boton de volver
    /// </summary>
    [SerializeField]
    protected GameObject returnButton,
    /// <summary>
    /// Objeto con el indicador de la posicion del puntero
    /// </summary>
    pointerPos,
    /// <summary>
    /// Objeto con el texto de la palabra respondida
    /// </summary>
    answer,
    /// <summary>
    /// Objeto con el texto de que no se ha seleccionado ningun objeto
    /// </summary>
    noSelection,
    /// <summary>
    /// Objeto con el texto de respuesta incorrecta
    /// </summary>
    incorrect,
    /// <summary>
    /// Objeto con el texto de objetos restantes    <-- NO SE USA
    /// </summary>
    remaining,
    /// <summary>
    /// Objeto con los elementos del resultado
    /// </summary>
    resultsPanel;


    /// <summary>
    /// Texto de la palabra respondida
    /// </summary>
    [SerializeField]
    protected TextMeshProUGUI answerText,
    /// <summary>
    /// Texto de los puntos totales
    /// </summary>
    totalPointsText;

    /// <summary>
    /// Texto por defecto (sin anadir el nombre del objeto) de la palabra respondida
    /// </summary>
    protected string defaultAnswerText;

    /// <summary>
    /// Tiempo que tarda en desaparecer el feedback
    /// </summary>
    [SerializeField]
    protected float feedbackDuration = 1.0f;

    /// <summary>
    /// Instancia del GameManager
    /// </summary>
    protected GameManager gameManager;

    /// <summary>
    /// Instancia del AudioManager
    /// </summary>
    protected AudioManager audioManager;

    /// <summary>
    /// Script del modo de juego seleccionado
    /// </summary>
    protected BaseGamemode gamemode;
    
    /// <summary>
    /// Objetos especificos del modo de juego seleccionado
    /// </summary>
    protected GameObject gamemodeElements,
    /// <summary>
    /// Objetos del nivel seleccionado
    /// </summary>
    levelItems;

    /// <summary>
    /// Informacion del nivel seleccionado
    /// </summary>
    protected LevelInfo levelInfo;

    /// <summary>
    /// Nombre del nivel seleccionado
    /// </summary>
    protected string levelName = "";

    /// <summary>
    /// Color por defecto de los objetos
    /// </summary>
    protected Color defaultColor = new Color(255, 255, 255),
    /// <summary>
    /// Color de los objetos cuando la respuesta es correcta
    /// </summary>
    correctColor = new Color(0, 255, 0);

    /// <summary>
    /// Numero de intentos que se pueden hacer antes de terminar el juego
    /// </summary>
    protected int maxAttempts = 15;
    /// <summary>
    /// Numero de veces que se ha respondido (ya sea correcta o incorrectamente)
    /// </summary>
    protected int attempts = 0,
    /// <summary>
    /// Numero de veces que se ha respondido incorrectamente
    /// </summary>
    mistakes = 0;

    /// <summary>
    /// ????
    /// </summary>
    /// TODO
    bool fileConfig = false;
    FileStream fs;


    // Start is called before the first frame update
    protected virtual void Start()
    {
        attempts = 0;
        mistakes = 0;

        gameManager = GameManager.Instance;
        audioManager = AudioManager.Instance;

        // Se instancian los elementos del modo de juego por encima (en el editor
        // el objeto esta debajo) del indicadord el puntero
        gamemodeElements = Instantiate(gameManager.GamemodeElements, transform);
        gamemodeElements.transform.SetSiblingIndex(pointerPos.transform.GetSiblingIndex() + 1);
        gamemode = gamemodeElements.GetComponent<BaseGamemode>();
        gamemode.LevelManager = this;

        // Se instancian los objetos del nivel por debajo (en el editor
        // el objeto esta encima) del indicadord el puntero
        levelItems = Instantiate(gameManager.LevelItems, transform);
        levelItems.transform.SetSiblingIndex(pointerPos.transform.GetSiblingIndex());
        levelInfo = levelItems.GetComponent<LevelInfo>();
        levelName = levelInfo.LevelName;


        pointerPos.SetActive(false);
        answer.SetActive(false);
        noSelection.SetActive(false);
        incorrect.SetActive(false);
        remaining.SetActive(false);
        resultsPanel.SetActive(false);

        defaultAnswerText = answer.GetComponent<LocalizeStringEvent>().StringReference.GetLocalizedString();

        LoadFileConfig();
    }


    // Update is called once per frame
    protected virtual void Update()
    {
        // Si se ha pulsado la pantalla (con click izquierdo usando raton), no se
        // esta dando feedback, y no se ha superado el numero maximo de intentos
        if (Input.GetMouseButtonDown(0) && !answer.activeSelf && !resultsPanel.activeSelf && attempts <= maxAttempts)
        {
            // Se comprueba si en el punto del mouse al hacer click hay colisión con algún objeto. Se devuelven todos los objetos en items
            Collider2D[] items = Physics2D.OverlapPointAll(Input.mousePosition);

            // Si hay algun objeto
            if (items.Length > 0)
            {
                // Se activa el indicador del puntero, se coloca donde se ha pulsado y se reproduce un sonido
                pointerPos.SetActive(true);
                pointerPos.transform.position = Input.mousePosition;
                audioManager.Play(GameSound.Point);

                // Se deja al modo de juego gestionar los objetos pulsados
                gamemode.OnItemSelected(items);
            }

            // TODO
            //string log = "Se ha pinchado en: ";
            //foreach (Collider2D c in items)
            //{
            //    log += c.gameObject.name + " ";
            //}
            //log += "\n";
            //Byte[] info = new UTF8Encoding(true).GetBytes(log);
            //if (items.Length > 0) fs.Write(info, 0, info.Length);
        }
        // Si no, si se ha superado el, no se esta recibiendo feedback, y el panel de resultados no esta activo, se termina el juego 
        else if (attempts >= maxAttempts && !answer.activeSelf && !resultsPanel.activeSelf)
        {
            EndGame();
        }
    }


    /// <summary>
    /// Cambia el color del objeto pasado como parametro tras un delay
    /// </summary>
    protected IEnumerator ChangeColor(GameObject obj, Color color, float delayTime = 0)
    {
        yield return new WaitForSeconds(delayTime);

        Image img = obj.GetComponent<Image>();
        img.color = color;

    }

    /// <summary>
    /// Activar/desactivar el objeto pasado como parametro tras un delay
    /// </summary>
    protected IEnumerator ActivateObj(GameObject obj, bool activate, float delayTime = 0)
    {
        yield return new WaitForSeconds(delayTime);

        obj.SetActive(activate);
    }


    /// <summary>
    /// Llamado al responder un objeto, ya sea correcta o incorrectamente
    /// </summary>
    protected virtual void Answer(string itemName)
    {
        attempts++;

        answerText.text = defaultAnswerText + " " + itemName;
        answer.SetActive(true);
        StartCoroutine(ActivateObj(answer, false, feedbackDuration));

        // TODO
        //  Progreso del nivel actual
        //  float progress = (float)attempts / (float)totalAttempts;
        //  if (XasuTracker.Instance.Status.State != TrackerState.Uninitialized)
        //      CompletableTracker.Instance.Progressed(level, CompletableTracker.CompletableType.Level, progress);

        //Debug.Log($"Intentos: {attempts}, Errores: {mistakes}");
    }
    /// <summary>
    /// Muestra el texto que indica que la respuesta es incorrecta
    /// </summary> 
    public void IncorrectAnswer(GameObject item, string itemName)
    {
        mistakes++;
        Answer(itemName);
        
        incorrect.SetActive(true);
        StartCoroutine(ActivateObj(incorrect, false, feedbackDuration));

        audioManager.Play(GameSound.Failed);
    }
    /// <summary>
    /// Indica que la respuesta es correcta cambiando de color el objeto respondido correctamente
    /// </summary> 
    public void CorrectAnswer(GameObject item, string itemName)
    {
        Answer(itemName);

        StartCoroutine(ChangeColor(item, correctColor));
        StartCoroutine(ChangeColor(item, defaultColor, feedbackDuration));
        audioManager.Play(GameSound.Success);

        // TODO: REVISAR
        //obj.GetComponent<Item>().enabled = false;
        item.GetComponent<Item>().CorrectWords.Clear();
        item.GetComponent<Item>().FillerWords.Clear();
    }


    // TODO: REVISAR
    void LoadFileConfig()
    {
        // Para escribir los resultados en un archivo
        string path;
        if (fileConfig)
        {
            path = @".\configFile15O.txt";
            if (!File.Exists(path))
            {
                // Note that no lock is put on the file and the possibility exists that another
                // process could do something with it between the calls to Exists and Delete.
                fs = File.Create(path);
                Byte[] info = new UTF8Encoding(true).GetBytes("A");
                fs.Write(info, 0, info.Length);
                fs.Close();
            }

            StreamReader file = new StreamReader(path);
            file.Close();
            fileConfig = false;
        }

        path = @".\Resultados.txt";

        bool closed = false;
        int attempts = 0, maxTries = 10;

        //Mecanismo para que se reintente borrar el archivo si este ya existe
        while (!closed)
        {
            try
            {
                if (File.Exists(path))
                {
                    // Note that no lock is put on the file and the possibility exists that another
                    // process could do something with it between the calls to Exists and Delete.
                    File.Delete(path);
                }
                fs = File.Create(path);
                closed = true;
            }
            catch (IOException ex)
            {
                //error si se intenta demasiadas veces sin exito
                if (++attempts > maxTries)
                {
                    Debug.Log($"Failed to handle the file after {maxTries} attempts: {ex.Message}");
                    throw;
                }
                //demora antes de reintentarlo
                Thread.Sleep(100);
            }
        }
    }


    /// <summary>
    /// Termina la partida (llamado por el Update, el boton de 
    /// volver, y el boton de continuar del panel de resultados)
    /// </summary> 
    public void EndGame()
    {
        // Si el panel de resultados no es visible, es que todavia no se ha mostrado la puntuacion
        if (!resultsPanel.activeSelf)
        {
            // Se activa el panel y se desactiva el boton de volver
            resultsPanel.SetActive(true);
            returnButton.SetActive(false);

            // Se cambia el texto de la puntuacion total
            totalPointsText.text = $"{attempts - mistakes}/{maxAttempts}";

            // Se determina si se ha fallado el nivel y la puntuacion final
            bool failed = mistakes > (maxAttempts / 2.0f);
            float score = 1.0f - (mistakes / maxAttempts);

            // TODO: REVISAR
            fileConfig = false;
            if (XasuTracker.Instance.Status.State != TrackerState.Uninitialized)
            {
                CompletableTracker.Instance.Completed(levelName, CompletableTracker.CompletableType.Level).
                    WithResultExtensions(new Dictionary<string, object> {
                        { "https://" + "result", !failed },
                        { "https://" + "score", score }
                    }
                );

            }
        }
        // Si es visible, se vuelve al menu de configuracion del nivel
        else
        {
            gameManager.ChangeScene(gameManager.LEVEL_SETTINGS_SCENE_NAME);
        }
    }


    /// <summary>
    /// Configura el tutorial, "clonando" todos los atributos de la instancia de LevelManager 
    /// desde la que se llama a la instancia de TutorialManager que se le pasa como parametro
    /// 
    /// Se tiene que hacer asi porque el TutorialManager es parte de los objetos del nivel, no
    /// de la escena de juego, por lo que el TutorialManager no puede conocer los elementos de
    /// la escena instanciados previamente y tampoco puede acceder a ellos desde la clase padre
    /// porque las instancias son distintas. Una vez terminada la "clonacion", la instancia
    /// de este script se desactiva para que el TutorialManager tome el control
    /// </summary> 
    public void SetupTutorial(TutorialManager mngr)
    {
        maxAttempts = mngr.maxAttempts;

        mngr.returnButton = returnButton;
        mngr.pointerPos = pointerPos;
        mngr.answer = answer;
        mngr.noSelection = noSelection;
        mngr.incorrect = incorrect;
        mngr.remaining = remaining;
        mngr.resultsPanel = resultsPanel;

        mngr.feedbackDuration = feedbackDuration;

        mngr.totalPointsText = totalPointsText;
        mngr.answerText = answerText;

        mngr.defaultAnswerText = defaultAnswerText;
        mngr.gameManager = gameManager;
        mngr.audioManager = audioManager;

        mngr.gamemode = gamemode;
        mngr.gamemodeElements = gamemodeElements;
        mngr.levelItems = levelItems;
        mngr.levelInfo = levelInfo;
        mngr.levelName = levelName;

        this.enabled = false;
    }
}
