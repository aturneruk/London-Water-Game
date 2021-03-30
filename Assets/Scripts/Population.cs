using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Population {

    HexCell cell;
    public int Size { get; private set; }
    public float GrowthRate { get; private set; }

    public Population(HexCell cell) {
        this.cell = cell;
        Size = 0;
        GrowthRate = 0;
    }

    public void SetInitialPopulation(int size, float growthRate) {
        Size = size;
        GrowthRate = growthRate;
    }

    public override string ToString() {
        return Size.ToString();
    }


    public void GrowPopulation() {

    }
}
