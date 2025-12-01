using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectGamemode : BaseGamemode
{
    /// <summary>
    /// Numero maximo de columnas de botones de respuesta
    /// </summary>
    const int MAX_COLS = 2,
    /// <summary>
    /// Numero maximo de filas de botones de respuesta
    /// </summary>
    MAX_ROWS = 3;

    /// <summary>
    /// Numero maximo de botones que puede haber por pagina
    /// </summary>
    const int WORDS_PER_PAGE = MAX_COLS * MAX_ROWS;

    /// <summary>
    /// Objeto padre de todos los elementos de la lista de opciones
    /// </summary>
    [SerializeField]
    GameObject elementsObj,
    /// <summary>
    /// Objeto padre de las filas de opciones (es el que define el layout vertical)
    /// </summary>
    optionsRows,
    /// <summary>
    /// Prefab de cada fila de opciones (es el que define el layout horizontal)
    /// </summary>
    rowPrefab,
    /// <summary>
    /// Prefab de cada boton de opciones
    /// </summary>    
    optionPrefab,
    /// <summary>
    /// Objeto con el boton de la flecha de navegacion izquierda
    /// </summary>
    navigationArrowLeft,
    /// <summary>
    /// Objeto con el boton de la flecha de navegacion derecha
    /// </summary>
    navigationArrowRight;

    /// <summary>
    /// RectTransform de elementsObj para forzar su actualizacion
    /// </summary>
    RectTransform elementsObjTr,
    /// <summary>
    /// RectTransform de optionsRows para forzar su actualizacion
    /// </summary>
    rowsLayoutTr;


    /// <summary>
    /// Array con todos los objetos de los botones de opciones
    /// </summary>
    GameObject[] optionsButtons = new GameObject[WORDS_PER_PAGE];

    /// <summary>
    /// Array con el texto de cada boton
    /// </summary>
    TextMeshProUGUI[] buttonsText = new TextMeshProUGUI[WORDS_PER_PAGE];

    /// <summary>
    /// "Pagina" actual de la lista de opciones
    /// </summary>
    int currPage = 0,

    /// <summary>
    /// "Paginas" maximas que tiene la lista de opciones
    /// </summary>
    totalPages = 0;
    

    // Start is called before the first frame update
    void Start()
    {
        // Se ocultan los elementos de la interfaz
        elementsObj.SetActive(false);
        navigationArrowLeft.SetActive(false);
        navigationArrowRight.SetActive(false);

        elementsObjTr = elementsObj.GetComponent<RectTransform>();
        rowsLayoutTr = optionsRows.GetComponent<RectTransform>();

        // Se rellenan las filas de botones
        for (int i = 0; i < MAX_ROWS; i++)
        {
            GameObject col = Instantiate(rowPrefab, optionsRows.transform);

            // Se rellenan las columnas de botones
            for (int j = 0; j < MAX_COLS; j++)
            {
                int index = (i * MAX_COLS) + j;
                GameObject button = Instantiate(optionPrefab, col.transform);
                optionsButtons[index] = button;
                buttonsText[index] = button.GetComponentInChildren<TextMeshProUGUI>();

                // Se anade a cada boton el callback (como son prefabs y necesitan
                // usar scripts de otros objetos, no puede hacerse desde el editor)
                button.GetComponent<Button>().onClick.AddListener(() =>
                {
                    SendAnswer(buttonsText[index].text);
                });
            }
        }

    }


    public override void OnItemSelected(Collider2D[] items)
    {
        base.OnItemSelected(items);

        if (items.Length > 0)
        {
            // Se reinicia la pagina actual y se calcula el nuevo numero de paginas
            currPage = 0;
            totalPages = possibleWords.Count() / WORDS_PER_PAGE;
        }
    }

    public override void ShowElements() 
    {
        // Se activa el objeto con todos los elementos
        elementsObj.SetActive(true);

        // Se randomiza el orden de las palabras y muestra los botones
        RandomizeList(possibleWords);
        ShowOptions();
    }

    public override void HideElements() 
    {
        // Desactiva los botones de respuesta y de navegacion
        foreach (GameObject optionButton in optionsButtons)
        {
            optionButton.SetActive(false);
        }
        navigationArrowLeft.SetActive(false);
        navigationArrowRight.SetActive(false);
    }

    /// <summary>
    /// Randomiza la lista que se pasa como parametro usando el algoritmo Fisher-Yates
    /// </summary> 
    private void RandomizeList(List<string> l)
    {
        int n = l.Count;
        var rng = new System.Random();
        while (n > 1)
        {
            int k = rng.Next(n--);
            string temp = l[n];
            l[n] = l[k];
            l[k] = temp;
        }
    }


    /// <summary>
    /// Muestra las siguientes WORDS_PER_PAGE opciones
    /// </summary> 
    public void ChangeOptionsPage(int increment)
    {
        currPage += increment;

        if (currPage < 0)
        {
            currPage = 0;
        }
        else if (currPage > totalPages)
        {
            currPage = totalPages;
        }
        ShowOptions();
    }


    /// <summary>
    /// Muestra las opciones en los botones
    /// </summary> 
    private void ShowOptions()
    {
        // Si esta en cualquier pagina que no sea la primera, se muestra el boton izquierdo y si esta
        // en cualquier pagina que no sea la ultima y hay mas paginas, se muestra el boton derecho
        navigationArrowLeft.SetActive(currPage > 0);
        navigationArrowRight.SetActive(currPage < totalPages && possibleWords.Count() > WORDS_PER_PAGE);

        // Oculta todos los botones
        foreach (GameObject optionButton in optionsButtons)
        {
            optionButton.SetActive(false);
        }

        // Recorre WORDS_PER_PAGE palabras de la pagina y muestra los botones con la palabra
        for (int i = WORDS_PER_PAGE * currPage; i < possibleWords.Count() && i < WORDS_PER_PAGE * (currPage + 1); i++)
        {
            int j = i % optionsButtons.Count();
            optionsButtons[j].SetActive(true);
            buttonsText[j].text = possibleWords[i];
        }
        // Fuerza la actualizacion del layout (por si acaso se desplazan los botones)
        LayoutRebuilder.ForceRebuildLayoutImmediate(elementsObjTr);
        LayoutRebuilder.ForceRebuildLayoutImmediate(rowsLayoutTr);
    }
}
