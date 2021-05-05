using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI {
    public class InfoBox : MonoBehaviour {

        public HexGrid hexGrid;

        CanvasGroup activeInfoBox;

        [SerializeField]
        CanvasGroup CellInfoBox;
        [SerializeField]
        Text hexCellIndex, cellPopulation, cellBorough, boroughPopulation, waterDemand, groundwaterSupply, groundwaterLevel, groundwaterQuality, riverSupply, reservoirSupply,  waterSupply;

        [SerializeField]
        CanvasGroup RiverInfoBox;
        [SerializeField]
        Text hexCellIndexRiver, riverCellIndex, riverFlow, riverQuality, riverAbstraction, riverDischarge, dischargeQuality;

        [SerializeField]
        CanvasGroup EmptyInfoBox;
        [SerializeField]
        Text emptyCellIndex, emptyCellPopulation, newReservoir, newWWTP;

        [SerializeField]
        CanvasGroup ReservoirInfoBox;
        [SerializeField]
        Text reservoirCellIndex, reservoirLevel, reservoirStorageCapacity, reservoirStorageLevel, reservoirStorageQuality, reservoirAbstractionVolume, reservoirSupplyVolume, reservoirSupplyMultiplier, reservoirUpgrade;
        [SerializeField]
        Slider reservoirSupplyMultiplierSlider;

        [SerializeField]
        CanvasGroup WWTPInfoBox;
        [SerializeField]
        Text wwtpCellIndex, wwtpLevel, wwtpCapacity, wwtpUpgrade;

        private HexCell selectedCell;

        void Start() {
            CloseAllWindows();
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
                        HideWindow(activeInfoBox);
                    }

                    selectedCell = newCell;

                    if (selectedCell.riverDistance != 0) {
                        if (selectedCell.Population.Size != 0) {
                            activeInfoBox = CellInfoBox;
                            SelectCell(selectedCell);
                            ShowWindow();
                        }
                        else if (selectedCell.GetComponent<Water.Reservoir>() != null) {
                            activeInfoBox = ReservoirInfoBox;
                            SelectCell(selectedCell);
                            ShowWindow();
                        }
                        else if (selectedCell.GetComponent<Water.WWTP>() != null) {
                            activeInfoBox = WWTPInfoBox;
                            SelectCell(selectedCell);
                            ShowWindow();
                        }
                        else {
                            activeInfoBox = EmptyInfoBox;
                            SelectCell(selectedCell);
                            ShowWindow();
                        }

                    }
                    else if (selectedCell.riverDistance == 0) {
                        activeInfoBox = RiverInfoBox;
                        SelectCell(selectedCell);
                        ShowWindow();
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
                if (activeInfoBox) {
                    HideWindow(activeInfoBox);
                }
            }
        }

        public void CloseButton() {
            DeselectCell(selectedCell);
            HideWindow(activeInfoBox);
            selectedCell = null;
        }

        public void BuildReservoir() {
            HideWindow(EmptyInfoBox);
            activeInfoBox = ReservoirInfoBox;
            selectedCell.BuildReservoir();
            SelectCell(selectedCell);
            ShowWindow();
        }

        public void UpgradeReservoir() {
            selectedCell.GetComponent<Water.Reservoir>().UpgradeReservoir();
            SelectCell(selectedCell);
        }

        public void BuildWWTP() {
            HideWindow(EmptyInfoBox);
            activeInfoBox = WWTPInfoBox;
            ShowWindow();
        }

        public void UpgradeWWTP() {
            //TODO
        }

        private void CloseAllWindows() {
            HideWindow(CellInfoBox);
            HideWindow(RiverInfoBox);
            HideWindow(EmptyInfoBox);
            HideWindow(ReservoirInfoBox);
            HideWindow(WWTPInfoBox);
        }

        private void ShowWindow() {
            activeInfoBox.alpha = 1f;
            activeInfoBox.interactable = true;
            activeInfoBox.blocksRaycasts = true;
        }

        private void HideWindow(CanvasGroup canvasGroup) {
            if (canvasGroup == activeInfoBox) {
                activeInfoBox = null;
            }
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        void SelectCell(HexCell cell) {
            if (activeInfoBox == CellInfoBox) {
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
            else if (activeInfoBox == ReservoirInfoBox) {
                cell.HighlightColor = HexMetrics.selectedColor;
                Water.Reservoir reservoir = selectedCell.gameObject.GetComponent<Water.Reservoir>();
                foreach (HexCell areaCell in reservoir.serviceArea) {
                    areaCell.HighlightColor = Color.red;
                }
            }
            else {
                cell.HighlightColor = HexMetrics.selectedColor;
            }
            selectedCell = cell;
        }

        void DeselectCell(HexCell cell) {
            if (activeInfoBox == CellInfoBox) {
                Borough borough = cell.borough;

                if (borough.Cells != null) {
                    foreach (HexCell boroughCell in borough.Cells)
                        boroughCell.HighlightColor = null;
                }
                else {
                    cell.HighlightColor = null;
                }
            }
            else if (activeInfoBox == ReservoirInfoBox) {
                cell.HighlightColor = null;
                Water.Reservoir reservoir = selectedCell.gameObject.GetComponent<Water.Reservoir>();
                foreach (HexCell areaCell in reservoir.serviceArea) {
                    areaCell.HighlightColor = null;
                }
            }
            else {
                cell.HighlightColor = null;
            }
            selectedCell = null;
        }

        void UpdateInfo() {

            if (activeInfoBox == CellInfoBox) {
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

                waterDemand.text = "Demand: " + selectedCell.waterManager.Demand.FormattedFlow;

                groundwaterSupply.text = "Supplied:" + selectedCell.waterManager.groundwaterSupply.FormattedFlow;
                groundwaterLevel.text = "Remaining: " + selectedCell.waterManager.groundwater.Storage.FormattedLevel;
                groundwaterQuality.text = "Quality: " + selectedCell.waterManager.groundwater.Storage.FormattedQuality;
                riverSupply.text = "Supplied: " + selectedCell.waterManager.riverSupply.FormattedFlow;

                reservoirSupply.text = "Supplied: " + selectedCell.waterManager.reservoirSupply.FormattedFlow;

                waterSupply.text = "Total supplied: " + selectedCell.waterManager.Supply.FormattedFlow;
            }
            else if (activeInfoBox == EmptyInfoBox) {
                emptyCellIndex.text = "Cell " + selectedCell.index.ToString();
                emptyCellPopulation.text = "Population: " + selectedCell.Population.ToString();
                if (selectedCell.GetComponent<Water.Reservoir>() == null) {
                    newReservoir.text = "0 → 1";
                }

                if (selectedCell.GetComponent<Water.WWTP>() == null) {
                    newWWTP.text = "0 → 1";
                }
            }
            else if (activeInfoBox == RiverInfoBox) {

                Water.RiverCell riverCell = selectedCell.gameObject.GetComponent<Water.RiverCell>();

                hexCellIndexRiver.text = "Cell " + selectedCell.index.ToString();
                riverCellIndex.text = "River cell: " + riverCell.index.ToString();

                riverFlow.text = "Flow: " + riverCell.flow.FormattedFlow; ;
                riverQuality.text = "River Quality: " + riverCell.flow.FormattedQuality;

                riverAbstraction.text = "Abstraction: " + riverCell.Abstractions.FormattedFlow;
                riverDischarge.text = "Discharge: " + riverCell.Discharges.FormattedFlow;
                dischargeQuality.text = "Discharge quality: " + riverCell.Discharges.FormattedQuality;
            }
            else if (activeInfoBox == ReservoirInfoBox) {

                Water.Reservoir reservoir = selectedCell.gameObject.GetComponent<Water.Reservoir>();

                reservoirCellIndex.text = "Cell " + selectedCell.index.ToString();
                reservoirLevel.text = "Level " + reservoir.Level + " reservoir";
                reservoirStorageCapacity.text = "Capacity: " + reservoir.Storage.FormattedMaxVolume;
                reservoirStorageLevel.text = "Remaining: " + reservoir.Storage.FormattedLevel;
                reservoirStorageQuality.text = "Quality: " + reservoir.Storage.FormattedQuality;
                reservoirAbstractionVolume.text = "Abstraction: " + reservoir.RiverAbstraction.FormattedFlow;
                reservoirSupplyVolume.text = "Supply: " + reservoir.CellSupply.FormattedFlow;
                reservoirUpgrade.text = reservoir.Level + " → " + (reservoir.Level + 1);

                reservoir.supplyMultiplier = reservoirSupplyMultiplierSlider.value;
                reservoirSupplyMultiplier.text = reservoir.supplyMultiplier.ToString("P0");
            }
            else if (activeInfoBox == WWTPInfoBox) {
            }
        }
    }
}

