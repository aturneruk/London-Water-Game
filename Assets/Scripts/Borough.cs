using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Borough {

    public HexCell[] Cells { get; }
    public string Name { get; }

    public Borough(string name, HexCell[] cells, int population) {
        Cells = cells;
        Name = name;

        int cellPopulation = Mathf.RoundToInt((float)population / (float)cells.Length);

        foreach (HexCell cell in cells) {
            cell.borough = this;
            cell.Color = Color.grey;
            cell.Population = cellPopulation;
        }
    }

    public override string ToString() {
        return Name;
    }
}
