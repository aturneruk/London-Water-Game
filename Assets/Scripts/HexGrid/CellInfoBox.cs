using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CellInfoBox : MonoBehaviour {

    public HexGrid hexGrid;

    [SerializeField]
    Text cellIndex, cellPopulation;

    CanvasGroup canvasGroup;
    bool isOpen;

    void Start() {
        canvasGroup = gameObject.GetComponent<CanvasGroup>();
        Hide();
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()) {
            HandleInput();
        }
    }

    private void HandleInput() {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(inputRay, out hit)) {
            CellInfo(hexGrid.GetCellFromPosition(hit.point));
        }
    }

    private void CellInfo(HexCell cell) {
        cellIndex.text = "Cell " + cell.index.ToString();
        cellPopulation.text = "Population: " + cell.Population.ToString();


        if (isOpen == false) {
            Show();
        }

    }

    public void CloseDiaglogue() {
        Hide();
    }

    private void Hide() {
        isOpen = false;
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    private void Show() {
        isOpen = true;
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }
}
