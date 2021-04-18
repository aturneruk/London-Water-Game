using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI {
    public class CellInfoBox : MonoBehaviour {

        public HexGrid hexGrid;

        [SerializeField]
        Text cellIndex, cellPopulation, cellBorough, boroughPopulation;

        [SerializeField]
        Text waterDemand, waterSupply, groundwaterLevel, groundwaterQuality;

        CanvasGroup canvasGroup;

        private HexCell selectedCell;

        void Start() {
            canvasGroup = gameObject.GetComponent<CanvasGroup>();
            HideWindow();
        }

        // Update is called once per frame
        void Update() {
            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()) {
                HandleInput();
            }
            if (selectedCell) {
                UpdateInfo();
            }
        }

        private void HandleInput() {
            Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(inputRay, out RaycastHit hit)) {

                HexCell newCell = hexGrid.GetCellFromPosition(hit.point);

                if (selectedCell != newCell) {

                    if (selectedCell) {
                        DeselectCell(selectedCell);
                    }

                    selectedCell = newCell;

                    if (selectedCell.isThames != true) {
                        SelectCell(selectedCell);
                        ShowWindow();
                    }
                    else {
                        SelectCell(selectedCell);
                        HideWindow();
                    }
                }
            }
            else {
                if (selectedCell) {
                    DeselectCell(selectedCell);
                }
                selectedCell = null;
                HideWindow();
            }
        }

        public void CloseButton() {
            HideWindow();
            DeselectCell(selectedCell);
        }

        private void ShowWindow() {
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }

        private void HideWindow() {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        void DeselectCell(HexCell cell) {

            Borough borough = cell.borough;

            if (borough.Cells != null) {
                foreach (HexCell boroughCell in borough.Cells)
                    boroughCell.SetDefaultColor();
            }
            else {
                cell.SetDefaultColor();
            }
        }

        void SelectCell(HexCell cell) {
            cell.Color = HexGrid.touchedColor;
            Borough borough = cell.borough;

            if (borough.Cells != null) {
                foreach (HexCell boroughCell in borough.Cells) {
                    if (boroughCell != cell) {
                        boroughCell.Color = Color.yellow;
                    }
                }
            }
        }

        void UpdateInfo() {
            cellIndex.text = "Cell " + selectedCell.index.ToString();
            cellPopulation.text = "Population: " + selectedCell.Population.ToString();

            if (selectedCell.borough.Name != null) {
                cellBorough.text = selectedCell.borough.ToString();
                boroughPopulation.text = "Population: " + selectedCell.borough.Population();
            }
            else {
                cellBorough.text = null;
                boroughPopulation.text = null;
            }

            waterDemand.text = "Demand: " + selectedCell.waterManager.FormattedDemand + " L/day";
            waterSupply.text = "Supply: " + selectedCell.waterManager.FormattedSupply + " L/day";
            groundwaterLevel.text = "Groundwater remaining: " + selectedCell.waterManager.groundwater.Level.ToString("P2");
            groundwaterQuality.text = "Groundwater quality: " + selectedCell.waterManager.groundwater.Storage.Quality.ToString("P2");
        }
    }
}

