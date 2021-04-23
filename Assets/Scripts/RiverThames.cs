using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Water {
    public class RiverThames : MonoBehaviour {

        private HexGrid hexGrid;
        private List<HexCell> hexCells = new List<HexCell>();
        private List<RiverCell> riverCells = new List<RiverCell>();
        public int RiverLength { get; private set; }

        [SerializeField]
        UnityEngine.UI.Slider inflowSlider;

        private void Awake() {

            hexGrid = gameObject.GetComponent<HexGrid>();

            CreateRiverCells();
            GenerateNetwork();
        }

        private void OnEnable() {
            GameTime.NewDay += DailyRefresh;
        }

        private void OnDisable() {
            GameTime.NewDay -= DailyRefresh;
        }

        private void CreateRiverCells() {

            for (int i = 0; i < cellIndices.Length; i++) {

                int hexCellIndex = cellIndices[i];
                HexCell hexCell = hexGrid.GetCellFromIndex(hexCellIndex);
                hexCells.Add(hexCell);
                hexCell.riverDistance = 0;

                RiverCell riverCell = hexCell.gameObject.AddComponent<RiverCell>();
                riverCells.Add(riverCell);
                riverCell.river = this;
                riverCell.hexCell = hexCell;
                riverCell.index = i;
                if (i > 0) {
                    riverCell.PreviousCell = riverCells[i - 1];
                    riverCells[i - 1].NextCell = riverCell;
                }
            }

            RiverLength = riverCells.Count;
        }

        public void GenerateNetwork() {

            GenerateNetwork2();

            // Set discharge cell
            for (int i = 0; i < RiverLength; i++) {

                for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++) {

                    HexCell neighbor = hexCells[i].GetNeighbor(d);

                    if (neighbor && neighbor.riverDistance == 1) {
                        neighbor.dischargeCell = hexCells[i];
                    }
                }
            }

            // Set abstraction cell
            for (int i = RiverLength - 1; i >= 0; i--) {

                for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++) {

                    HexCell neighbor = hexCells[i].GetNeighbor(d);

                    if (neighbor && neighbor.riverDistance == 1) {
                        neighbor.abstractionCell = hexCells[i];
                    }
                }
            }
        }

        private void GenerateNetwork2() {

            List<HexCell> currentCells = new List<HexCell>(hexCells);
            List<HexCell> nextCells = new List<HexCell>();

            System.Random rng = new System.Random();

            int level = 1;
            int changed = 0;

            while (true) {

                foreach (HexCell cell in currentCells) {

                    for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++) {

                        HexCell neighbor = cell.GetNeighbor(d);

                        if (neighbor && neighbor.riverDistance == null) {
                            neighbor.riverDistance = level;
                            changed++;
                            nextCells.Add(neighbor);

                            if (level > 1) {
                                bool set = false;

                                while (!set) {
                                    int random = rng.Next(6);
                                    HexCell neighbor1 = neighbor.GetNeighbor((HexDirection)random);
                                    if (neighbor1 && neighbor1.riverDistance == level - 1) {
                                        neighbor.dischargeCell = neighbor1;
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

                level++;
                changed = 0;
                currentCells.Clear();
                currentCells.AddRange(nextCells);
                nextCells.Clear();
            }
        }

        private float inflowValueScalar = 1;

        public Water GetInflow(RiverCell riverCell) {

            switch (riverCell.index) {
                case 0:
                    return new Water(5200000000 * inflowValueScalar, 1);
                default:
                    return new Water(0, 1);
            }
        }

        public void UpdateSlider() {
            inflowValueScalar = inflowSlider.value;
        }

        bool toggle = false;

        private void DailyRefresh() {

            toggle ^= true;

            foreach (RiverCell riverCell in riverCells) {
                riverCell.UpdateFlow(toggle);
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



