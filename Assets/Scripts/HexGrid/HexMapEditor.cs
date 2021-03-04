using UnityEngine;
using UnityEngine.EventSystems;

public class HexMapEditor : MonoBehaviour
{

    public Color[] colors;

    public HexGrid hexGrid;

    private Color activeColor;

    private void Awake() {
        SelectColor(0);
    }

    private void Update() {
        if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject()) {
            HandleInput();
        }
    }

    public void SelectColor(int index) {
        activeColor = colors[index];
    }

    private void HandleInput() {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(inputRay, out hit)) {
            EditCell(hexGrid.GetCellFromPosition(hit.point));
        }
    }

    void EditCell(HexCell cell) {
        cell.Color = activeColor;
        // Debug.Log("touched at " + cell.coordinates.ToString());
    }

}
