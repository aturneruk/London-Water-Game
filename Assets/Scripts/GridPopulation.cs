using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridPopulation : MonoBehaviour
{

    List<CellPopulation> populations = new List<CellPopulation>();
    List<double> populationSizes = new List<double>();

    public int GetTotalPopulation() {

        double totalPopulation = 0;

        foreach (CellPopulation population in populations) {
            totalPopulation += population.Size;
        }


        //double totalPopulation = populationSizes.Sum();
        return Mathf.RoundToInt((float)totalPopulation);
    }

    public double MaxCellPopulation {
        get {
            //double maxPop = 0;
            //foreach (HexCell cell in cells) {
            //    Population population = cell.GetComponent<Population>();
            //    if (population && population.Size > maxPop) {
            //        maxPop = population.Size;
            //    }
            //}
            //return maxPop;
            return 30000d;
        }
    }

    public void AddCellPopulation(HexCell cell) {
        CellPopulation cellPopulation = cell.cellPopulation = cell.gameObject.AddComponent<CellPopulation>();
        cell.cellPopulation.gridPopulation = this;
        cell.cellPopulation.hexCell = cell;
        populations.Add(cellPopulation);
        populationSizes.Add(cellPopulation.Size);
    }
}
