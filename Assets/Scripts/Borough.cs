using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Borough {

    public HexCell[] Cells { get; }
    public string Name { get; }

    public Borough(string name, HexCell[] cells, int population) {
        Cells = cells;
        Name = name;

        int cellPopulation;

        if (cells.Length > 0) {
            cellPopulation = Mathf.RoundToInt((float)population / (float)cells.Length);
        }
        else {
            cellPopulation = 0;
        }

        foreach (HexCell cell in cells) {
            cell.borough = this;
            cell.MainColor = Color.grey;
            cell.Population.Size = cellPopulation;
        }
    }

    public string Population() {
        float boroughPopulation = 0;

        foreach (HexCell cell in Cells) {
            boroughPopulation += cell.Population.Size;
        }

        return Mathf.RoundToInt(boroughPopulation).ToString();
    }

    public override string ToString() {
        return Name;
    }
}
