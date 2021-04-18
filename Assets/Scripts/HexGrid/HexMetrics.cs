﻿using UnityEngine;

public class HexMetrics {

    public const float outerRadius = 10f;
    public const float innerRadius = outerRadius * 0.866025404f;

    public static readonly Color defaultColor = Color.white;
    public static readonly Color boroughColor = Color.grey;
    public static readonly Color riverColor = Color.blue;
    public static readonly Color selectedColor = Color.cyan;
    public static readonly Color selectedBoroughColor = Color.yellow;

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
    public const float solidFactor = 0.5f;
    public const float blendFactor = 1f - solidFactor;

    public static Vector3 GetFirstCorner(HexDirection direction) {
        return corners[(int)direction];
    }

    public static Vector3 GetSecondCorner(HexDirection direction) {
        return corners[(int)direction + 1];
    }

    public static Vector3 GetFirstSolidCorner(HexDirection direction) {
        return corners[(int)direction] * solidFactor;
    }

    public static Vector3 GetSecondSolidCorner(HexDirection direction) {
        return corners[(int)direction + 1] * solidFactor;
    }

    public static Vector3 GetBridge(HexDirection direction) {
        return (corners[(int)direction] + corners[(int)direction + 1]) * 0.5f * blendFactor;
    }
}
