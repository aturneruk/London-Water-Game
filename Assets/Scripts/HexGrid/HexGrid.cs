using UnityEngine;
using UnityEngine.UI;

public class HexGrid : MonoBehaviour {

    // Define grid size
    public int chunkCountX = 12, chunkCountZ = 8;
    int cellCountX, cellCountZ;

    int centralAreaXIndex;
    int centralAreaZIndex;

    // Set up chunk reference
    public HexGridChunk chunkPrefab;
    HexGridChunk[] chunks;

    // Set up cells
    public HexCell cellPrefab;
    HexCell[] cells;

    // Set up cell labels
    public UnityEngine.UI.Text cellLabelPrefab;

    // Set up colours
    public Color defaultColor = Color.white;
    public Color touchedColor = Color.magenta;

    private void Awake() {
        cellCountX = chunkCountX * HexMetrics.chunkSizeX;
        cellCountZ = chunkCountZ * HexMetrics.chunkSizeZ;

        centralAreaXIndex = ((cellCountX - HexMetrics.startAreaSize) / 2) - 1;
        centralAreaZIndex = ((cellCountZ - HexMetrics.startAreaSize) / 2) - 1;

        CreateChunks();
        CreateCells();
    }

    private void CreateChunks() {
        chunks = new HexGridChunk[chunkCountX * chunkCountZ];

        for (int z = 0, i = 0; z < chunkCountZ; z++) {
            for (int x = 0; x < chunkCountX; x++) {
                HexGridChunk chunk = chunks[i++] = Instantiate(chunkPrefab);
                chunk.transform.SetParent(transform);
            }
        }
    }

    private void CreateCells() {
        // Create the required number of HexCells using the prefab
        cells = new HexCell[cellCountZ * cellCountX];

        for (int z = 0, i = 0; z < cellCountZ; z++) {
            for (int x = 0; x < cellCountX; x++) {
                CreateCell(x, z, i++);
            }
        }
    }


    void CreateCell(int x, int z, int i) {
        Vector3 position;
        position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
        position.y = 0f;
        position.z = z * (HexMetrics.outerRadius * 1.5f);

        HexCell cell = cells[i] = Instantiate<HexCell>(cellPrefab);
        cell.transform.localPosition = position;
        cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
        cell.Color = defaultColor;
        cell.index = i;

        if (x > 0) {
            cell.SetNeighbor(HexDirection.W, cells[i - 1]);
        }

        if (z > 0) {
            if ((z & 1) == 0) {
                cell.SetNeighbor(HexDirection.SE, cells[i - cellCountX]);
                if (x > 0) {
                    cell.SetNeighbor(HexDirection.SW, cells[i - cellCountX - 1]);
                }
            }
            else {
                cell.SetNeighbor(HexDirection.SW, cells[i - cellCountX]);
                if (x < cellCountX - 1) {
                    cell.SetNeighbor(HexDirection.SE, cells[i - cellCountX + 1]);
                }
            }
        }

        // Set initial population
        if (x > centralAreaXIndex && x <= centralAreaXIndex + HexMetrics.startAreaSize) {
            if (z > centralAreaZIndex && z <= centralAreaZIndex + HexMetrics.startAreaSize) {
                cell.population = 300f;
            }
        }
        cell.popGrowthRate = 0.1f;

        Text label = Instantiate<Text>(cellLabelPrefab);
        label.rectTransform.anchoredPosition = new Vector2(position.x, position.z);
        label.text = cell.coordinates.ToStringOnSeparateLines();
        cell.uiRect = label.rectTransform;

        AddCellToChunk(x, z, cell);
    }

    void AddCellToChunk(int x, int z, HexCell cell) {
        int chunkX = x / HexMetrics.chunkSizeX;
        int chunkZ = z / HexMetrics.chunkSizeZ;
        HexGridChunk chunk = chunks[chunkX + chunkZ * chunkCountX];

        int localX = x - chunkX * HexMetrics.chunkSizeX;
        int localZ = z - chunkZ * HexMetrics.chunkSizeZ;
        chunk.AddCell(localX + localZ * HexMetrics.chunkSizeX, cell);
    }

    public HexCell GetCellFromPosition(Vector3 position) {
        position = transform.InverseTransformPoint(position);
        HexCoordinates coordinates = HexCoordinates.FromPosition(position);
        int index = coordinates.X + coordinates.Z * cellCountX + coordinates.Z / 2;
        return cells[index];
    }

    public HexCell GetCellFromOffset(int x, int z) {
        int index = x + (z * cellCountX);
        return cells[index];
    }

}
