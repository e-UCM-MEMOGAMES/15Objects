using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class LevelInfo : MonoBehaviour
{
    /// <summary>
    /// Nombre del nivel
    /// </summary>
    [SerializeField]
    string levelName;
    public string LevelName
    {
        get { return levelName; }
        private set { }
    }

    /// <summary>
    /// Ruta de los archivos de localizacion de los niveles
    /// </summary>
    const string LOCALIZATION_FILES_PATH = "Localization/Levels";


    // Start is called before the first frame update
    void Start()
    {
        LevelName = levelName;

        // Se obtiene la ruta del archivo de localizacion correspondiente al nivel
        string currLoc = $"{LOCALIZATION_FILES_PATH}/{LocalizationSettings.SelectedLocale.Identifier.Code}_{levelName}";
        //Debug.Log(currLoc);

        // Carga el archivo como texto plano y se parsea a un array de JSON
        TextAsset jsonFile = (TextAsset)Resources.Load(currLoc, typeof(TextAsset));
        JArray jsonObject = JArray.Parse(jsonFile.text);

        // Recorre cada elemento leido
        foreach (JObject obj in jsonObject)
        {
            // Obtiene el componente Item del objeto cuyo nombre en la escena sea el nombre leido
            string itemName = (string)obj["item"];
            GameObject item = GameObject.Find(itemName);
            Item levelItem = item.GetComponent<Item>();
            //Debug.Log(itemName);

            // Se guardan todas las palabras en su lista correspondiente
            // (EN MINUSCULAS para facilitar su procesado en los modos de juego)
            foreach (string word in obj["words"])
            {
                levelItem.CorrectWords.Add(word.ToLower());
            }
            foreach (string word in obj["fillers"])
            {
                levelItem.FillerWords.Add(word.ToLower());
            }
        }
    }
}
