using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Water {
    public class RiverThames : MonoBehaviour {

        private HexGrid hexGrid;
        private GridManager gridManager;

        private List<HexCell> hexCells = new List<HexCell>();
        private List<RiverCell> riverCells = new List<RiverCell>();
        public int RiverLength { get; private set; }

        private double inflowFactor;

        public double inflow;
        public double outflow; 

        private void Awake() {

            hexGrid = gameObject.GetComponent<HexGrid>();
            gridManager = gameObject.GetComponent<GridManager>();

            CreateRiverCells();
            GenerateNetwork();

            UpdateAnnualFlowFactor();
        }

        private void OnEnable() {
            GameTime.NewYear += UpdateAnnualFlowFactor;
        }

        private void OnDisable() {
            GameTime.NewYear -= UpdateAnnualFlowFactor;
        }

        public Water GetInflow(RiverCell riverCell) {
            switch (riverCell.index) {
                case 0:
                    return new Water(65000d * 86400d * 30d * inflowFactor, 1);
                default:
                    return new Water(0, 1);
            }
        }

        public void UpdateAnnualFlowFactor() {
            inflowFactor = Stochasticity.RiverFlowMultiplier;
        }

        private void CreateRiverCells() {

            for (int i = 0; i < cellIndices.Length; i++) {

                int hexCellIndex = cellIndices[i];
                HexCell hexCell = hexGrid.GetCellFromIndex(hexCellIndex);
                hexCells.Add(hexCell);
                hexCell.riverDistance = 0;
                hexCell.riverDistanceSet = true;

                gridManager.RemoveCellManager(hexCell.GetComponent<CellManager>());
                hexCell.cellPopulation = null;
                Destroy(hexCell.GetComponent<CellPopulation>());

                RiverCell riverCell = hexCell.gameObject.AddComponent<RiverCell>();
                riverCells.Add(riverCell);
                riverCell.river = this;
                riverCell.hexCell = hexCell;
                riverCell.index = i;
                gridManager.riverCells.Add(riverCell);
                if (i > 0) {
                    riverCell.PreviousCell = riverCells[i - 1];
                    riverCells[i - 1].NextCell = riverCell;
                }
            }

            RiverLength = riverCells.Count;
        }

        public void GenerateNetwork() {
            SetOverlandFlowCells();
            SetDischargeCells();
            SetAbstractionCells();
        }

        private void SetOverlandFlowCells() {

            List<HexCell> currentCells = new List<HexCell>(hexCells);
            List<HexCell> nextCells = new List<HexCell>();

            System.Random rng = new System.Random();

            int level = 1;
            int changed = 0;

            while (true) {

                foreach (HexCell cell in currentCells) {

                    for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++) {

                        HexCell neighbor = cell.GetNeighbor(d);

                        if (neighbor && neighbor.riverDistanceSet == false) {
                            neighbor.riverDistance = level;
                            neighbor.riverDistanceSet = true;
                            changed++;
                            nextCells.Add(neighbor);

                            if (level > 1) {
                                bool set = false;

                                while (!set) {
                                    int random = rng.Next(6);
                                    HexCell neighbor1 = neighbor.GetNeighbor((HexDirection)random);
                                    if (neighbor1 && neighbor1.riverDistance == level - 1) {
                                        neighbor.waterManager.overlandFlowNext = neighbor1.waterManager;
                                        neighbor1.waterManager.overlandFlowPrevious.Add(neighbor.waterManager);
                                        set = true;
                                    }
                                }
                            }
                        }
                    }
                }

                if (changed == 0) {
                    return;
                }

                gameObject.GetComponent<GridManager>().AddCellManagerLevel(level, nextCells);

                currentCells.Clear();
                currentCells.AddRange(nextCells);
                nextCells.Clear();
                changed = 0;
                level++;
            }
        }

        private void SetDischargeCells() {
            // Discharge cell
            for (int i = 0; i < RiverLength; i++) {

                for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++) {

                    HexCell neighbor = hexCells[i].GetNeighbor(d);

                    if (neighbor && neighbor.riverDistance == 1) {

                        CellManager manager = neighbor.waterManager;

                        if (manager.riverDischargeCell == null) {
                            manager.riverDischargeCell = riverCells[i];
                            riverCells[i].DischargeCells.Add(manager);
                        }
                        else {
                            manager.riverDischargeCell.DischargeCells.Remove(manager);
                            manager.riverDischargeCell = riverCells[i];
                            riverCells[i].DischargeCells.Add(manager);
                        }
                    }
                }
            }
        }

        private void SetAbstractionCells() {        

            // Abstraction cell
            for (int i = RiverLength - 1; i >= 0; i--) {

                for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++) {

                    HexCell neighbor = hexCells[i].GetNeighbor(d);

                    if (neighbor && neighbor.riverDistance == 1) {

                        CellManager manager = neighbor.waterManager;

                        if (manager.riverAbstractionCell == null) {
                            manager.riverAbstractionCell = riverCells[i];
                            riverCells[i].AbstractionCells.Add(manager);
                        }
                        else {
                            manager.riverAbstractionCell.AbstractionCells.Remove(manager);
                            manager.riverAbstractionCell = riverCells[i];
                            riverCells[i].AbstractionCells.Add(manager);
                        }
                    }
                }
            }
        }

        private int[] cellIndices = {
            192,
            144,
            97,
            49,
            50,
            51,
            100,
            148,
            149,
            150,
            151,
            104,
            56,
            57,
            106,
            154,
            203,
            250,
            249,
            297,
            345,
            346,
            395,
            442,
            441,
            489,
            537,
            586,
            634,
            635,
            588,
            540,
            541,
            590,
            638,
            639,
            592,
            543,
            496,
            497,
            498,
            546,
            595,
            596,
            597,
            598,
            646,
            695,
            743,
            744,
            745,
            746,
            747,
            748,
            749,
            702,
            654,
            607,
            608,
            656,
            704,
            752,
            753,
            706,
            707,
            708,
            709,
            757,
            758,
            807,
            808,
            760,
            761,
            714,
            715,
            667,
            620,
            621,
            622,
            574,
            527,
            479
        };
    }
}
