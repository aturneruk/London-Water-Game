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
    private Color mainColor;
    private Color? overlayColor;
    private Color? highlightColor;

    [SerializeField]
    HexCell[] neighbors;

    public HexGridChunk chunk;

    // Network data
    public bool riverDistanceSet = false;
    public int riverDistance;

    // River data
    public bool hasRiver;
    public bool isThames;
    public bool isRiverside;

    public Water.CellManager waterManager;
    public CellPopulation cellPopulation;
    public Water.Reservoir reservoir;

    public int Elevation {
        get {
            return global::Elevation.GetElevation(index);
        }
    }

    public Color Color {
        get {
            return color;
        }
        private set {
            if (color == value) {
                return;
            }
            else {
                color = value;
                Refresh();
            }
        }
    }

    public Color MainColor {
        get {
            return mainColor;
        }
        set {
            if (value == mainColor) {
                return;
            }
            else {
                mainColor = value;
                if (OverlayColor == null && HighlightColor == null) {
                    Color = mainColor;
                }
                else {
                    return;
                }
            }
        }
    }

    public Color? OverlayColor {
        get {
            return overlayColor;
        }
        set {
            if (value == overlayColor) {
                return;
            }
            else if (value == null) {
                overlayColor = null;
                if (HighlightColor != null) {
                    Color = (Color)HighlightColor;
                }
                else {
                    Color = MainColor;
                }
            }
            else {
                overlayColor = value;
                if (HighlightColor == null) {
                    Color = (Color)overlayColor;
                }
            }
        }
    }

    public Color? HighlightColor {
        get {
            return highlightColor;
        }
        set {
            if (value == highlightColor) {
                return;
            }
            else if (value == null) {
                highlightColor = null;
                if (OverlayColor != null) {
                    Color = (Color)OverlayColor;
                }
                else {
                    Color = MainColor;
                }
            }
            else {
                highlightColor = value;
                Color = (Color)HighlightColor;
            }
        }
    }

    private void Start() {
        SetMainColor();
    }

    private void OnDrawGizmosSelected() {
        Water.CellManager manager = this.waterManager;

        for (int? i = riverDistance; i > 0; i--) {
            Gizmos.color = Color.black;
            if (manager.overlandFlowNext) {
                Gizmos.DrawLine(manager.transform.position, manager.overlandFlowNext.transform.position);
                manager = manager.overlandFlowNext;
            }
        }

        if (manager.riverAbstractionCell && manager.riverDischargeCell) {
            //Gizmos.DrawLine(transform.position, abstractionCell.transform.position);
            Gizmos.DrawLine(manager.transform.position, manager.riverDischargeCell.transform.position);
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

    public void SetMainColor() {
        if (riverDistance == 0) {
            MainColor = HexMetrics.riverColor;
        }
        else if (gameObject.GetComponent<Water.Reservoir>() != null) {
            MainColor = HexMetrics.reservoirColor;
        }
        else if (borough.Name != null) {
            MainColor = HexMetrics.boroughColor;
        }
        else {
            MainColor = HexMetrics.defaultColor;
        }
    }

    public HexEdgeType GetEdgeType(HexDirection direction) {
        return HexMetrics.GetEdgeType(Elevation, GetNeighbor(direction).Elevation);
    }

    public HexEdgeType GetEdgeType(HexCell cell) {
        return HexMetrics.GetEdgeType(Elevation, cell.Elevation);
    }

    public void BuildReservoir() {

        gameObject.AddComponent<Water.Reservoir>();

    }

}
