using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace Water {

    public class Reservoir : MonoBehaviour {

        private HexCell hexCell;
        private CellManager cellManager;
        private GridManager gridManager;

        public List<HexCell> serviceArea = new List<HexCell>();
        private List<CellManager> serviceAreaCellManagers = new List<CellManager>();

        private RiverCell abstractionCell;

        public Water Storage;
        private Water riverAbstraction;

        public Water SuppliedToCells;
        public Water AbstractedFromRiver;

        public double requestedStorageLevel;
        public double maxCellSupplyMultiplier;
        public string displayedMaxCellSupply;

        private int level = 0;

        public int Level {
            get {
                return level;
            }
            set {
                if (value < 0) {
                    throw new ArgumentOutOfRangeException("The reservoir level must be at least 0");
                }
                else if (value < capacities.Length) {
                    level = value;
                }
                else {
                    throw new ArgumentOutOfRangeException("The reservoir has reached its maximum level");
                }
            }
        }

        private double MaxDailyCellSupply {
            get {
                if (Storage.Volume < 1e-6 && RemainingRiverAbstractionCapacity < 1e-6) {
                    return Storage.Volume + RemainingRiverAbstractionCapacity;
                }
                else {
                    return maxAbstractions[Level] * maxCellSupplyMultiplier;  //* Storage.Level * Storage.Level;
                }
            }
        }

        private double MaxMonthlyCellSupply {
            get {
                return MaxDailyCellSupply * 30;
            }
        }

        public double MaxCellSupply {
            get {
                double fillToRequestedLevel = (double)Storage.MaxCapacity * requestedStorageLevel - Storage.Volume;

                if (Storage.Volume == 0 && RemainingRiverAbstractionCapacity == 0) {
                    return 0;
                }
                else if (fillToRequestedLevel >= RemainingRiverAbstractionCapacity) {
                    return 0;
                }
                else if (fillToRequestedLevel > 0) {
                    return Math.Min(RemainingRiverAbstractionCapacity - fillToRequestedLevel, MaxMonthlyCellSupply);
                }
                else {
                    return Math.Min(RemainingRiverAbstractionCapacity + Storage.Volume - (double)Storage.MaxCapacity * requestedStorageLevel, MaxMonthlyCellSupply);
                }
            }
        }

        private double MaxDailyRiverAbstraction {
            get {
                return maxRefills[Level];
            }
        }

        private double MaxMonthlyRiverAbstraction {
            get {
                return MaxDailyRiverAbstraction * 30;
            }
        }

        public double MaxRiverAbstraction {
            get {
                if (abstractionCell) {
                    return Math.Min(MaxMonthlyRiverAbstraction, abstractionCell.ReservoirMaxAbstraction);
                }
                else {
                    return 0;
                }
            }
        }

        public double RemainingRiverAbstractionCapacity {
            get {
                if (MaxRiverAbstraction - riverAbstraction.Volume < 0) {
                    throw new ArgumentOutOfRangeException("The remaining river abstraction capacity must be nonnegative");
                }
                return MaxRiverAbstraction - riverAbstraction.Volume;
            }
        }

        public double CapitalUpgradeCost {
            get {
                return capacities[level + 1] / 20d;
            }
        }

        public double UpgradeCost {
            get {
                return CapitalUpgradeCost;
            }
        }

        public static double CapitalBuildCost {
            get {
                return 250000d;
            }
        }

        public static double PopulationBuildCost(HexCell cell) {            
            if (cell.cellPopulation && cell.cellPopulation.Size != 0) {
                return cell.cellPopulation.Size * 10;
            }
            else {
                return 0;
            }            
        }

        public static double BuildCost(HexCell cell) {
            return CapitalBuildCost + PopulationBuildCost(cell);
        }

        public static readonly double[] capacities = {
            0, 5000000, 10000000, 15000000, 20000000, 30000000, 50000000, 100000000, 500000000, 1000000000, 5000000000, 10000000000, 15000000000, 20000000000, 30000000000, 50000000000
        };

        private static readonly double[] maxAbstractions = {
            0, 5000, 10000, 15000, 20000, 30000, 50000, 100000, 500000, 1000000, 5000000, 10000000, 15000000, 20000000, 30000000, 50000000
        };

        private static readonly double[] maxRefills = {
            0, 50000, 100000, 150000, 200000, 300000, 500000, 1000000, 5000000, 10000000, 50000000, 100000000, 150000000, 200000000, 300000000, 500000000
        };

        private void Awake() {
            hexCell = gameObject.GetComponent<HexCell>();
            cellManager = gameObject.GetComponent<CellManager>();
            gridManager = gameObject.GetComponentInParent<GridManager>();

            hexCell.reservoir = this;

            Level = 1;
            Storage = new Water(0, 0, capacities[level]);
            abstractionCell = cellManager.riverAbstractionCell;
            gridManager.AddReservoir(this);
            hexCell.SetMainColor();
            hexCell.cellPopulation = null;
            Destroy(hexCell.GetComponent<CellPopulation>());

            CalculateServiceArea();            
        }

        public void UpgradeReservoir() {
            Level++;
            Storage.MaxCapacity = capacities[level];
            CalculateServiceArea();            
        }

        public void CalculateServiceArea() {
            serviceArea.Clear();
            List<HexCell> cells = new List<HexCell> { hexCell };
            List<HexCell> newCells = new List<HexCell>();

            for (int i = 0; i < Level * 2; i++) {
                foreach (HexCell cell in cells) {
                    for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++) {
                        HexCell neighbor = cell.GetNeighbor(d);
                        if (neighbor && neighbor.riverDistance != 0) {
                            if (!newCells.Contains(neighbor) && !cells.Contains(neighbor) && !serviceArea.Contains(neighbor) && neighbor != hexCell) {
                                newCells.Add(neighbor);
                            }
                        }
                    }
                }
                cells.Clear();
                cells.AddRange(newCells);
                serviceArea.AddRange(newCells);
                newCells.Clear();
            }

            foreach (HexCell supplyCell in serviceArea) {
                CellManager cellManager = supplyCell.GetComponent<CellManager>();
                serviceAreaCellManagers.Add(cellManager);
                cellManager.reservoirs.Add(this);
            }
        }

        public Water Abstract(double demand) {

            if (demand > MaxCellSupply) {
                demand = MaxCellSupply;
            }

            if (demand <= RemainingRiverAbstractionCapacity) {
                riverAbstraction.Volume += demand;
                SuppliedToCells.Volume += demand;
                return new Water(demand, riverAbstraction.Quality);
            }
            else {
                double riverPortion = RemainingRiverAbstractionCapacity;
                riverAbstraction.Volume += riverPortion;
                demand -= riverPortion;
                Storage.Volume -= demand;
                SuppliedToCells.Volume += (riverPortion + demand);
                return new Water(riverPortion + demand, (riverPortion * riverAbstraction.Quality + demand * Storage.Quality) / (riverPortion + demand));
            }   
        }

        public void SetSupply() {
            SuppliedToCells.Volume = 0;
            riverAbstraction.Volume = 0;
            if (abstractionCell) {
                riverAbstraction.Quality = abstractionCell.flow.Quality;
            }
            else {
                riverAbstraction.Quality = 0;
            }
            displayedMaxCellSupply = Water.FormatFlow(MaxMonthlyCellSupply);
        }

        public void SetStorage() {
            Water abstracted;

            if (MaxRiverAbstraction > 0) {
                if (RemainingRiverAbstractionCapacity <= Storage.RemainingCapacity) {
                    double abstraction = MaxRiverAbstraction;
                    abstracted = abstractionCell.ReservoirAbstract(abstraction);
                    Water refilled = abstracted - riverAbstraction.Volume;
                    Storage += refilled;
                }
                else if (RemainingRiverAbstractionCapacity > Storage.RemainingCapacity) {
                    double abstraction = riverAbstraction.Volume + (double)Storage.RemainingCapacity;
                    abstracted = abstractionCell.ReservoirAbstract(abstraction);
                    Water refilled = abstracted - riverAbstraction.Volume;
                    Storage += refilled;
                }
                else {
                    Debug.LogError("Reservoir.RemainingRiverAbstractionCapacity does not meet one of the required conditions");
                    abstracted = new Water(0, 0);
                }
            }
            else {
                abstracted = new Water(0, 0);
            }

            AbstractedFromRiver = abstracted;
        }
    }
}


