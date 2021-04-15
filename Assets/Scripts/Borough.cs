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

        try {
            cellPopulation = Mathf.RoundToInt((float)population / (float)cells.Length);
        }
        catch (System.Exception e) {
            Debug.Log(population.ToString() + cells.Length.ToString());
            Debug.Log(e.ToString());
            cellPopulation = 0;
        }


        //int cellPopulation = Mathf.RoundToInt((float)population / (float)cells.Length);

        foreach (HexCell cell in cells) {
            cell.borough = this;
            cell.Color = Color.grey;
            cell.Population.Size = cellPopulation;
        }
    }

    public string Population() {
        float boroughPopulation = 0;

        foreach (HexCell cell in Cells) {
            boroughPopulation += cell.Population.Size;
        }

        try {
            return Mathf.RoundToInt(boroughPopulation).ToString();
        }
        catch (System.Exception e) {
            Debug.Log(boroughPopulation.ToString() + " in borough " + Name);
            Debug.Log(e.ToString());
            return Mathf.Round(boroughPopulation).ToString();
        }
    }

    public override string ToString() {
        return Name;
    }
}
