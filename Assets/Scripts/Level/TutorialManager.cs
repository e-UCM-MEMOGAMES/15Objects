using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Xasu.HighLevel;

public class TutorialManager : LevelManager
{
    /// <summary>
    /// Estados del tutorial (en que momentos se muestra cada panel)
    /// </summary>
    enum States { PANEL1, PANEL2, PANEL3, PANEL4, PANEL5, LAST };
    States currState = States.PANEL1;

    /// <summary>
    /// Paneles con la informacion de cada estado del tutorial
    /// </summary>
    [SerializeField]
    GameObject[] statePanels;

    /// <summary>
    /// Objeto con el mensaje de error del estado Panel2
    /// </summary>
    [SerializeField]
    GameObject panel2Error,
    /// <summary>
    /// Objeto con el panel 3 en el modo de juego de seleccionar
    /// </summary>
    panel3Select,
    /// <summary>
    /// Objeto con el panel 3 en el modo de juego de escribir
    /// </summary>
    panel3Write;

    /// <summary>
    /// Collider del objeto de la botella
    /// </summary>
    [SerializeField]
    Collider2D bottleCollider;


    // Start is called before the first frame update
    protected override void Start()
    {
        // Se actualiza el numero de objetos totales(15 en la clase padre)
        totalItems = 5;
        
        // Se elige el panel 3 dependiendo del modo de juego seleccionado y se desactiva el contrario
        if (gamemode is SelectGamemode)
        {
            statePanels[(int)States.PANEL3] = panel3Select;
            panel3Write.SetActive(false);
        }
        else if (gamemode is WriteGamemode)
        {
            statePanels[(int)States.PANEL3] = panel3Write;
            panel3Select.SetActive(false);
        }

        // Se desactivan todos los paneles
        foreach (GameObject panel in statePanels)
        {
            panel.SetActive(false);
        }

        // Se desactiva el resto de elementos
        panel2Error.SetActive(false);

        statePanels[(int)States.PANEL1].SetActive(true);

        //returnButton.GetComponent<Button>().enabled = false;
    }

    //Update is called once per frame
    protected override void Update()
    {
        // Si se ha pulsado la pantalla y no se ha llegado al ultimo panel
        if (Input.GetMouseButtonDown(0) && (int)currState < statePanels.Length)
        {
            // Si no se esta en los estados 2 o 3 (los que requieren seleccionar la botella y responder),
            // se actualiza al siguiente estado (ya que solo se necesita pulsar para pasarlos)
            if (currState != States.PANEL2 && currState != States.PANEL3)
            {
                UpdateState();
            }
            // Si no, se esta en los estados 2 o 3
            else
            {
                // Se comprueba si en el punto del cursor al pulsar hay colision con algun objeto. Se devuelven todos los objetos en items
                Collider2D[] items = Physics2D.OverlapPointAll(Input.mousePosition);

                // Si hay algun objeto seleccionado
                if (items.Length > 0)
                {
                    // Se desactiva el texto de objeto no seleccionado
                    noSelection.SetActive(false);

                    // Si la botella no esta entre los objetos seleccionados
                    if (!items.ToList().Contains(bottleCollider))
                    {
                        // Se activa el indicador del puntero, se coloca donde se ha pulsado y se reproduce un sonido
                        pointerPos.SetActive(true);
                        pointerPos.transform.position = Input.mousePosition;
                        audioManager.Play(GameSound.Point);

                        // Se activa el mensaje de error
                        panel2Error.SetActive(true);

                        // Se ocultan los elementos del modo de juego
                        gamemode.HideElements();

                        // Se vuelve al estado anterior si se estaba en el panel de responder
                        if (currState == States.PANEL3)
                        {
                            UpdateState(-1);
                        }
                    }
                    // Si no, la botella esta entre los objetos seleccionados
                    else
                    {
                        // Se desactiva el mensaje de error
                        panel2Error.SetActive(false);

                        // Si el estado actual es el panel 2 (ya que se puede volver a pulsar
                        // la botella aunque este seleccionada), se pasa al estado siguiente
                        if (currState == States.PANEL2)
                        {
                            UpdateState();
                        }

                        // Se usa el update de la clase base
                        base.Update();
                    }
                }
                // Si no, se desactiva el indicador del puntero y el error del
                // panel 2 y se activa el mensaje de que no se ha seleccionado nada
                else if (currState == States.PANEL2)
                {
                    pointerPos.SetActive(false);
                    panel2Error.SetActive(false);
                    noSelection.SetActive(true);
                }

            }

            //if (currState == States.PANEL5)
            //{
            //    returnButton.GetComponent<Button>().enabled = true;
            //}
        }
        // Si el estado actual supera el numero de paneles indicados, se ejecuta el Update de la clase padre
        else
        {
            base.Update();
        }

    }

    public override void Answer(string answer)
    {
        // Si se responde en el estado del panel 3, se actualiza
        // el estado y se activa el texto con las instrucciones
        if (currState == States.PANEL3)
        {
            UpdateState();
        }

        // En cualquier estado, se ejecuta el Answer de la clase padre
        base.Answer(answer);
    }


    /// <summary>
    /// Actualiza el estado actual, ocultando el panel actual y mostrando el siguiente
    /// </summary>
    void UpdateState(int increment = 1)
    {
        // Oculta el panel del estado actual
        if (ValidState())
        {
            statePanels[(int)currState].SetActive(false);
        }
        // Actualiza el estado
        currState += increment;
        // Muestra el panel del nuevo estado actual
        if (ValidState())
        {
            statePanels[(int)currState].SetActive(true);
        }

        trackerManager.TrySendStatement(CompletableTracker.Instance.Progressed(levelName, COMPLETABLE_TYPE, (float)currState / (int)States.LAST));
    }

    /// <summary>
    /// Devuelve si el estado actual, es valido
    /// </summary>
    bool ValidState()
    {
        return currState >= 0 && (int)currState < statePanels.Length;
    }
}
