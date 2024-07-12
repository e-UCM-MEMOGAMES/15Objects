using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

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

    JObject jobj;
    IList<JToken> info;

    /// <summary>
    /// Lee la informacion del nivel de un json
    /// </summary> 
    public void initItems()
    {
        string currLoc = "Localization/";
        //currLoc = string.Concat(currLoc, LocalizationSettings.SelectedLocale.Identifier.Code);
        currLoc = string.Concat(currLoc, GM.Instance.Level);
        TextAsset jsonFile = (TextAsset)Resources.Load(currLoc, typeof(TextAsset));
        Debug.Log(jsonFile);
        jobj = JObject.Parse(jsonFile.text);
        info = jobj["items"].Children().ToList();
        
        ReadItems();
    }

    /// <summary>
    /// Metodo auxilizar de InitItems, lee y alamcena la informacion de los objetos
    /// </summary> 
    void ReadItems()
    {
        //objetos de escena
        foreach(JToken obj in info)
        {
            Debug.Log(GameObject.Find(obj.Value<string>("item")));
            Objeto aux = GameObject.Find(obj.Value<string>("item")).GetComponent<Objeto>();
            string currLangID = LocalizationSettings.SelectedLocale.Identifier.Code;


            if (aux == null) continue;

            aux.InitLists();
            foreach (string word in obj["words"][currLangID])
                aux.addWord(word);

            foreach (string word in obj["fillers"][currLangID])
                aux.addFiller(word);

            int a;
            foreach (string word in aux.dameDic(out a))
                Debug.Log("Word:" + word);
            foreach (string word in aux.dameFill())
                Debug.Log("Filler:" + word);

        }

    }
}
