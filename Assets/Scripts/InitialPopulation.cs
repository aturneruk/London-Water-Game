using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitialPopulation : MonoBehaviour
{

    int[] initialPopulations = new int[] {
             5000,
             5000,
             10000
         };


    int[] cellReferences = new int[] {
        599,
        600,
        645
    };


    // Start is called before the first frame update
    void Start()
    {
        SetInitialCellPopulation();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SetInitialCellPopulation() {
        foreach (int i in cellReferences) {
            gameObject.GetComponent<HexGrid>().GetCellFromIndex(i).Population = initialPopulations[i];
        }
    }



}
