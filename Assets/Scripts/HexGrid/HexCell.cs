using UnityEngine;

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

    // River data
    public bool hasIncomingRiver, hasOutgoingRiver;

    private void Start() {
            // InvokeRepeating("Population.GrowPopulation(this)", 2.0f, 0.3f);
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

}
