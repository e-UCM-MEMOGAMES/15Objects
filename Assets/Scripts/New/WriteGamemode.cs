using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;
using UnityEngine.Localization.SmartFormat.Core.Parsing;
using UnityEngine.UI;
using Xasu;
using Xasu.HighLevel;
using static System.Net.Mime.MediaTypeNames;

public class WriteGamemode : BaseGamemode
{
    [SerializeField]
    GameObject inputFieldObj;
    InputField inputField;


    // Start is called before the first frame update
    void Start()
    {
        inputField = inputFieldObj.GetComponent<InputField>();
        inputFieldObj.SetActive(false);
    }

    public override void OnItemSelected(Collider2D[] items)
    {
        base.OnItemSelected(items);
        if (items.Length > 0)
        {
            inputFieldObj.SetActive(true);
        }
    }



    //Este método es llamado cada vez que se pulsa enter en el inputField y recibe de parámetro la palabra introducida.
    //public void OnFieldEnter(string word)
    //{
    //    string log;
    //    if (diccionary.ContainsKey(word.ToLower()))
    //    //Si la palabra se encuentra en el diccionario la añadimos al diccionario de respondidos
    //    {
    //        int value;
    //        diccionary.TryGetValue(word.ToLower(), out value);
    //        answered.Add(word, value);
    //        log = "\t? Ha respondido correctamente con: " + word;
    //        AudioManager.Instance.Play(GameSound.Success);

    //        feedbackResponse.text = notifications[0] + word;
    //        feedbackResponse.gameObject.SetActive(true);
    //        string name;
    //        reverseDictionary.TryGetValue(value, out name);
    //        //Feedback de respuesta
    //        ChangeColor(name, correctColor);
    //        StartCoroutine(ChangeColor(name, normalColor, 2f));
    //        selected = null;
    //        // Tracking
    //        Dictionary<string, bool> simpleVarDictionary = new Dictionary<string, bool>();
    //        foreach (KeyValuePair<string, int> attachStat in simpleDictionary)
    //        {
    //            int simpleValue;
    //            simpleDictionary.TryGetValue(attachStat.Key, out simpleValue);
    //            if (simpleValue == value)
    //            {
    //                simpleVarDictionary.Add(attachStat.Key, true);
    //            }
    //            else
    //            {
    //                simpleVarDictionary.Add(attachStat.Key, false);
    //            }
    //        }
    //        Dictionary<string, object> extensions = new Dictionary<string, object>();
    //        if (simpleVarDictionary != null)
    //            extensions.Add("https://" + "targets", simpleVarDictionary);

    //        foreach (KeyValuePair<string, int> attachStat in diccionary)
    //        {
    //            if (attachStat.Key != null)
    //                extensions.Add("https://" + attachStat.Key, attachStat.Value);
    //        }
    //        // No hubo cambio de objeto
    //        extensions.Add("https://" + "object-changed", 0);
    //        // Respuesta correcta
    //        extensions.Add("https://" + "correct", 1);
    //        if (XasuTracker.Instance.Status.State != TrackerState.Uninitialized)
    //            AlternativeTracker.Instance.Selected(level, word).WithSuccess(true).WithResultExtensions(extensions);
    //    }
    //    else if (word != "")
    //    {
    //        mistakes++;
    //        Debug.Log("Fallaste");
    //        if (answered.ContainsKey(word.ToLower())) log = "\t? Ha respondido una palabra repetida: " + word;
    //        else log = "\t? Ha respondido con error: " + word;
    //        feedbackResponse.text = notifications[0] + word;
    //        feedbackResponse.gameObject.SetActive(true);
    //        IncorrectFeedback();
    //        AudioManager.Instance.Play(GameSound.Failed);
    //        Invoke("IncorrectFeedback", 1.5f);

    //        // Tracking
    //        Dictionary<string, object> extensions = new Dictionary<string, object>();
    //        Dictionary<String, bool> simpleVarDictionary = new Dictionary<string, bool>();
    //        foreach (KeyValuePair<string, int> attachStat in simpleDictionary)
    //        {
    //            simpleVarDictionary.Add(attachStat.Key, false);

    //            // Mappings por si hacen falta en el analysis
    //            String varKey = "mappings_" + attachStat.Key;
    //            String varValue = " ";
    //            foreach (KeyValuePair<string, int> dicKeyValues in diccionary)
    //            {
    //                if (dicKeyValues.Value == attachStat.Value)
    //                {
    //                    varValue += dicKeyValues.Key + ",";
    //                }
    //            }
    //            if (varValue.EndsWith(","))
    //            {
    //                varValue = varValue.Substring(0, varValue.Length - 1);
    //            }
    //            if (varKey != null && varValue != null)
    //                extensions.Add("https://" + varKey, varValue);
    //        }
    //        if (simpleVarDictionary != null)
    //            extensions.Add("https://" + "targets", simpleVarDictionary);

    //        foreach (KeyValuePair<string, int> attachStat in diccionary)
    //        {
    //            if (attachStat.Key != null)
    //                extensions.Add("https://" + attachStat.Key, attachStat.Value);
    //        }
    //        // No hubo cambio de objeto
    //        extensions.Add("https://" + "object-changed", 0);
    //        // Respuesta incorrecta
    //        extensions.Add("https://" + "correct", 0);
    //        if (XasuTracker.Instance.Status.State != TrackerState.Uninitialized)
    //            AlternativeTracker.Instance.Selected(level, word).WithSuccess(false).WithResultExtensions(extensions);
    //    }
    //    else
    //    {
    //        log = "\tHa cambiado de objeto";

    //        // Tracking object changed without answer
    //        Dictionary<string, object> extensions = new Dictionary<string, object>();
    //        Dictionary<string, bool> simpleVarDictionary = new Dictionary<string, bool>();
    //        foreach (KeyValuePair<string, int> attachStat in simpleDictionary)
    //        {
    //            simpleVarDictionary.Add(attachStat.Key, false);

    //            // Mappings por si hacen falta en el analysis
    //            String varKey = "mappings_" + attachStat.Key;
    //            String varValue = " ";
    //            foreach (KeyValuePair<string, int> dicKeyValues in diccionary)
    //            {
    //                if (dicKeyValues.Value == attachStat.Value)
    //                {
    //                    varValue += dicKeyValues.Key + ",";
    //                }
    //            }
    //            if (varValue.EndsWith(","))
    //            {
    //                varValue = varValue.Substring(0, varValue.Length - 1);
    //            }
    //            if (varKey != null && varValue != null)
    //                extensions.Add("https://" + varKey, varValue);
    //        }
    //        if (simpleVarDictionary != null)
    //            extensions.Add("https://" + "targets", simpleVarDictionary);

    //        foreach (KeyValuePair<string, int> attachStat in diccionary)
    //        {
    //            if (attachStat.Key != null)
    //                extensions.Add("https://" + attachStat.Key, attachStat.Value);
    //        }
    //        // Hubo cambio de objeto
    //        extensions.Add("https://" + "object-changed", 1);
    //        // Respuesta desconocida
    //        extensions.Add("https://" + "correct", -1);
    //        if (XasuTracker.Instance.Status.State != TrackerState.Uninitialized)
    //            AlternativeTracker.Instance.Selected(level, "empty").WithSuccess(false).WithResultExtensions(extensions);
    //    }
    //    log += "\n";

    //    Byte[] info = new UTF8Encoding(true).GetBytes(log);
    //    fs.Write(info, 0, info.Length);

    //    diccionary.Clear();                                                    //Limpiamos el diccionario.
    //    simpleDictionary.Clear();
    //    reverseDictionary.Clear();
    //    textBx.gameObject.SetActive(false);
    //    pointerPos.SetActive(false);
    //    rightButton.SetActive(false);
    //    leftButton.SetActive(false);
    //    foreach (GameObject go in selectorOptions)
    //        go.SetActive(false);

    //    attempts++;

    //    if (hayCont) cont.text = notifications[0] + attempts.ToString() + notifications[1] + (totalAttempts - attempts).ToString();
    //    if (hayLista)
    //        listaText.text += "\n- " + word;

    //    textBx.Select();
    //    textBx.text = "";

    //    // Progreso del nivel actual
    //    float progress = (float)attempts / (float)totalAttempts;
    //    if (XasuTracker.Instance.Status.State != TrackerState.Uninitialized)
    //        CompletableTracker.Instance.Progressed(level, CompletableTracker.CompletableType.Level, progress);
    //}

    //public void Limpiatexto(Text txt)
    //{
    //    txt.text = "";
    //}
}
