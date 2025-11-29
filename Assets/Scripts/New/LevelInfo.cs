using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class LevelInfo : MonoBehaviour
{
    [SerializeField]
    string levelName;
    public string LevelName
    {
        get { return levelName; }
        private set { }
    }

    [SerializeField]
    string localizationFilesPath = "Localization/Levels";

    // Start is called before the first frame update
    void Start()
    {
        LevelName = levelName;

        string currLoc = $"{localizationFilesPath}/{LocalizationSettings.SelectedLocale.Identifier.Code}_{levelName}";
        Debug.Log(currLoc);

        TextAsset jsonFile = (TextAsset)Resources.Load(currLoc, typeof(TextAsset));
        JArray jsonObject = JArray.Parse(jsonFile.text);

        foreach (JObject obj in jsonObject)
        {
            string itemName = (string)obj["item"];
            //Debug.Log(itemName);

            GameObject item = GameObject.Find(itemName);
            Item levelItem = item.GetComponent<Item>();

            foreach (string word in obj["words"])
            {
                levelItem.CorrectWords.Add(word);
            }
            foreach (string word in obj["fillers"])
            {
                levelItem.FillerWords.Add(word);
            }
        }
    }
}
