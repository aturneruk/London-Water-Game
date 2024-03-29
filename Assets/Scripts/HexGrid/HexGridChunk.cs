﻿using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class HexGridChunk : MonoBehaviour {

    HexCell[] cells;

    HexMesh hexMesh;
    Canvas gridCanvas;

    private void Awake() {
        gridCanvas = GetComponentInChildren<Canvas>();
        hexMesh = GetComponentInChildren<HexMesh>();

        #if UNITY_EDITOR
        SceneVisibilityManager visibilityManager = SceneVisibilityManager.instance;
        visibilityManager.DisablePicking(gameObject, false);
        #endif

        cells = new HexCell[HexMetrics.chunkSizeX * HexMetrics.chunkSizeZ];
    }

    public void AddCell(int index, HexCell cell) {
        cells[index] = cell;
        cell.chunk = this;
        cell.transform.SetParent(transform, false);
        // cell.uiRect.SetParent(gridCanvas.transform, false);
    }

    public void Refresh() {
        enabled = true;
    }

    private void LateUpdate() {
        hexMesh.Triangulate(cells);
        enabled = false;
    }

}

