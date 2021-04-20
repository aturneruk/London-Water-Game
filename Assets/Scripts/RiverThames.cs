using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiverThames : MonoBehaviour {

    private HexGrid hexGrid;
    private List<HexCell> riverCells = new List<HexCell>();
    public int RiverLength { get; private set; }

    private void Start() {
        hexGrid = gameObject.GetComponent<HexGrid>();

        for (int i = 0; i < cellIndices.Length; i++) {

            int hexCellIndex = cellIndices[i];
            HexCell cell = hexGrid.GetCellFromIndex(hexCellIndex);
            cell.riverDistance = 0;

            Water.RiverCell riverCell = cell.gameObject.AddComponent<Water.RiverCell>();
            riverCell.index = i;

            riverCells.Add(cell);
        }

        RiverLength = riverCells.Count;

        GenerateNetwork();
    }

    public void GenerateNetwork() {

        List<HexCell> cells = new List<HexCell>(riverCells);
        List<HexCell> nextCells = new List<HexCell>();

        System.Random rng = new System.Random();

        bool complete = false;
        int level = 1;
        int changed = 0;

        while (!complete) {

            foreach (HexCell cell in cells) {

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
                complete = true;
            }

            level++;
            changed = 0;
            cells.Clear();
            cells.AddRange(nextCells);
            nextCells.Clear();
        }

        // Set discharge cell
        for (int i = 0; i < RiverLength; i++) {
            for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++) {

                HexCell neighbor = riverCells[i].GetNeighbor(d);

                if (neighbor && neighbor.riverDistance == 1) {
                    neighbor.dischargeCell = riverCells[i];                                        
                }
            }
        }

        // Set abstraction cell
        for (int i = RiverLength - 1; i >= 0; i--) {
            for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++) {

                HexCell neighbor = riverCells[i].GetNeighbor(d);

                if (neighbor && neighbor.riverDistance == 1) {
                    neighbor.abstractionCell = riverCells[i];
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


