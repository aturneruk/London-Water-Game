using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

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

#if UNITY_EDITOR
        SceneVisibilityManager visibilityManager = SceneVisibilityManager.instance;
        visibilityManager.DisablePicking(gameObject, false);
#endif
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

        if (direction <= HexDirection.SE) {
            TriangulateConnection(direction, cell, v1, v2);
        }
    }

    private void TriangulateConnection(HexDirection direction, HexCell cell, Vector3 v1, Vector3 v2) {

        HexCell neighbor = cell.GetNeighbor(direction);

        if (neighbor == null) {
            return;
        }

        Vector3 bridge = HexMetrics.GetBridge(direction);
        Vector3 v3 = v1 + bridge;
        Vector3 v4 = v2 + bridge;
        // Override the height for the other end of the bridge
        v3.y = v4.y = neighbor.Elevation;

        // Add terraces for "slope" type bridges
        if (cell.GetEdgeType(direction) == HexEdgeType.Slope) {
            TriangulateEdgeTerraces(cell, v1, v2, neighbor, v3, v4);
        }
        else {
            AddQuad(v1, v2, v3, v4);
            AddQuadColor(cell.Color, neighbor.Color);
        }


        // Deal with the 3-way corners
        HexCell nextNeighbor = cell.GetNeighbor(direction.Next());
        if (direction <= HexDirection.E && nextNeighbor != null) {
            Vector3 v5 = v2 + HexMetrics.GetBridge(direction.Next());
            v5.y = nextNeighbor.Elevation;

            if (cell.Elevation <= neighbor.Elevation) {
                if (cell.Elevation <= nextNeighbor.Elevation) {
                    TriangulateCorner(cell, v2, neighbor, v4, nextNeighbor, v5);
                }
                else {
                    TriangulateCorner(nextNeighbor, v5, cell, v2, neighbor, v4);
                }
            }
            else if (neighbor.Elevation <= nextNeighbor.Elevation) {
                TriangulateCorner(neighbor, v4, nextNeighbor, v5, cell, v2);
            }
            else {
                TriangulateCorner(nextNeighbor, v5, cell, v2, neighbor, v4);
            }
        }
    }

    private void TriangulateEdgeTerraces(HexCell startCell, Vector3 startLeft, Vector3 startRight, HexCell endCell, Vector3 endLeft, Vector3 endRight) {

        Vector3 v3 = HexMetrics.TerraceLerp(startLeft, endLeft, 1);
        Vector3 v4 = HexMetrics.TerraceLerp(startRight, endRight, 1);
        Color c2 = HexMetrics.TerraceLerp(startCell.Color, endCell.Color, 1);

        AddQuad(startLeft, startRight, v3, v4);
        AddQuadColor(startCell.Color, c2);

        for (int i = 2; i < HexMetrics.terraceSteps; i++) {
            Vector3 v1 = v3;
            Vector3 v2 = v4;
            Color c1 = c2;
            v3 = HexMetrics.TerraceLerp(startLeft, endLeft, i);
            v4 = HexMetrics.TerraceLerp(startRight, endRight, i);
            c2 = HexMetrics.TerraceLerp(startCell.Color, endCell.Color, i);
            AddQuad(v1, v2, v3, v4);
            AddQuadColor(c1, c2);
        }

        AddQuad(v3, v4, endLeft, endRight);
        AddQuadColor(c2, endCell.Color);
    }

    private void TriangulateCorner(HexCell bottomCell, Vector3 bottom, HexCell leftCell, Vector3 left, HexCell rightCell, Vector3 right) {

        HexEdgeType leftEdgeType = bottomCell.GetEdgeType(leftCell);
        HexEdgeType rightEdgeType = bottomCell.GetEdgeType(rightCell);

        if (leftEdgeType == HexEdgeType.Slope) {
            if (rightEdgeType == HexEdgeType.Slope) {
                TriangulateCornerTerrace(bottomCell, bottom, leftCell, left, rightCell, right);
            }
            else if (rightEdgeType == HexEdgeType.Flat) {
                TriangulateCornerTerrace(leftCell, left, rightCell, right, bottomCell, bottom);
            }
            else {
                TriangulateCornerTerraceCliff(bottomCell, bottom, leftCell, left, rightCell, right);
            }
        }
        else if (rightEdgeType == HexEdgeType.Slope) {
            if (leftEdgeType == HexEdgeType.Flat) {
                TriangulateCornerTerrace(rightCell, right, bottomCell, bottom, leftCell, left);
            }
            else {
                TriangulateCornerCliffTerrace(bottomCell, bottom, leftCell, left, rightCell, right);
            }
        }
        else if (leftCell.GetEdgeType(rightCell) == HexEdgeType.Slope) {
            if (leftCell.Elevation < rightCell.Elevation) {
                TriangulateCornerCliffTerrace(rightCell, right, bottomCell, bottom, leftCell, left);
            }
            else {
                TriangulateCornerTerraceCliff(leftCell, left, rightCell, right, bottomCell, bottom);
            }
        }
        else {
            AddTriangle(bottom, left, right);
            AddTriangleColor(bottomCell.Color, leftCell.Color, rightCell.Color);
        }
    }

    private void TriangulateCornerTerrace(HexCell startCell, Vector3 start, HexCell leftCell, Vector3 left, HexCell rightCell, Vector3 right) {

        Vector3 v3 = HexMetrics.TerraceLerp(start, left, 1);
        Vector3 v4 = HexMetrics.TerraceLerp(start, right, 1);
        Color c3 = HexMetrics.TerraceLerp(startCell.Color, leftCell.Color, 1);
        Color c4 = HexMetrics.TerraceLerp(startCell.Color, rightCell.Color, 1);

        AddTriangle(start, v3, v4);
        AddTriangleColor(startCell.Color, c3, c4);

        for (int i = 2; i < HexMetrics.terraceSteps; i++) {
            Vector3 v1 = v3;
            Vector3 v2 = v4;
            Color c1 = c3;
            Color c2 = c4;
            v3 = HexMetrics.TerraceLerp(start, left, i);
            v4 = HexMetrics.TerraceLerp(start, right, i);
            c3 = HexMetrics.TerraceLerp(startCell.Color, leftCell.Color, i);
            c4 = HexMetrics.TerraceLerp(startCell.Color, rightCell.Color, i);
            AddQuad(v1, v2, v3, v4);
            AddQuadColor(c1, c2, c3, c4);
        }

        AddQuad(v3, v4, left, right);
        AddQuadColor(c3, c4, leftCell.Color, rightCell.Color);
    }

    private void TriangulateCornerTerraceCliff(HexCell startCell, Vector3 start, HexCell leftCell, Vector3 left, HexCell rightCell, Vector3 right) {
        float b = Mathf.Abs(1f / (rightCell.Elevation - startCell.Elevation));
        Vector3 boundary = Vector3.Lerp(start, right, b);
        Color boundaryColor = Color.Lerp(startCell.Color, rightCell.Color, b);

        TriangulateBoundaryTriangle(startCell, start, leftCell, left, boundaryColor, boundary);

        if (leftCell.GetEdgeType(rightCell) == HexEdgeType.Slope) {
            TriangulateBoundaryTriangle(leftCell, left, rightCell, right, boundaryColor, boundary);
        }
        else {
            AddTriangle(left, right, boundary);
            AddTriangleColor(leftCell.Color, rightCell.Color, boundaryColor);
        }

    }
    private void TriangulateCornerCliffTerrace(HexCell startCell, Vector3 start, HexCell leftCell, Vector3 left, HexCell rightCell, Vector3 right) {
        float b = Mathf.Abs(1f / (leftCell.Elevation - startCell.Elevation));
        Vector3 boundary = Vector3.Lerp(start, left, b);
        Color boundaryColor = Color.Lerp(startCell.Color, leftCell.Color, b);

        TriangulateBoundaryTriangle(rightCell, right, startCell, start, boundaryColor, boundary);

        if (leftCell.GetEdgeType(rightCell) == HexEdgeType.Slope) {
            TriangulateBoundaryTriangle(leftCell, left, rightCell, right, boundaryColor, boundary);
        }
        else {
            AddTriangle(left, right, boundary);
            AddTriangleColor(leftCell.Color, rightCell.Color, boundaryColor);
        }
    }

    private void TriangulateBoundaryTriangle(HexCell startCell, Vector3 start, HexCell leftCell, Vector3 left, Color boundaryColor, Vector3 boundary) {
        Vector3 v2 = HexMetrics.TerraceLerp(start, left, 1);
        Color c2 = HexMetrics.TerraceLerp(startCell.Color, leftCell.Color, 1);

        AddTriangle(start, v2, boundary);
        AddTriangleColor(startCell.Color, c2, boundaryColor);

        for (int i = 2; i < HexMetrics.terraceSteps; i++) {
            Vector3 v1 = v2;
            Color c1 = c2;
            v2 = HexMetrics.TerraceLerp(start, left, i);
            c2 = HexMetrics.TerraceLerp(startCell.Color, leftCell.Color, i);
            AddTriangle(v1, v2, boundary);
            AddTriangleColor(c1, c2, boundaryColor);
        }

        AddTriangle(v2, left, boundary);
        AddTriangleColor(c2, leftCell.Color, boundaryColor);
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
