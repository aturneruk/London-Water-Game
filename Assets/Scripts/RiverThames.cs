using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiverThames : MonoBehaviour {

    private HexGrid hexGrid;

    public static List<HexCell> riverCells = new List<HexCell>();

    public int riverLength;

    public float[] flows;
    public float[] quality;
    public float totalQuality;

    private void Start() {
        hexGrid = gameObject.GetComponent<HexGrid>();

        foreach (int i in cellIndices) {
            HexCell cell = hexGrid.GetCellFromIndex(i);

            cell.gameObject.AddComponent<Water.RiverCell>();

            riverCells.Add(cell);
        }

        riverLength = riverCells.Count;

        Water.FlowNetwork.GenerateNetwork();
        AssignRiversideCells();
    }

    //private void Start() {
    //    // StartCoroutine(ShowFlow());
    //}

    private void AssignRiversideCells() {
        // Set discharge cell
        for (int i = 0; i < riverLength; i++) {
            for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++) {

                HexCell neighbor = riverCells[i].GetNeighbor(d);

                if (neighbor && neighbor.riverDistance == 1) {
                    neighbor.dischargeCell = riverCells[i];
                }
            }
        }

        // Set abstraction cell
        for (int i = riverLength - 1; i >= 0; i--) {
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


