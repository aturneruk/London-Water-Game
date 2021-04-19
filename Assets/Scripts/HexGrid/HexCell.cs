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

    public void SetMainColor() {
        if (riverDistance == 0) {
            MainColor = HexMetrics.riverColor;
        }
        else if (borough.Name != null) {
            MainColor = HexMetrics.boroughColor;
        }
        else {
            MainColor = HexMetrics.defaultColor;
        }
    }
}
