using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HexMesh : MonoBehaviour {

    // Set up fields
    Mesh hexMesh;
    MeshCollider meshCollider;

    // Set up lists 
    static List<Vector3> vertices = new List<Vector3>();
    static List<int> triangles = new List<int>();
    static List<Color> colors = new List<Color>();

    private void Awake() {
        GetComponent<MeshFilter>().mesh = hexMesh = new Mesh();
        meshCollider = gameObject.AddComponent<MeshCollider>();
        hexMesh.name = "Hex Mesh";
    }

    public void Triangulate(HexCell[] cells) {
        hexMesh.Clear();
        vertices.Clear();
        triangles.Clear();
        colors.Clear();

        // Create triangles for each cell 
        for (int i = 0; i < cells.Length; i++) {
            Triangulate(cells[i]);
        }

        // Bring all lists into the HexMesh 
        hexMesh.vertices = vertices.ToArray();
        hexMesh.triangles = triangles.ToArray();
        hexMesh.colors = colors.ToArray();
        hexMesh.RecalculateNormals();
        meshCollider.sharedMesh = hexMesh;

    }

    private void Triangulate(HexCell cell) {
        for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++) {
            Triangulate(d, cell);
        }
    }

    private void Triangulate(HexDirection direction, HexCell cell) {
        
        Vector3 centre = cell.transform.localPosition;
        Vector3 v1 = centre + HexMetrics.GetFirstSolidCorner(direction);
        Vector3 v2 = centre + HexMetrics.GetSecondSolidCorner(direction);        
        
        // Add the inner solid triangle
        AddTriangle(centre, v1, v2);
        AddTriangleColor(cell.Color); // Set color of this triangle

        Vector3 bridge = HexMetrics.GetBridge(direction);
        Vector3 v3 = v1 + bridge;
        Vector3 v4 = v2 + bridge;

        HexCell prevNeighbor = cell.GetNeighbor(direction.Previous()) ?? cell;
        HexCell neighbor = cell.GetNeighbor(direction) ?? cell;
        HexCell nextNeighbor = cell.GetNeighbor(direction.Next()) ?? cell;

        Color bridgeColor = (cell.Color + neighbor.Color) * 0.5f;

        // Add the bridge quad
        AddQuad(v1, v2, v3, v4);
        AddQuadColor(cell.Color, bridgeColor);

        // Add the two 3-way triangles
        AddTriangle(v1, centre + HexMetrics.GetFirstCorner(direction), v3);
        AddTriangleColor(cell.Color, (cell.Color + prevNeighbor.Color + neighbor.Color) / 3f, bridgeColor);
        AddTriangle(v2, v4, centre + HexMetrics.GetSecondCorner(direction));
        AddTriangleColor(cell.Color, bridgeColor, (cell.Color + neighbor.Color + nextNeighbor.Color) / 3f);

    }

    private void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3) {
        int vertexIndex = vertices.Count;
        // Add the Vector3 of each vertex to vertices list
        vertices.Add(v1);
        vertices.Add(v2);
        vertices.Add(v3);
        // Add the vertex number of each vertex to triangles list
        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 2);
    }

    private void AddTriangleColor(Color color) {
        // Add color for each of the 3 vertices to colors list
        colors.Add(color);
        colors.Add(color);
        colors.Add(color);
    }

    private void AddTriangleColor(Color c1, Color c2, Color c3) {
        // Add color for each of the 3 vertices to colors list
        colors.Add(c1);
        colors.Add(c2);
        colors.Add(c3);
    }

    private void AddQuad(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4) {
        int vertexIndex = vertices.Count;
        vertices.Add(v1);
        vertices.Add(v2);
        vertices.Add(v3);
        vertices.Add(v4);
        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 2);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 2);
        triangles.Add(vertexIndex + 3);
    }

    private void AddQuadColor(Color c1, Color c2, Color c3, Color c4) {
        colors.Add(c1);
        colors.Add(c2);
        colors.Add(c3);
        colors.Add(c4);
    }

    void AddQuadColor(Color c1, Color c2) {
        colors.Add(c1);
        colors.Add(c1);
        colors.Add(c2);
        colors.Add(c2);
    }
}
