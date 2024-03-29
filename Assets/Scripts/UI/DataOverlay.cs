using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI {

    public enum Overlay {
        None, Population, GroundwaterLevel, GroundwaterQuality, SupplyRatio, WaterSources
    }

    public class DataOverlay : MonoBehaviour {

        public HexGrid hexGrid;

        Overlay selectedOverlay;

        public void ChangeOverlay(int index) {
            selectedOverlay = (Overlay)index;

            switch (selectedOverlay) {
                case Overlay.None:
                case Overlay.Population:
                    HexMetrics.selectedColor = Color.cyan;
                    break;
                case Overlay.GroundwaterLevel:
                case Overlay.GroundwaterQuality:
                case Overlay.SupplyRatio:
                case Overlay.WaterSources:
                    HexMetrics.selectedColor = Color.magenta;
                    break;
                default:
                    throw new ArgumentException("Selected overlay is not recognised");
            }
            hexGrid.SetDataOverlay(selectedOverlay);
        }        
        
        public void UpdateOverlay() {
            if (selectedOverlay != Overlay.None) {
                hexGrid.SetDataOverlay(selectedOverlay);
            }
        }

        private void OnEnable() {
            GameTime.NewMonth += UpdateOverlay;
        }

        private void OnDisable() {
            GameTime.NewMonth -= UpdateOverlay;
        }
    }

    public static class OverlayExtensions {

        public static Color? CellColor(this Overlay overlay, HexCell cell) {

            switch (overlay) {
                case Overlay.Population:
                    throw new System.ArgumentException("When using the population overlay the maximum cell population should also be specified");
                case Overlay.GroundwaterLevel:
                    return GroundwaterLevelColor(cell);
                case Overlay.GroundwaterQuality:
                    return GroundwaterQualityColor(cell);
                case Overlay.SupplyRatio:
                    return SupplyRatioColor(cell);
                case Overlay.WaterSources:
                    return WaterSourcesColor(cell);
                default: 
                    return null;
            }
        }

        public static Color? CellColor(this Overlay overlay, HexCell cell, double maxCellPopulation) {

            switch (overlay) {
                case Overlay.Population:
                    return PopulationColor(cell, maxCellPopulation);
                case Overlay.GroundwaterLevel:
                    return GroundwaterLevelColor(cell);
                case Overlay.GroundwaterQuality:
                    return GroundwaterQualityColor(cell);
                case Overlay.SupplyRatio:
                    return SupplyRatioColor(cell);
                case Overlay.WaterSources:
                    return WaterSourcesColor(cell);
                default:
                    return null;
            }
        }

        public static Color PopulationColor(HexCell cell, double maxPopulation) {
            double population = cell.cellPopulation.Size;

            // scale to set x=1 to a convenient point for the function below
            double val = population / maxPopulation;

            // This function has a horiztonal asymtote at y = 1.0 :-)
            // at x = 1, y = 0.74
            if (val > 0) {
                val = 0.1f + 0.9f * Math.Sqrt(val / (val + 1));
            }
            else {
                val = 0;
            }

            return new Color(1f, 1 - (float)val, 1f);            
        }

        public static Color GroundwaterLevelColor(HexCell cell) {
            double val = cell.waterManager.groundwater.Storage.Level;

            if (val <= 1 && val >= 0) {
                return new Color(1f - (float)val, 1f, 1f);
            }
            else {
                throw new ArgumentOutOfRangeException("Groundwater level must be between 0 and 1");
            }
        }

        public static Color GroundwaterQualityColor(HexCell cell) {
            double val = cell.waterManager.groundwater.Storage.Quality;

            if (val <= 1 && val >= 0) {
                val = val * val;
                return new Color(1 - (float)val, (float)val, 0f);
            }
            else {
                throw new ArgumentOutOfRangeException("Groundwater quality must be between 0 and 1");
            }
        }

        public static Color? SupplyRatioColor(HexCell cell) {

            Water.CellManager manager = cell.waterManager;
            double supplyRatio = manager.supplyRatio;

            if (cell.GetComponent<Water.Reservoir>()) {
                return null;
            }
            else if (supplyRatio <= 1 && supplyRatio >= 0) {
                return new Color(1 - (float)supplyRatio, (float)supplyRatio, 0f);
            }
            else {
                throw new ArgumentOutOfRangeException("Supply ratio must be between 0 and 1");
            }
        }

        public static Color WaterSourcesColor(HexCell cell) {

            Water.CellManager manager = cell.waterManager;

            bool reservoir = false;
            bool river = false;

            if (manager.reservoirs.Count != 0) {
                reservoir = true;
            }

            if (cell.riverDistance == 1) {
                river = true;
            }

            if (reservoir) {
                if (river) {
                    return Color.yellow;
                }
                else {
                    return Color.red;
                }
            }
            else if (river) {
                return Color.green;
            }
            else {
                return Color.white;
            }
        }
    }
}

