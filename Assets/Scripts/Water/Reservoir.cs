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

        private int grade;

        public Water Supply;

        public int Grade {
            get {
                return grade;
            }
            set {
                if (value < 1) {
                    throw new System.ArgumentOutOfRangeException("The reservoir level must be at least 1");
                }
                else if (value <= capacities.Length + 1) {
                    grade = value;
                }
                else {
                    throw new System.ArgumentOutOfRangeException("The reservoir has reached its maximum level");
                }
            }
        }

        //private int[] grades;

        private long[] capacities = {
            5000000, 10000000, 15000000, 20000000, 30000000, 50000000, 100000000, 500000000, 1000000000, 5000000000, 10000000000, 15000000000, 20000000000, 30000000000, 50000000000
        };

        private void Awake() {
            //grades = Enumerable.Range(0, capacities.Length + 1).ToArray();

            hexCell = gameObject.GetComponent<HexCell>();
            cellManager = gameObject.GetComponent<CellManager>();
            gridManager = gameObject.GetComponentInParent<GridManager>();

            hexCell.SetMainColor();

            Grade = 1;

            CalculateServiceArea();

            gridManager.AddReservoir(this);

            Supply = new Water(0, 1, capacities[grade - 1]);
        }

        public void UpgradeReservoir() {
            Grade++;
            Supply.MaxCapacity = capacities[grade - 1];
            CalculateServiceArea();
        }

        public void CalculateServiceArea() {

            serviceArea.Clear();
            List<HexCell> cells = new List<HexCell> { hexCell };
            List<HexCell> newCells = new List<HexCell>();

            for (int i = 0; i < Grade; i++) {
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

        public Water Abstract(Water demand) {

            if (demand.Volume <= Supply.MaxCapacity) {
                return new Water((float)demand.Volume, Supply.Quality);
            }
            else {
                return new Water((float)Supply.MaxCapacity, Supply.Quality);
            }
        }

        public void DistributeWater() {

            //foreach (CellManager cellManager in serviceAreaCellManagers) {
            //    cellManager.reservoirSupply += new Water((float)Supply.MaxCapacity, 1);
            //}
        }
    }
}


