using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetropolitanBoroughs : MonoBehaviour {

    public readonly string[] boroughs = {
        "City of London",
        "Battersea",
        "Bermondsey",
        "Bethnal Green",
        "Camberwell",
        "Chelsea",
        "Deptford",
        "Finsbury",
        "Fulham",
        "Greenwich",
        "Hackney",
        "Hammersmith",
        "Hampstead",
        "Holborn",
        "Islington",
        "Kensington",
        "Lambeth",
        "Lewisham",
        "Paddington",
        "Poplar",
        "St Marylebone",
        "St Pancras",
        "Shoreditch",
        "Southwark",
        "Stepney",
        "Stoke Newington",
        "Wandsworth",
        "Westminster",
        "Woolwich"
    };

    private int[][] cells = {
        new int[] { 792, 793, 794 },
        new int[] {  1 }
    };

    private void Start() {
        for (int i = 0; i < cells.Length; i++) {
            foreach (int j in cells[i]) {
                HexCell cell = gameObject.GetComponent<HexGrid>().GetCellFromIndex(j);
                cell.borough = i;
            }
        }
    }

}
