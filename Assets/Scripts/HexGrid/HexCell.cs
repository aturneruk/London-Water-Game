using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class HexCell : MonoBehaviour {

    public int index;

    // Geometric data
    public HexCoordinates coordinates;
    public int area = 1000000;
    
    public Borough borough;

    private Color color;

    [SerializeField]
    HexCell[] neighbors;

    public HexGridChunk chunk;

    public RectTransform uiRect;

    public Population Population;

    // Network data
    public int? riverDistance;

    // River data
    public bool hasRiver;
    public bool isThames;
    public bool isRiverside;
    public HexCell abstractionCell;
    public HexCell dischargeCell;

    public Water.Manager waterManager;

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

    private void Start() {
        // InvokeRepeating("Population.GrowPopulation(this)", 2.0f, 0.3f);     

        if (riverDistance == 0) {
            color = HexGrid.riverColor;
        }

        //for (int i = 0; i < 20; i += 3) {
        //    if (riverDistance == i + 1) {
        //        color = Color.yellow;
        //    }
        //    else if (riverDistance == i + 2) {
        //        color = Color.magenta;
        //    }
        //    else if (riverDistance == i + 3) {
        //        color = Color.green;
        //    }
        //}
    }

    private void OnDrawGizmosSelected() {
        HexCell cell = this;

        for (int? i = riverDistance; i > 0; i--) {
            Gizmos.color = Color.black;
            Gizmos.DrawLine(cell.transform.position, cell.dischargeCell.transform.position);
            cell = cell.dischargeCell;
        }        
    }

    public HexCell GetNeighbor(HexDirection direction) {
        return neighbors[(int)direction];
    }

    public void SetNeighbor(HexDirection direction, HexCell cell) {
        neighbors[(int)direction] = cell;
        cell.neighbors[(int)direction.Opposite()] = this;
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

    public void SetDefaultColor() {
        if (isThames) {
            Color = HexGrid.riverColor;
        }
        else if (borough.Name != null) {
            Color = Color.grey;
        }
        else {
            Color = HexGrid.defaultColor;
        }
    }
}
