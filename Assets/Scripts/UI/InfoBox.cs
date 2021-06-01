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
        ConfirmationDialog confirmationDialog;

        [SerializeField]
        CanvasGroup CellInfoBox;
        [SerializeField]
        Text hexCellIndex, cellPopulation, cellBorough, boroughPopulation, waterDemand, groundwaterSupply, groundwaterLevel, groundwaterQuality, riverSupply, reservoirSupply,  waterSupply, newPopulatedReservoirLevel, newPopulatedReservoirDetails;
        [SerializeField]
        Button newPopulatedReservoirButton;

        [SerializeField]
        CanvasGroup RiverInfoBox;
        [SerializeField]
        Text hexCellIndexRiver, riverCellIndex, riverFlow, riverQuality, riverAbstraction, riverDischarge, dischargeQuality;

        [SerializeField]
        CanvasGroup EmptyInfoBox;
        [SerializeField]
        Text emptyCellIndex, emptyCellPopulation, newReservoirLevel, newReservoirDetails, newWWTPLevel, newWWTPDetails;
        [SerializeField]
        Button newReservoirButton, newWWTPButton;

        [SerializeField]
        CanvasGroup ReservoirInfoBox;
        [SerializeField]
        Text reservoirCellIndex, reservoirLevel, reservoirStorageCapacity, reservoirStorageLevel, reservoirStorageQuality, reservoirAbstractionVolume, reservoirSupplyVolume, maxCellSupply, reservoirUpgradeLevel, reservoirUpgradeDetails;
        [SerializeField]
        Slider requestedStorageLevelSlider, maxCellSupplyMultiplierSlider;
        [SerializeField]
        Button reservoirUpgradeButton;

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
                        selectedCell = null;
                        HideWindow(activeInfoBox);
                    }

                    selectedCell = newCell;

                    if (selectedCell.riverDistance != 0) {
                        if (selectedCell.cellPopulation && selectedCell.cellPopulation.Size != 0) {
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
                    selectedCell = null;
                }
                if (activeInfoBox) {
                    HideWindow(activeInfoBox);
                }
            }
        }

        public void CloseButton() {
            DeselectCell(selectedCell);
            selectedCell = null;
            HideWindow(activeInfoBox);
            selectedCell = null;
        }

        IEnumerator PlaceBuilding(DialogType dialogType) {

            Water.Reservoir reservoir = selectedCell.GetComponent<Water.Reservoir>();

            if (dialogType == DialogType.EmptyCell) {
                confirmationDialog.ShowConfirmationDialog(dialogType, Water.Reservoir.CapitalBuildCost);
            }
            else if (dialogType == DialogType.PopulatedCell) {
                confirmationDialog.ShowConfirmationDialog(dialogType, Water.Reservoir.CapitalBuildCost, selectedCell.cellPopulation.Size, Water.Reservoir.PopulationBuildCost(selectedCell));
            }
            else if (dialogType == DialogType.Upgrade) {
                confirmationDialog.ShowConfirmationDialog(dialogType, reservoir.UpgradeCost);
            }
            else {
                throw new System.ArgumentNullException("the dialog type cannot be null");
            }

            while (confirmationDialog.result == DialogResult.None) {
                yield return null; // wait
            }

            if (confirmationDialog.result == DialogResult.Yes) {

                if (dialogType == DialogType.EmptyCell) {
                    if (Money.SubtractMoney(Water.Reservoir.BuildCost(selectedCell)) == true) {
                        DeselectCell(selectedCell);
                        HideWindow(EmptyInfoBox);
                        activeInfoBox = ReservoirInfoBox;
                        selectedCell.BuildReservoir();
                        SelectCell(selectedCell);
                        ShowWindow();
                    }
                }
                else if (dialogType == DialogType.PopulatedCell) {
                    if (Money.SubtractMoney(Water.Reservoir.BuildCost(selectedCell)) == true) {
                        DeselectCell(selectedCell);
                        HideWindow(CellInfoBox);
                        activeInfoBox = ReservoirInfoBox;
                        selectedCell.BuildReservoir();
                        SelectCell(selectedCell);
                        ShowWindow();
                    }
                }
                else if (dialogType == DialogType.Upgrade) {
                    if (Money.SubtractMoney(reservoir.UpgradeCost) == true) {
                        reservoir.UpgradeReservoir();
                        SelectCell(selectedCell);
                    }
                }
            }
            else if (confirmationDialog.result == DialogResult.No) {
            }
        }

        public void BuildReservoir() {
            if (activeInfoBox == EmptyInfoBox) {
                StartCoroutine(PlaceBuilding(DialogType.EmptyCell));
            }
            else if (activeInfoBox == CellInfoBox) {
                StartCoroutine(PlaceBuilding(DialogType.PopulatedCell));
            }
        }

        public void UpgradeReservoir() {
            if (activeInfoBox == ReservoirInfoBox) {
                StartCoroutine(PlaceBuilding(DialogType.Upgrade));
            }
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
        }

        void UpdateInfo() {

            if (activeInfoBox == CellInfoBox) {
                hexCellIndex.text = "Cell " + selectedCell.index.ToString();
                cellPopulation.text = "Population: " + selectedCell.cellPopulation.ToString();

                if (selectedCell.borough.Name != null) {
                    cellBorough.text = selectedCell.borough.ToString();
                    boroughPopulation.text = "Population: " + selectedCell.borough.Population();
                }
                else {
                    cellBorough.text = null;
                    boroughPopulation.text = null;
                }

                waterDemand.text = selectedCell.waterManager.Demand.FormattedFlow;

                groundwaterSupply.text = selectedCell.waterManager.groundwaterSupply.FormattedFlow;
                groundwaterLevel.text = selectedCell.waterManager.groundwater.Storage.FormattedLevel;
                groundwaterQuality.text = selectedCell.waterManager.groundwater.Storage.FormattedQuality;
                
                riverSupply.text = selectedCell.waterManager.riverSupply.FormattedFlow;

                reservoirSupply.text = selectedCell.waterManager.reservoirSupply.FormattedFlow;

                waterSupply.text = selectedCell.waterManager.Supply.FormattedFlow;

                newPopulatedReservoirLevel.text = "0 → 1";
                newPopulatedReservoirDetails.text = "Build Reservoir\nCost: " + Money.FormattedMoney(Water.Reservoir.BuildCost(selectedCell)) + "\nNew capacity: " + Water.Water.FormatVolume(Water.Reservoir.capacities[1]);

                if (Water.Reservoir.BuildCost(selectedCell) <= Money.Balance) {
                    newPopulatedReservoirButton.interactable = true;
                }
                else {
                    newPopulatedReservoirButton.interactable = false;
                }
            }
            else if (activeInfoBox == EmptyInfoBox) {
                emptyCellIndex.text = "Cell " + selectedCell.index.ToString();
                emptyCellPopulation.text = "Population: " + selectedCell.cellPopulation.ToString();

                if (selectedCell.GetComponent<Water.Reservoir>() == null) {
                    newReservoirLevel.text = "0 → 1";
                    newReservoirDetails.text = "Build Reservoir\nCost: " + Money.FormattedMoney(Water.Reservoir.BuildCost(selectedCell)) + "\nNew capacity: " + Water.Water.FormatVolume(Water.Reservoir.capacities[1]);
                }

                if (selectedCell.GetComponent<Water.WWTP>() == null) {
                    newWWTPLevel.text = "0 → 1";
                }

                if (Water.Reservoir.BuildCost(selectedCell) <= Money.Balance) {
                    newReservoirButton.interactable = true;
                }
                else {
                    newReservoirButton.interactable = false;
                }
            }
            else if (activeInfoBox == RiverInfoBox) {

                Water.RiverCell riverCell = selectedCell.gameObject.GetComponent<Water.RiverCell>();

                hexCellIndexRiver.text = "Cell " + selectedCell.index.ToString();
                riverCellIndex.text = "River cell: " + riverCell.index.ToString();

                riverFlow.text = riverCell.flow.FormattedFlow; ;
                riverQuality.text = riverCell.flow.FormattedQuality;

                riverAbstraction.text = riverCell.Abstractions.FormattedFlow;
                riverDischarge.text = riverCell.Discharges.FormattedFlow;
                dischargeQuality.text = riverCell.Discharges.FormattedQuality;
            }
            else if (activeInfoBox == ReservoirInfoBox) {

                Water.Reservoir reservoir = selectedCell.gameObject.GetComponent<Water.Reservoir>();

                reservoirCellIndex.text = "Cell " + selectedCell.index.ToString();
                reservoirLevel.text = "Level " + reservoir.Level + " reservoir";
                reservoirStorageCapacity.text = reservoir.Storage.FormattedMaxVolume;
                reservoirStorageLevel.text = reservoir.Storage.FormattedLevel;
                reservoirStorageQuality.text = reservoir.Storage.FormattedQuality;
                reservoirAbstractionVolume.text = reservoir.AbstractedFromRiver.FormattedFlow;
                reservoirSupplyVolume.text = reservoir.SuppliedToCells.FormattedFlow;

                reservoir.requestedStorageLevel = requestedStorageLevelSlider.value;
                reservoir.maxCellSupplyMultiplier = maxCellSupplyMultiplierSlider.value;
                maxCellSupply.text = reservoir.displayedMaxCellSupply;
                
                reservoirUpgradeLevel.text = reservoir.Level + " → " + (reservoir.Level + 1);
                reservoirUpgradeDetails.text = "Upgrade Reservoir\nCost: " + Money.FormattedMoney(reservoir.UpgradeCost) + "\nNew capacity: " + Water.Water.FormatVolume(Water.Reservoir.capacities[reservoir.Level + 1]);

                if (reservoir.UpgradeCost <= Money.Balance) {
                    reservoirUpgradeButton.interactable = true;
                }
                else {
                    reservoirUpgradeButton.interactable = false;
                }
            }
            else if (activeInfoBox == WWTPInfoBox) {
            }
        }
    }
}

