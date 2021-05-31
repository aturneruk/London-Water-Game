using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Borough {

    public HexCell[] Cells { get; }
    public string Name { get; }

    public Borough(string name, HexCell[] cells, double population) {
        Cells = cells;
        Name = name;

        double cellPopulation;

        if (cells.Length > 0) {
            cellPopulation = Math.Round(population / cells.Length);
        }
        else {
            cellPopulation = 0;
        }

        foreach (HexCell cell in cells) {
            cell.borough = this;
            cell.MainColor = Color.grey;
            cell.cellPopulation.Size = cellPopulation;
        }
    }

    public string Population() {
        double boroughPopulation = 0;

        foreach (HexCell cell in Cells) {
            if (cell.cellPopulation) {
                boroughPopulation += cell.cellPopulation.Size;
            }
        }

        return Math.Round(boroughPopulation).ToString();
    }

    public override string ToString() {
        return Name;
    }
}
