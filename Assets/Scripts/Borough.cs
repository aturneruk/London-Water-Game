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
            cell.GetComponent<Population>().Size = cellPopulation;
        }
    }

    public string Population() {
        double boroughPopulation = 0;

        foreach (HexCell cell in Cells) {
            Population cellPopulation = cell.GetComponent<Population>();
            if (cellPopulation) {
                boroughPopulation += cellPopulation.Size;
            }
        }

        return Mathf.RoundToInt((float)boroughPopulation).ToString();
    }

    public override string ToString() {
        return Name;
    }
}
