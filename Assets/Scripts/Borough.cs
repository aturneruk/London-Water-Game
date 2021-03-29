using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Borough {

    public HexCell[] Cells { get; }
    public string Name { get; }

    public Borough(HexCell[] cells, string name) {
        Cells = cells;
        Name = name;

        foreach (HexCell cell in cells) {
            cell.borough = this;
        }
    }

    public override string ToString() {
        return Name;
    }
}
