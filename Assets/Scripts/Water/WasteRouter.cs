using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Water {

    public class WasteRouter {

        private Manager manager;
        private Groundwater groundwater;

        public Water waste;

        private float runoffCoefficient;

        public float RunoffCoefficient {
            get {
                return runoffCoefficient;
            }
            private set {
                if (value >= 0 && value <= 1) {
                    waste.Quality = value;
                }
                else {
                    throw new System.ArgumentException("The runoff coefficient must be between 0 and 1 inclusive, value is " + value + " in cell " + manager.hexCell.index);
                }
            }
        }

        public WasteRouter(Manager manager) {
            this.manager = manager;
            groundwater = manager.groundwater;

            waste.Volume = 0;
            waste.Quality = 1;
            RunoffCoefficient = 0;
        }

        public void AddWaste(Water input) {
            waste.Quality = (waste.Quality * waste.Volume + input.Volume * input.Quality) / (waste.Volume + input.Volume); // weighted average of all wastewater qualities
            waste.Volume += input.Volume;
        }

        public Water Infiltration {
            get {
                float volume = waste.Volume * RunoffCoefficient;
                float quality = waste.Quality;
                return new Water(volume, quality);
            }
        }

    }

}
