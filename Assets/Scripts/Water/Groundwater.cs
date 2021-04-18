using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Water {

    public class Groundwater {

        private readonly Manager manager;
        public Water Storage;
        private float maxAbstraction = 600000;

        public float Level {
            get {
                float level = 0.0002f * Storage.Volume / (float)manager.hexCell.area;

                if (level >= 0 && level <= 1) {
                    return level;
                }
                else {
                    throw new System.ArgumentException("The groundwater level must be between 0 and 1 inclusive, value is " + level + " in cell " + manager.hexCell.index);
                }
            }
        }

        public Groundwater(Manager manager) {
            this.manager = manager;
            Storage = new Water(5000000000, 1, true);
        }

        public float GetMaxAbstraction {
            get {
                return maxAbstraction * Level * Level;
            }
        }

        public Water Abstract(float abstraction) {
            if (abstraction <= GetMaxAbstraction) {
                Storage.Volume -= abstraction;
                return new Water(abstraction, Storage.Quality);
                }
            else {
                abstraction = GetMaxAbstraction;
                Storage.Volume -= abstraction;
                return new Water(abstraction, Storage.Quality);
            }
        }

        public void Infiltrate(Water input) {
            Storage.Quality = (Storage.Product + input.Product) / (Storage.Volume + input.Volume);
            Storage.Volume += input.Volume;
        }
    }
}

