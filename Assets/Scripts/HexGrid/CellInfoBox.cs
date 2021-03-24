using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CellInfoBox : MonoBehaviour
{

    public HexGrid hexGrid;

    CanvasGroup canvasGroup;
    bool isOpen;

    // Start is called before the first frame update
    void Start()
    {
        canvasGroup = gameObject.GetComponent<CanvasGroup>();
        Hide();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()) {
            HandleInput();
        } 
    }

    public void CloseDiaglogue() {
        Hide();
    }

    private void HandleInput() {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(inputRay, out hit)) {
            Show(hexGrid.GetCellFromPosition(hit.point));
        }
    }

    private void Hide() {
        isOpen = false;
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    private void Show(HexCell cell) {
        isOpen = true;
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }




}
