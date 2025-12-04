using TMPro;
using UnityEngine;

public class WriteGamemode : BaseGamemode
{
    [SerializeField]
    /// <summary>
    /// Objeto con la caja de input
    /// </summary> 
    GameObject inputFieldObj;

    /// <summary>
    /// Componente InputField de la caja de input
    /// </summary> 
    TMP_InputField inputField;


    // Start is called before the first frame update
    void Start()
    {
        inputField = inputFieldObj.GetComponent<TMP_InputField>();
        inputFieldObj.SetActive(false);

        // Anade un listener al onSubmit de la caja de input para que se llame a SendAnswer cuando se pulsa el enter
        // (Se tiene que hacer desde codigo porque no esta en el editor. NO USAR onEndEdit PORQUE ESO TAMBIEN SE LLAMA AL PERDER EL FOCO)
        inputField.onSubmit.AddListener(SendAnswer);
    }

    public override void ShowElements()
    {
        inputFieldObj.SetActive(true);
        inputField.text = "";
        inputField.Select();
    }
    public override void HideElements()
    {
        // Desactiva la caja de input
        inputFieldObj.SetActive(false);
        inputField.text = "";
    }


    public override void SendAnswer(string answer)
    {
        // Solo envia la respuesta si hay texto introducido
        if (!string.IsNullOrEmpty(inputField.text))
        {
            base.SendAnswer(inputField.text);
        }
    }
}
