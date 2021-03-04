using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiverThames : MonoBehaviour {

    public HexGrid hexGrid;

    public float health;

    private static int[,] thamesCellPositions;
    public static HexCell[] thamesCells;


    private void Awake() {
        SetRiverCells();
    }

    private void Start() {
        StartCoroutine(ShowFlow());
    }

    private void SetRiverCells() {
        thamesCellPositions = new int[,] {
            { 0, 4 },
            { 0, 3 },
            { 1, 2 },
            { 1, 1 },
            { 2, 1 },
            { 3, 1 },
            { 4, 2 },
            { 4, 3 },
            { 5, 3 },
            { 6, 3 },
            { 7, 3 },
            { 8, 2 },
            { 8, 1 },
            { 9, 1 },
            { 10, 2 },
            { 10, 3 },
            { 11, 4 },
            { 10, 5 },
            { 9, 5 },
            { 9, 6 },
            { 9, 7 },
            { 10, 7 },
            { 11, 8 },
            { 10, 9 },
            { 9, 9 },
            { 9, 10 },
            { 9, 11 },
            { 10, 12 },
            { 10, 13 },
            { 11, 13 },
            { 12, 12 },
            { 12, 11 },
            { 13, 11 },
            { 14, 12 },
            { 14, 13 },
            { 15, 13 },
            { 16, 12 },
            { 15, 11 },
            { 16, 10 },
            { 17, 10 },
            { 18, 10 },
            { 18, 11 },
            { 19, 12 },
            { 20, 12 },
            { 21, 12 },
            { 22, 12 },
            { 22, 13 },
            { 23, 14 },
            { 23, 15 },
            { 24, 15 },
            { 25, 15 },
            { 26, 15 },
            { 27, 14 },
            { 28, 14 },
            { 28, 15 },
            { 29, 15 },
            { 30, 14 },
            { 29, 13 },
            { 30, 12 },
            { 31, 12 },
            { 32, 12 },
            { 32, 13 },
            { 32, 14 },
            { 32, 15 },
            { 33, 15 },
            { 34, 14 },
            { 35, 14 },
            { 36, 14 },
            { 37, 14 },
            { 37, 15 },
            { 38, 15 },
            { 39, 16 },
            { 40, 16 },
            { 40, 15 },
            { 41, 15 },
            { 42, 14 },
            { 43, 14 },
            { 43, 13 },
            { 44, 12 },
            { 45, 12 },
            { 46, 12 },
            { 46, 11 },
            { 47, 10 },
            { 47, 9 }
        };

        thamesCells = new HexCell[thamesCellPositions.GetLength(0)];

        for (int i = 0; i < thamesCellPositions.GetLength(0); i++) {
            HexCell cell = hexGrid.GetCellFromOffset(thamesCellPositions[i, 0], thamesCellPositions[i, 1]);
            thamesCells[i] = cell;
            // cell.Color = Color.blue;

        }
    }

    private IEnumerator ShowFlow() {

        for(int i = 0; i < thamesCellPositions.GetLength(0); i++) {
            yield return new WaitForSeconds(0.5f);
            thamesCells[i].Color = Color.blue;
        }
    }

}
