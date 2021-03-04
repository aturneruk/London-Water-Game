using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Population : MonoBehaviour
{
    public void GrowPopulation(HexCell cell) {
        cell.population = cell.population + (cell.population * cell.popGrowthRate);
        cell.population = Mathf.RoundToInt(cell.population);
    }
}
