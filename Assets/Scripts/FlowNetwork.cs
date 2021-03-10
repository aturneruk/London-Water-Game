using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FlowNetwork
{
    static List<HexCell> cells = new List<HexCell>();
    static List<HexCell> nextCells = new List<HexCell>();

    public static void BackPass() {

        foreach (HexCell cell in RiverThames.riverCells) {
            cell.riverDistance = 0;
            cells.Add(cell);
        }


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
    }

    public static void ForwardPass() {

    }


}
