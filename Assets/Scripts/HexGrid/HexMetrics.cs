using UnityEngine;

public class HexMetrics {

    public const float outerRadius = 10f;
    public const float innerRadius = outerRadius * 0.866025404f;

    public static readonly Color defaultColor = Color.white;
    public static readonly Color boroughColor = Color.grey;
    public static readonly Color riverColor = Color.blue;
    public static Color selectedColor = Color.cyan;
    public static Color selectedBoroughColor = Color.yellow;

    public static Vector3[] corners = {
        new Vector3(0f, 0f, outerRadius),
        new Vector3(innerRadius, 0f, 0.5f * outerRadius),
        new Vector3(innerRadius, 0f, -0.5f * outerRadius),
        new Vector3(0f, 0f, -outerRadius),
        new Vector3(-innerRadius, 0f, -0.5f * outerRadius),
        new Vector3(-innerRadius, 0f, 0.5f * outerRadius),
        new Vector3(0f, 0f, outerRadius)
    };

    // Set chunk size
    public const int chunkSizeX = 4, chunkSizeZ = 4;

    // Color blending options
    public const float solidFactor = 0.75f;
    public const float blendFactor = 1f - solidFactor;

    public const int elevationStep = 1;

    // Terrace options and methods
    public const int terracesPerSlope = 2;
    public const int terraceSteps = terracesPerSlope * 2 + 1;
    public const float horizontalTerraceStepSize = 1f / terraceSteps;
    public const float verticalTerraceStepSize = 1f / (terracesPerSlope + 1);

    public static Vector3 TerraceLerp(Vector3 a, Vector3 b, int step) {
        float h = step * horizontalTerraceStepSize;
        a.x += (b.x - a.x) * h;
        a.z += (b.z - a.z) * h;
        float v = ((step + 1) / 2) * verticalTerraceStepSize;
        a.y += (b.y - a.y) * v;
        return a;
    }

    public static Color TerraceLerp(Color a, Color b, int step) {
        float h = step * horizontalTerraceStepSize;
        return Color.Lerp(a, b, h);
    }

    public static HexEdgeType GetEdgeType(int elevation1, int elevation2) {

        int delta = elevation1 - elevation2;

        switch (delta) {
            case 0:
                return HexEdgeType.Flat;
            case 1:
            case -1:
                return HexEdgeType.Slope;
            default:
                return HexEdgeType.Cliff;
        }
    }

    // Methods used in triangulation
    //public static Vector3 GetFirstCorner(HexDirection direction) {
    //    return corners[(int)direction];
    //}

    //public static Vector3 GetSecondCorner(HexDirection direction) {
    //    return corners[(int)direction + 1];
    //}

    public static Vector3 GetFirstSolidCorner(HexDirection direction) {
        return corners[(int)direction] * solidFactor;
    }

    public static Vector3 GetSecondSolidCorner(HexDirection direction) {
        return corners[(int)direction + 1] * solidFactor;
    }

    public static Vector3 GetBridge(HexDirection direction) {
        return (corners[(int)direction] + corners[(int)direction + 1]) * blendFactor;
    }
}
