using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Population : MonoBehaviour
{
    public void GrowPopulation(HexCell cell) {
        cell.Population = cell.Population + (cell.Population * cell.popGrowthRate);
        cell.Population = Mathf.RoundToInt(cell.Population);
    }
}
