using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI {

    public enum Overlay {
        None, Population, GroundwaterLevel, GroundwaterQuality
    }

    public class DataOverlay : MonoBehaviour {

        public HexGrid hexGrid;

        public void ChangeOverlay() {
            int index = gameObject.GetComponent<Dropdown>().value;
            Overlay overlay = (Overlay)index;

            switch (overlay) {
                case Overlay.None:
                case Overlay.Population:
                    HexMetrics.selectedColor = Color.cyan;
                    break;
                case Overlay.GroundwaterLevel:
                case Overlay.GroundwaterQuality:
                    HexMetrics.selectedColor = Color.magenta;
                    break;
                default:
                    throw new System.ArgumentException("Selected overlay is not recognised");
            }
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

            switch (overlay) {
                case Overlay.Population:
                    return PopulationColor(cell);
                case Overlay.GroundwaterLevel:
                    return GroundwaterLevelColor(cell);
                case Overlay.GroundwaterQuality:
                    return GroundwaterQualityColor(cell);
                default: return null;
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

        public static Color GroundwaterLevelColor(HexCell cell) {
            float val = cell.waterManager.groundwater.Storage.Level;

            if (val <= 1 && val >= 0) {
                return new Color(1f - val, 1f, 1f);
            }
            else {
                throw new System.ArgumentOutOfRangeException("Groundwater level must be between 0 and 1");
            }
        }

        public static Color GroundwaterQualityColor(HexCell cell) {
            float val = cell.waterManager.groundwater.Storage.Quality;

            if (val <= 1 && val >= 0) {
                val = val * val;
                return new Color32((byte)(75), (byte)(54), (byte)(33), (byte)(0));
                //return new Color32((byte)(75 + val * 180), (byte)(54 + val * 201), (byte)(33 + val * 222), 255);

            }
            else {
                throw new System.ArgumentOutOfRangeException("Groundwater quality must be between 0 and 1");
            }
        }

    }
}

