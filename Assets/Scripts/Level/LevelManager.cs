using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;
using Xasu.HighLevel;


public class LevelManager : MonoBehaviour
{
    /// <summary>
    /// Temporizador para medir el tiempo que se tarda en completar el nivel
    /// </summary>
    protected Stopwatch watch = Stopwatch.StartNew();

    /// <summary>
    /// Instancia del TrackerManager
    /// </summary>
    protected TrackerManager trackerManager;

    protected CompletableTracker.CompletableType COMPLETABLE_TYPE = CompletableTracker.CompletableType.Level;

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
    /// Lista de los objetos seleccionados
    /// </summary> 
    protected List<Item> selectedItems = new List<Item>();

    /// <summary>
    /// Lista con los objetos respondidos correctamente
    /// </summary>
    protected HashSet<Item> correctItems = new HashSet<Item>();

    /// <summary>
    /// Numero de intentos que se pueden hacer antes de terminar el juego
    /// </summary>
    protected int maxAttempts = 15,
    /// <summary>
    /// Numero de veces que se ha respondido (ya sea correcta o incorrectamente)
    /// </summary>
    attempts = 0,
    /// <summary>
    /// Numero de veces que se ha respondido incorrectamente
    /// </summary>
    mistakes = 0;


    // Start is called before the first frame update
    protected virtual void Start()
    {
        trackerManager = TrackerManager.Instance;

        watch.Start();

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

        trackerManager.TrySendStatement(CompletableTracker.Instance.Initialized(levelName, COMPLETABLE_TYPE));


        pointerPos.SetActive(false);
        answer.SetActive(false);
        noSelection.SetActive(false);
        incorrect.SetActive(false);
        remaining.SetActive(false);
        resultsPanel.SetActive(false);

        // Se obtiene el texto por defecto localizado del objeto respondido y se
        // desactiva la localizacion para que se pueda anadir el nombre del objeto
        LocalizeStringEvent localizeEvt = answer.GetComponent<LocalizeStringEvent>();
        defaultAnswerText = localizeEvt.StringReference.GetLocalizedString();
        localizeEvt.enabled = false;
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

                // Se borran los objetos seleccionados que hubiera guardados anteriormente y se guardon los nuevas
                selectedItems.Clear();
                foreach (Collider2D item in items)
                {
                    Item it = item.gameObject.GetComponent<Item>();
                    selectedItems.Add(it);
                    trackerManager.TrySendStatement(GameObjectTracker.Instance.Interacted(item.gameObject.name, GameObjectTracker.TrackedGameObject.Item));
                }

                // Se deja al modo de juego gestionar los objetos pulsados
                gamemode.OnItemSelected(selectedItems);
            }
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
    public virtual void Answer(string answer)
    {
        // Busca el primer objeto que tenga la palabra respondida
        // en su lista de palabras correctas y lo guarda
        bool found = false;
        Item answeredItem = null;
        for (int i = 0; i < selectedItems.Count() && !found; i++)
        {
            if (selectedItems[i].CorrectWords.Contains(answer.ToLower()))
            {
                answeredItem = selectedItems[i];
                found = true;
            }

        }

        bool correct = (found && answeredItem != null);
        bool repeatedAnswer = correctItems.Contains(answeredItem);

        // Si ha encontrado algun objeto, es que la respuesta es correcta
        if (correct)
        {
            // Si el objeto no estaba respondido, se contabiliza la respuesta y se guarda como respondido
            if (!repeatedAnswer)
            {
                attempts++;
                correctItems.Add(answeredItem);
            }

            // Cambia el objeto respondido correctamente de color 
            StartCoroutine(ChangeColor(answeredItem.gameObject, correctColor));
            StartCoroutine(ChangeColor(answeredItem.gameObject, defaultColor, feedbackDuration));
            audioManager.Play(GameSound.Success);
        }
        // Si no, la respuesta es incorrecta
        else
        {
            attempts++;
            mistakes++;

            // Se muestra el texto que indica que la respuesta es incorrecta
            incorrect.SetActive(true);
            StartCoroutine(ActivateObj(incorrect, false, feedbackDuration));

            audioManager.Play(GameSound.Failed);
        }

        // Se muestra el texto que muestra que respuesta ha dado el jugador
        answerText.text = defaultAnswerText + " " + answer;
        this.answer.SetActive(true);
        StartCoroutine(ActivateObj(this.answer, false, feedbackDuration));


        Dictionary<string, object> extensions = new Dictionary<string, object>();

        extensions.Add("https://repeatedAnswer", repeatedAnswer);

        // Se recorre cada objeto seleccionado guardando sus posibles respuestas
        foreach (Item item in selectedItems)
        {
            extensions.Add($"https://{item.gameObject.name}/answers", item.CorrectWords);
        }
        trackerManager.TrySendStatement(
            AlternativeTracker.Instance.Selected(answeredItem == null ? "wrong-item" : answeredItem.name, answer)
            .WithSuccess(correct)
            .WithResultExtensions(extensions)
        );


        // Progreso del nivel actual
        float progress = (float)attempts / maxAttempts;
        trackerManager.TrySendStatement(CompletableTracker.Instance.Progressed(levelName, COMPLETABLE_TYPE, progress));
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
            watch.Stop();
            long completionTime = watch.ElapsedMilliseconds;

            // Se activa el panel y se desactiva el boton de volver
            resultsPanel.SetActive(true);
            returnButton.SetActive(false);

            // Se cambia el texto de la puntuacion total
            totalPointsText.text = $"{attempts - mistakes}/{maxAttempts}";

            // Se determina si se ha fallado el nivel y la puntuacion final
            float score = 1.0f - (mistakes / maxAttempts);
            bool failed = mistakes > (maxAttempts / 2.0f);

            trackerManager.TrySendStatement(
                CompletableTracker.Instance.Completed(levelName, COMPLETABLE_TYPE, watch.ElapsedMilliseconds)
                .WithScore(score)
                .WithSuccess(!failed)
            );
        }
        // Si es visible, se vuelve al menu de configuracion del nivel
        else
        {
            gameManager.ChangeScene(Defs.LEVEL_SETTINGS_SCENE_NAME);
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

        mngr.trackerManager = trackerManager;

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
