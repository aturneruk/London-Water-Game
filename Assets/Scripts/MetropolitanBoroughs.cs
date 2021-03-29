using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetropolitanBoroughs : MonoBehaviour {

    public readonly string[] boroughNames = {
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

    private int[][] cellIndices = {
        new int[] { 792, 793, 794 },
        new int[] { },
        new int[] { },
        new int[] { },
        new int[] { },
        new int[] { },
        new int[] { },
        new int[] { },
        new int[] { },
        new int[] { },
        new int[] { },
        new int[] { },
        new int[] { },
        new int[] { },
        new int[] { },
        new int[] { },
        new int[] { },
        new int[] { },
        new int[] { },
        new int[] { },
        new int[] { },
        new int[] { },
        new int[] { },
        new int[] { },
        new int[] { },
        new int[] { },
        new int[] { },
        new int[] { },
        new int[] { }
    };

    private List<HexCell> cells = new List<HexCell>();
    public List<Borough> boroughs = new List<Borough>();

    private void Start() {

        if (boroughNames.Length != cellIndices.Length) {
            Debug.Log("Borough name list and cell indices list lengths do not match");
        }

        for (int i = 0; i < cellIndices.Length; i++) {
            foreach (int j in cellIndices[i]) {
                Debug.Log(i);
                Debug.Log(j);
                HexCell cell = gameObject.GetComponent<HexGrid>().GetCellFromIndex(j);
                cells.Add(cell);
            }

            boroughs.Add(new Borough(cells.ToArray(), boroughNames[i]));
            cells.Clear();
        }
    }
}
