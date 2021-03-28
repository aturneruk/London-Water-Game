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

    private HexCell currentCell;

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

            CloseDiaglogue();
            currentCell = hexGrid.GetCellFromPosition(hit.point);

            if (currentCell.isThames != true) {
                CellInfo(currentCell);
            }
            else {
                RiverCellInfo(currentCell);
            }
        }
        else {
            CloseDiaglogue();
        }
    }

    private void CellInfo(HexCell cell) {

        SelectCell(cell);

        cellIndex.text = "Cell " + cell.index.ToString();
        cellPopulation.text = "Population: " + cell.Population.ToString();

        Show();
    }

    private void RiverCellInfo(HexCell cell) {

    }


    public void CloseDiaglogue() {
        Hide();
        if (currentCell) {
            DeselectCell(currentCell);
        }
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

    void DeselectCell(HexCell cell) {
        cell.Color = hexGrid.defaultColor;
    }

    void SelectCell(HexCell cell) {
        cell.Color = hexGrid.touchedColor;
    }

}
