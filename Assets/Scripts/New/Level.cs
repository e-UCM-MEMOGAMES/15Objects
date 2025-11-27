using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using TMPro;
using UnityEngine;
using Xasu;
using Xasu.HighLevel;

public class Level : MonoBehaviour
{
    [SerializeField]
    GameObject returnButton, pointerPos,
               answerText, noAnswerText, incorrectText, remainingText, results;

    [SerializeField]
    GameObject[] gamemodesList, levelList;

    GameObject currGamemode, currLevel;
    Gamemode gamemode;

    AudioManager audioManager;

    bool fileConfig = false;
    FileStream fs;

    TextMeshProUGUI pointsText;
    const int TOTAL_ATTEMPTS = 15;
    int attempts = 0, mistakes = 0;
    string levelName = "";


    // Start is called before the first frame update
    void Start()
    {
        audioManager = AudioManager.Instance;
        LoadFileConfig();
    }

    void OnEnable()
    {
        pointerPos.SetActive(false);
        answerText.SetActive(false);
        noAnswerText.SetActive(false);
        incorrectText.SetActive(false);
        remainingText.SetActive(false);
        results.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Se comprueba si en el punto del mouse al hacer click hay colisión con algún objeto. Se devuelven todos los objetos en result.
            Collider2D[] result = Physics2D.OverlapPointAll(Input.mousePosition);

            if (result.Length > 0)
            {
                pointerPos.SetActive(true);
                pointerPos.transform.position = Input.mousePosition;
                audioManager.Play(GameSound.Point);

                gamemode.OnItemSelected(result);
            }
            else
            {

            }
        }
    }


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
        fileConfig = false;
        pointsText.text = (attempts - mistakes).ToString() + "/" + TOTAL_ATTEMPTS;

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


    public void SelectGamemode(GameObject mode)
    {
        currGamemode = mode;
        gamemode = currGamemode.GetComponent<Gamemode>();

        foreach (GameObject gamemodes in gamemodesList)
        {
            gamemodes.SetActive(false);
        }
        if (currGamemode != null)
        {
            currGamemode.SetActive(true);
        }
    }

    public void SelectLevel(GameObject level)
    {
        currLevel = level;
        levelName = level.name;

        foreach (GameObject levels in levelList)
        {
            levels.SetActive(false);
        }
        if (currLevel != null)
        {
            currLevel.SetActive(true);
        }
    }

    public void Return()
    {
        if (returnButton.activeSelf)
        {
            returnButton.SetActive(false);
            results.SetActive(true);

            gamemode.Return();
            EndGame();
        }
        else
        {
            returnButton.SetActive(true);
            gameObject.SetActive(false);
        }
    }

}
