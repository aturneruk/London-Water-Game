using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class HexCell : MonoBehaviour {

    public int index;
    public HexCoordinates coordinates;


    public Color Color {
        get {
            return color;
        }
        set {
            if (color == value) {
                return;
            }
            else {
                color = value;
                Refresh();
            }
        }
    }

    Color color;

    [SerializeField]
    HexCell[] neighbors;

    public HexGridChunk chunk;

    public RectTransform uiRect;

    // Population fields
    public float population;
    public float popGrowthRate;

    // Network data
    public int? riverDistance;

    // River data
    public bool hasRiver;
    public bool isThames;
    public bool isRiverside;
    public HexCell abstractionCell;
    public HexCell dischargeCell;

    private void Start() {
            // InvokeRepeating("Population.GrowPopulation(this)", 2.0f, 0.3f);

        if(riverDistance == 0) {
            color = Color.blue;
                    }
        else if (riverDistance == 1) {
            color = Color.yellow;
        }
        else if (riverDistance == 2) {
            color = Color.green;
        }
        else if (riverDistance == 3) {
            color = Color.magenta;
        }

    }

    public HexCell GetNeighbor(HexDirection direction) {
        return neighbors[(int)direction];
    }

    public void SetNeighbor(HexDirection direction, HexCell cell) {
        neighbors[(int)direction] = cell;
        cell.neighbors[(int)direction.Opposite()] = this;
    }

    public int SetRiverDistance(int distance) {

        if (riverDistance == null) {
            riverDistance = distance;
            return 1;
        }
        else {
            return 0;
        }
    }

    void Refresh() {
        if (chunk) {
            chunk.Refresh();
            for (int i = 0; i < neighbors.Length; i++) {
                HexCell neighbor = neighbors[i];
                if (neighbor != null && neighbor.chunk != chunk) {
                    neighbor.chunk.Refresh();
                }
            }
        }
    }

}
