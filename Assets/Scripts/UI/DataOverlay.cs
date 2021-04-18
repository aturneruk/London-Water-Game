using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI {

    public enum Overlay {
        None, Population, GroundwaterQuality
    }

    public static class OverlayExtensions {

        public static Color CellColor(this Overlay overlay) {

            if (overlay == Overlay.None) {
                return Color.white;
            }
            else {
                return Color.yellow;
            }
        }
    }

    public class DataOverlay : MonoBehaviour {

        public HexGrid hexGrid;

        public void ChangeOverlay() {
            int index = gameObject.GetComponent<Dropdown>().value;
            Overlay overlay = (Overlay)index;
            hexGrid.SetDataOverlay(overlay);
        }

    }

}

