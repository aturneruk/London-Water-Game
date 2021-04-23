using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI {
    public class InfoBox : MonoBehaviour {

        public HexGrid hexGrid;

        [SerializeField]
        CanvasGroup CellInfoBox;
        [SerializeField]
        Text hexCellIndex, cellPopulation, cellBorough, boroughPopulation, waterDemand, waterSupply, groundwaterLevel, groundwaterQuality;

        [SerializeField]
        CanvasGroup RiverInfoBox;
        [SerializeField]
        Text hexCellIndexRiver, riverCellIndex, riverFlow, riverQuality, riverAbstraction, riverDischarge, dischargeQuality;


        private HexCell selectedCell;

        void Start() {
            HideCellWindow();
            HideRiverWindow();
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
                        HideCellWindow();
                        HideRiverWindow();
                        DeselectCell(selectedCell);
                    }

                    selectedCell = newCell;

                    if (selectedCell.riverDistance != 0) {
                        SelectCell(selectedCell);
                        ShowCellWindow();
                    }
                    else if (selectedCell.riverDistance == 0) {
                        SelectCell(selectedCell);
                        ShowRiverWindow();
                    }
                    else {
                        throw new System.NullReferenceException("The selected cell does not have a river distance set");
                    }
                }
            }
            else {
                if (selectedCell) {
                    DeselectCell(selectedCell);
                }
                selectedCell = null;
                HideCellWindow();
                HideRiverWindow();
            }
        }

        public void CloseButton() {
            HideCellWindow();
            HideRiverWindow();
            DeselectCell(selectedCell);
        }

        private void ShowCellWindow() {
            CellInfoBox.alpha = 1f;
            CellInfoBox.interactable = true;
            CellInfoBox.blocksRaycasts = true;
        }

        private void HideCellWindow() {
            CellInfoBox.alpha = 0f;
            CellInfoBox.interactable = false;
            CellInfoBox.blocksRaycasts = false;
        }

        private void ShowRiverWindow() {
            RiverInfoBox.alpha = 1f;
            RiverInfoBox.interactable = true;
            RiverInfoBox.blocksRaycasts = true;
        }

        private void HideRiverWindow() {
            RiverInfoBox.alpha = 0f;
            RiverInfoBox.interactable = false;
            RiverInfoBox.blocksRaycasts = false;
        }

        void DeselectCell(HexCell cell) {

            Borough borough = cell.borough;

            if (borough.Cells != null) {
                foreach (HexCell boroughCell in borough.Cells)
                    boroughCell.HighlightColor = null;
            }
            else {
                cell.HighlightColor = null;
            }
        }

        void SelectCell(HexCell cell) {
            cell.HighlightColor = HexMetrics.selectedColor;
            Borough borough = cell.borough;

            if (borough.Cells != null) {
                foreach (HexCell boroughCell in borough.Cells) {
                    if (boroughCell != cell) {
                        boroughCell.HighlightColor = HexMetrics.selectedBoroughColor;
                    }
                }
            }
        }

        void UpdateInfo() {

            if (selectedCell.riverDistance != 0) {
                hexCellIndex.text = "Cell " + selectedCell.index.ToString();
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
                groundwaterLevel.text = "Groundwater remaining: " + selectedCell.waterManager.groundwater.Storage.Level.ToString("P2");
                groundwaterQuality.text = "Groundwater quality: " + selectedCell.waterManager.groundwater.Storage.Quality.ToString("P2");
            }
            else if (selectedCell.riverDistance == 0) {

                Water.RiverCell riverCell = selectedCell.gameObject.GetComponent<Water.RiverCell>();

                hexCellIndexRiver.text = "Cell " + selectedCell.index.ToString();
                riverCellIndex.text = "River cell: " + riverCell.index.ToString();

                riverFlow.text = "Flow: " + riverCell.Flow.Volume.ToString();
                riverQuality.text = "River Quality: " + riverCell.Flow.Quality.ToString("P2");

                riverAbstraction.text = "Not implemented yet";
                riverDischarge.text = "Not implemented yet";
                dischargeQuality.text = "Not Implemented yet";
            }

        }
    }
}

