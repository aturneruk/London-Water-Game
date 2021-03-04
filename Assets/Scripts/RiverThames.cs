using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiverThames : MonoBehaviour {

    public HexGrid hexGrid;

    public float health;

    private static int[,] thamesCellPositions;
    public static HexCell[] thamesCells;


    private void Awake() {
        thamesCellPositions = new int[,] {
            { 0, 20 },
            { 0, 19 }
        };

        for (int i = 0; i < 2; i++) {
            HexCell cell = hexGrid.GetCellFromOffset(thamesCellPositions[i, 0], thamesCellPositions[i, 1]);
            cell.Color = Color.blue;

        }
    }

}
