using UnityEngine;
using UnityEngine.UI;
using UI;

public class HexGrid : MonoBehaviour {

    // Define grid size
    public int chunkCountX = 12, chunkCountZ = 8;
    int cellCountX, cellCountZ;

    // Set up chunk reference
    public HexGridChunk chunkPrefab;
    HexGridChunk[] chunks;

    // Set up cells
    public HexCell cellPrefab;
    HexCell[] cells;

    private void Awake() {
        cellCountX = chunkCountX * HexMetrics.chunkSizeX;
        cellCountZ = chunkCountZ * HexMetrics.chunkSizeZ;

        Application.targetFrameRate = 60;

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
        // position.y = 0;
        position.y = Elevation.GetElevation(i);
        position.z = z * (HexMetrics.outerRadius * 1.5f);

        HexCell cell = cells[i] = Instantiate<HexCell>(cellPrefab);
        cell.transform.localPosition = position;
        cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
        cell.index = i;

        cell.Population = cell.gameObject.AddComponent<Population>();

        gameObject.GetComponent<Water.GridManager>().AddCellManager(cell);

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

    public HexCell GetCellFromIndex(int i) {
        return cells[i];
    }

    public int GetTotalPopulation() {

        float totalPopulation = 0;

        foreach (HexCell cell in cells) {
            totalPopulation += cell.Population.Size;
        }

        return Mathf.RoundToInt(totalPopulation);
    }

    public void SetDataOverlay(Overlay overlay) {

        foreach (HexCell cell in cells) {

            if (cell.riverDistance != 0) {
                cell.OverlayColor = overlay.CellColor(cell);
            }
        }
    }
}
