using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI {

    public enum Overlay {
        None, Population, GroundwaterQuality
    }

    public class DataOverlay : MonoBehaviour {

        public HexGrid hexGrid;

        public void ChangeOverlay() {
            int index = gameObject.GetComponent<Dropdown>().value;
            Overlay overlay = (Overlay)index;
            hexGrid.SetDataOverlay(overlay);
        }
        
        
        public void UpdateOverlay() {
            int index = gameObject.GetComponent<Dropdown>().value;
            if (index > 0) {
                Overlay overlay = (Overlay)index;
                hexGrid.SetDataOverlay(overlay);
            }
        }

        private void OnEnable() {
            GameTime.NewDay += UpdateOverlay;
        }

        private void OnDisable() {
            GameTime.NewDay -= UpdateOverlay;
        }
    }

    public static class OverlayExtensions {

        public static Color? CellColor(this Overlay overlay, HexCell cell) {

            if (overlay == Overlay.Population) {
                return PopulationColor(cell);                
            }
            else if (overlay == Overlay.GroundwaterQuality) {
                return null;
            }            
            else {
                return null;
            }
        }

        public static Color PopulationColor(HexCell cell) {
            float population = cell.Population.Size;

            if (population > 100000) {
                throw new System.ArgumentOutOfRangeException("Population is greater than 100000");
            }
            else {
                // scale to set x=1 to a convenient point for the function below
                float val = population / 30000f;
                if (val > 1) {
                    Debug.Log(val);
                }

                // This function has a horiztonal asymtote at y = 1.0 :-)
                // at x = 1, y = 0.74
                if (val > 0) {
                    val = 0.1f + 0.9f * Mathf.Sqrt(val / (val + 1));
                }
                else {
                    val = 0;
                }

                return new Color(1f, 1 - val, 1f);
            }
        }


    }


}

