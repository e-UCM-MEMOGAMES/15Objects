using System.Collections;
using System.Collections.Generic;
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
    [System.Serializable]
    public class LevelInfo
    {
        public ObjectInfo[] items;
    }
    [System.Serializable]
    public class ObjectInfo
    {
        public string item;
        public string[] words;
        public string[] fillers;
    }

    LevelInfo info;

    public void initItems()
    {
        string currLoc = "Localization/";
        currLoc = string.Concat(currLoc, LocalizationSettings.SelectedLocale.Identifier.Code);
        currLoc = string.Concat(currLoc, GM.Instance.Level);
        TextAsset jsonFile = (TextAsset)Resources.Load(currLoc, typeof(TextAsset));
        Debug.Log(jsonFile);
        info = JsonUtility.FromJson<LevelInfo>(jsonFile.text);
        
        ReadItems();
    }

    void ReadItems()
    {
        //objetos de escena
        foreach(ObjectInfo obj in info.items)
        {
            Objeto aux = GameObject.Find(obj.item).GetComponent<Objeto>();
            
            if(aux == null) continue;

            aux.InitLists();
            foreach (string word in obj.words)            
                aux.addWord(word);
            
            foreach (string word in obj.fillers)
                aux.addFiller(word);
            
            int a;
            foreach (string word in aux.dameDic(out a))
                Debug.Log("Word:" + word);
            foreach (string word in aux.dameFill())
                Debug.Log("Filler:" + word);

        }

    }
}
