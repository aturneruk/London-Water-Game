using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetropolitanBoroughs : MonoBehaviour {

    private List<HexCell> cells = new List<HexCell>();
    private List<Borough> boroughs = new List<Borough>();

    private void Start() {

        if (boroughNames.Length != cellIndices.Length) {
            Debug.Log("Borough name list and cell indices list lengths do not match");
        }

        for (int i = 0; i < cellIndices.Length; i++) {
            foreach (int j in cellIndices[i]) {
                HexCell cell = gameObject.GetComponent<HexGrid>().GetCellFromIndex(j);
                cells.Add(cell);
            }

            boroughs.Add(new Borough(boroughNames[i], cells.ToArray(), boroughPopulations[i]));
            cells.Clear();
        }
    }

    private string[] boroughNames = {
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
        new int[] { 792, 793, 794 }, // City of London
        new int[] { }, // Battersea
        new int[] { }, // Bermondsey
        new int[] { 891, 892, 893 }, // Bethnal Green
        new int[] { }, // Camberwell
        new int[] { 642, 643, 692 }, // Chelsea
        new int[] { }, // Deptford
        new int[] { 840, 888 }, // Finsbury
        new int[] { 544, 545, 593, 594, 640, 641, 689 }, // Fulham
        new int[] { }, // Greenwich
        new int[] { 938, 939, 940, 941, 987, 988, 989, 990, 1035, 1036, 1037, 1084, 1131 }, // Hackney
        new int[] { 687, 688, 734, 735, 736, 783, 784, 831, 880 }, // Hammersmith
        new int[] { 931, 932, 979, 980, 981, 1026, 1027, 1028, 1076 }, // Hampstead
        new int[] { 838, 839 }, // Holborn
        new int[] { 936, 937, 984, 985, 986, 1031, 1032, 1033, 1079, 1080, 1081, 1127, 1128}, // Islington
        new int[] { 690, 691, 737, 738, 785, 786, 832, 833, 881 }, // Kensington
        new int[] { }, // Lambeth
        new int[] { }, // Lewisham
        new int[] { 787, 834, 835, 882, 883 }, // Paddington
        new int[] { 655, 703, 750, 751, 798, 799, 800, 846, 894 }, // Poplar
        new int[] { 789, 790, 836, 837, 884, 885 }, // St Marylebone
        new int[] { 886, 887, 933, 934, 935, 982, 983, 1029, 1030, 1077, 1078 }, // St Pancras
        new int[] { 841, 889, 890 }, // Shoreditch
        new int[] { }, // Southwark
        new int[] { 795, 796, 797, 842, 843, 844, 845 }, // Stepney
        new int[] { 1034, 1082, 1083, 1130 }, // Stoke Newington
        new int[] { }, // Wandsworth
        new int[] { 644, 645, 693, 694, 739, 740, 741, 742, 788, 791 }, // Westminster
        new int[] { } // Woolwich
    };

    private double[] boroughPopulations = {
        128833, // City of London
        3368, // Battersea
        46221, // Bermondsey
        22385, // Bethnal Green
        7034, // Camberwell
        8825, // Chelsea
        11442, // Deptford
        55084, // Finsbury
        4450, // Fulham
        22246, // Greenwich
        12838, // Hackney
        5622, // Hammersmith
        4340, // Hampstead
        63390, // Holborn
        10247, // Islington
        8780, // Kensington
        28407, // Lambeth
        4412, // Lewisham
        2178, // Paddington
        8314, // Poplar
        64692, // St Marylebone
        32020, // St Pancras
        35034, // Shoreditch
        62820, // Southwark
        113630, // Stepney
        1998, // Stoke Newington
        14205, // Wandsworth
        163828, // Westminster
        12667 // Woolwich
    };
}
