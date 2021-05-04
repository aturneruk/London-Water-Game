using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

        private int level = 0;

        public int Level {
            get {
                return level;
            }
            set {
                if (value < 1) {
                    throw new System.ArgumentOutOfRangeException("The reservoir level must be at least 1");
                }
                else if (value < capacities.Length) {
                    level = value;
                }
                else {
                    throw new System.ArgumentOutOfRangeException("The reservoir has reached its maximum level");
                }
            }
        }

        private double MaxDailyCellSupply {
            get {
                if (Storage.Volume < 1e-6 && RemainingRiverAbstractionCapacity < 1e-6) {
                    return Storage.Volume + RemainingRiverAbstractionCapacity;
                }
                else {
                    return maxAbstractions[Level];  //* Storage.Level * Storage.Level;
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
                if (Storage.Volume == 0 && RemainingRiverAbstractionCapacity == 0) {
                    return 0;
                }
                else if (MaxMonthlyCellSupply > Storage.Volume + RemainingRiverAbstractionCapacity) {
                    return Storage.Volume + RemainingRiverAbstractionCapacity;
                }
                else {
                    return MaxMonthlyCellSupply;
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
                return Mathf.Min((float)MaxMonthlyRiverAbstraction, (float)abstractionCell.ReservoirMaxAbstraction);
            }
        }

        public double RemainingRiverAbstractionCapacity {
            get {
                return MaxRiverAbstraction - riverAbstraction.Volume;
            }
        }

        private double[] capacities = {
            0, 5000000, 10000000, 15000000, 20000000, 30000000, 50000000, 100000000, 500000000, 1000000000, 5000000000, 10000000000, 15000000000, 20000000000, 30000000000, 50000000000
        };

        private double[] maxAbstractions = {
            0, 5000, 10000, 15000, 20000, 30000, 50000, 100000, 500000, 1000000, 5000000, 10000000, 15000000, 20000000, 30000000, 50000000
        };

        private double[] maxRefills = {
            0, 50000, 100000, 150000, 200000, 300000, 500000, 1000000, 5000000, 10000000, 50000000, 100000000, 150000000, 200000000, 300000000, 500000000
        };

        private void Awake() {
            //grades = Enumerable.Range(0, capacities.Length + 1).ToArray();

            hexCell = gameObject.GetComponent<HexCell>();
            cellManager = gameObject.GetComponent<CellManager>();
            gridManager = gameObject.GetComponentInParent<GridManager>();

            Level = 1;
            Storage = new Water(0, 1, capacities[level]);
            abstractionCell = cellManager.riverAbstractionCell;
            gridManager.AddReservoir(this);
            hexCell.SetMainColor();

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

        public Water Abstract(double abstraction) {
            if (abstraction <= MaxCellSupply) {
                if (abstraction <= RemainingRiverAbstractionCapacity) {
                    riverAbstraction.Volume += abstraction;
                    return new Water(abstraction, riverAbstraction.Quality);
                }
                else {
                    double riverPortion = RemainingRiverAbstractionCapacity;
                    riverAbstraction.Volume += riverPortion;
                    abstraction -= riverPortion;
                    Storage.Volume -= abstraction;
                    return new Water(riverPortion + abstraction, (riverPortion * riverAbstraction.Quality + abstraction * Storage.Quality) / (riverPortion + abstraction));
                }
            }
            else {
                abstraction = MaxCellSupply;
                if (abstraction <= RemainingRiverAbstractionCapacity) {
                    riverAbstraction.Volume += abstraction;
                    return new Water(abstraction, abstractionCell.flow.Quality);
                }
                else {
                    double riverPortion = RemainingRiverAbstractionCapacity;
                    riverAbstraction.Volume += riverPortion;
                    abstraction -= riverPortion;
                    Storage.Volume -= abstraction;
                    return new Water(riverPortion + abstraction, (riverPortion * riverAbstraction.Quality + abstraction * Storage.Quality) / (riverPortion + abstraction));
                }
            }
        }

        public void SetSupply() {
            riverAbstraction.Volume = 0;
            riverAbstraction.Quality = abstractionCell.flow.Quality;
        }

        public void SetStorage() {
            if (RemainingRiverAbstractionCapacity == 0) {
                double abstraction = MaxRiverAbstraction;
                abstractionCell.ReservoirAbstract(abstraction);
            }
            else if (RemainingRiverAbstractionCapacity <= Storage.RemainingCapacity) {
                double abstraction = MaxRiverAbstraction;
                Water abstracted = abstractionCell.ReservoirAbstract(abstraction);
                Water refilled = abstracted - riverAbstraction.Volume;
                Storage += refilled;
            }
            else if (RemainingRiverAbstractionCapacity > Storage.RemainingCapacity) {
                double abstraction = riverAbstraction.Volume + (double)Storage.RemainingCapacity;
                Water abstracted = abstractionCell.ReservoirAbstract(abstraction);
                Water refilled = abstracted - riverAbstraction.Volume;
                Storage += refilled;
            }
        }

        public void DistributeWater() {

            //foreach (CellManager cellManager in serviceAreaCellManagers) {
            //    cellManager.reservoirSupply += new Water((float)Supply.MaxCapacity, 1);
            //}
        }
    }
}


