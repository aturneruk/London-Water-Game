using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Water {

    public class WasteRouter {

        private readonly CellManager manager;
        private readonly Groundwater groundwater;

        public Water waste;

        private double runoffCoefficient;

        public double RunoffCoefficient {
            get {
                return runoffCoefficient;
            }
            private set {
                if (value >= 0 && value <= 1) {
                    runoffCoefficient = value;
                }
                else {
                    throw new System.ArgumentException("The runoff coefficient must be between 0 and 1 inclusive, value is " + value + " in cell " + manager.hexCell.index);
                }
            }
        }

        public WasteRouter(CellManager manager) {
            this.manager = manager;
            groundwater = manager.groundwater;

            waste.Volume = 0;
            waste.Quality = 1;
            RunoffCoefficient = 0.2f;
        }

        public void WasteInput(Water input) {

            if (waste.Volume + input.Volume != 0) {
                waste.Quality = (waste.Product + input.Product) / (waste.Volume + input.Volume); // weighted average of all wastewater qualities
            }
            else {
                waste.Quality = 1;
            }

            waste.Volume += input.Volume;

            //if (input.Volume != waste.Volume) {
            //    Debug.Log("error in cell " + manager.hexCell.index);
            //}
        }

        public void DistributeWaste() {

            double infiltrationVolume = waste.Volume * (1f - RunoffCoefficient);
            double maxInfiltration = groundwater.MaxInfiltration;

            if (infiltrationVolume > maxInfiltration) {
                infiltrationVolume = maxInfiltration;
            }

            if (infiltrationVolume != 0) {
                Water infiltration = new Water(infiltrationVolume, waste.Quality);
                groundwater.Infiltrate(infiltration);
                waste.Volume -= infiltration.Volume;
            }
        }

        public Water GetOverlandFlow() {

            Water flow = new Water(waste.Volume, waste.Quality);
            waste.Volume = 0;
            return flow;
        }
    }
}
