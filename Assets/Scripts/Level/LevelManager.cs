using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Xasu;
using Xasu.HighLevel;

public class LevelManager : MonoBehaviour
{
    [SerializeField]
    ///
    ///
    GameObject returnButton, pointerPos,
               answer, noSelection, incorrect, remaining, 
               resultsPanel;

    [SerializeField]
    float feedbackDuration = 1.0f;

    [SerializeField]
    TextMeshProUGUI totalPointsText, answerText;

    string defaultAnswerText;

    GameManager gameManager;
    AudioManager audioManager;

    BaseGamemode gamemode;
    GameObject gamemodeElements, levelItems;
    LevelInfo levelInfo;
    string levelName = "";

    Color normalColor = new Color(255, 255, 255),
          correctColor = new Color(0, 255, 0);

    const int TOTAL_ATTEMPTS = 15;
    int attempts = 0, mistakes = 0;

    bool fileConfig = false;
    FileStream fs;




    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.Instance;
        audioManager = AudioManager.Instance;

        gamemodeElements = Instantiate(gameManager.GamemodeElements, transform);
        gamemodeElements.transform.SetSiblingIndex(pointerPos.transform.GetSiblingIndex() + 1);
        gamemode = gamemodeElements.GetComponent<BaseGamemode>();
        gamemode.LevelManager = this;

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

        defaultAnswerText = answerText.text;

        LoadFileConfig();
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !answer.activeSelf && !resultsPanel.activeSelf && attempts <= TOTAL_ATTEMPTS)
        {
            // Se comprueba si en el punto del mouse al hacer click hay colisión con algún objeto. Se devuelven todos los objetos en result.
            Collider2D[] items = Physics2D.OverlapPointAll(Input.mousePosition);

            if (items.Length > 0)
            {
                pointerPos.SetActive(true);
                pointerPos.transform.position = Input.mousePosition;
                audioManager.Play(GameSound.Point);

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
        else if (!answer.activeSelf && !resultsPanel.activeSelf && attempts > TOTAL_ATTEMPTS)
        {
            EndGame();
        }
    }


    /// <summary>
    /// Cambia el color del objeto pasado como parametro tras un delay
    /// </summary>
    private IEnumerator ChangeColor(GameObject obj, Color color, float delayTime = 0)
    {
        yield return new WaitForSeconds(delayTime);

        Image img = obj.GetComponent<Image>();
        img.color = color;

    }

    /// <summary>
    /// Activar/desactivar el objeto pasado como parametro tras un delay
    /// </summary>
    private IEnumerator ActivateObj(GameObject obj, bool activate, float delayTime = 0)
    {
        yield return new WaitForSeconds(delayTime);

        obj.SetActive(activate);
    }


    private void Answer(string itemName)
    {
        attempts++;

        answer.SetActive(true);
        answerText.text = defaultAnswerText + " " + itemName;
        StartCoroutine(ActivateObj(answer, false, feedbackDuration));

        // TODO
        //  Progreso del nivel actual
        //  float progress = (float)attempts / (float)totalAttempts;
        //  if (XasuTracker.Instance.Status.State != TrackerState.Uninitialized)
        //      CompletableTracker.Instance.Progressed(level, CompletableTracker.CompletableType.Level, progress);
    }
    /// <summary>
    /// Muestra el texto que indica que la respuesta es incorrecta
    /// </summary> 
    public void IncorrectAnswer(GameObject item, string itemName)
    {
        Answer(itemName);
        mistakes++;
        
        incorrect.SetActive(true);
        StartCoroutine(ActivateObj(incorrect, false, feedbackDuration));

        audioManager.Play(GameSound.Failed);
    }

    public void CorrectAnswer(GameObject item, string itemName)
    {
        Answer(itemName);

        attempts++;
        StartCoroutine(ChangeColor(item, correctColor));
        StartCoroutine(ChangeColor(item, normalColor, feedbackDuration));
        audioManager.Play(GameSound.Success);

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
    /// Termina la partida
    /// </summary> 
    public void EndGame()
    {
        // TODO: REVISAR

        if (returnButton.activeSelf)
        {
            returnButton.SetActive(false);
            resultsPanel.SetActive(true);

            fileConfig = false;
            totalPointsText.text = $"{attempts - mistakes}/{TOTAL_ATTEMPTS}";

            // Completed the 15 Objects level
            bool failed = (float)mistakes > ((float)TOTAL_ATTEMPTS / 2.0f);
            float score = 1.0f - ((float)mistakes / (float)TOTAL_ATTEMPTS);

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
        else
        {
            gameManager.ChangeScene(gameManager.LEVEL_SETTINGS_SCENE);
        }
    }
}
