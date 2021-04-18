using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI {

    public enum Overlay {
        None, Population, GroundwaterQuality
    }

    public static class OverlayExtensions {

        public static Color? CellColor(this Overlay overlay, HexCell cell) {

            if (overlay == Overlay.Population) {

                float population = cell.Population.Size;

                if (population > 100000) {
                    throw new System.ArgumentOutOfRangeException("Population is greater than 100000");
                }
                else {
                    float val = population / 30000;
                    val = Mathf.Sqrt(val / (val + 1));
                    return new Color(1f, 1 - val, 1f);
                }
            }
            else if (overlay == Overlay.GroundwaterQuality) {
                return null;
            }            
            else {
                return null;
            }
        }
    }

    public class DataOverlay : MonoBehaviour {

        public HexGrid hexGrid;

        public void ChangeOverlay() {
            int index = gameObject.GetComponent<Dropdown>().value;
            Debug.Log(index);
            Overlay overlay = (Overlay)index;
            Debug.Log(overlay.ToString());
            hexGrid.SetDataOverlay(overlay);
        }
    }
}

