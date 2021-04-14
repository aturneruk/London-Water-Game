using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Water {

    public class Groundwater {

        private Manager manager;
        private float level;
        private float quality;
        private float volume;
        private float maxAbstraction = 600000;

        public float Level { 
            get {
                return level;
            }
            private set {
                if (value >= 0 && value <= 1) {
                    level = value;
                }
                else {
                    throw new System.ArgumentException("The groundwater level must be between 0 and 1 inclusive, value is " + value + " in cell " + manager.hexCell.index);
                }
            }
        }

        public float Quality {
            get {
                return quality;
            }
            private set {
                if (value >= 0 && value <= 1) {
                    quality = value;
                }
                else {
                    throw new System.ArgumentException("The quality must be between 0 and 1 inclusive, value is " + value + " in cell " + manager.hexCell.index);
                }
            }
        }

        public float Volume {
            get {
                return volume;
            }
            private set {
                if (value > 5000000000) {
                    throw new System.ArgumentException("Maximum volume is 5e9 L");
                }
                volume = value;
                Level = 0.0002f * volume / (float)manager.hexCell.area; 
            }
        }

        public Groundwater(Manager manager) {
            this.manager = manager;
            Volume = 5000000000; // L
            Quality = 1;
        }

        public float GetMaxAbstraction {
            get {
                return maxAbstraction * Level * Level;
            }
        }

        public Water Abstract(float abstraction) {
            if (abstraction <= GetMaxAbstraction) {
                Volume -= abstraction;
                return new Water(abstraction, Quality);
                }
            else {
                abstraction = GetMaxAbstraction;
                Volume -= abstraction;
                return new Water(abstraction, Quality);
            }
        }
    }
}

