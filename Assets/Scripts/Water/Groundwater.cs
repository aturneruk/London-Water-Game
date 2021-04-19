using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Water {

    public class Groundwater {

        private readonly Manager manager;
        public Water Storage;
        private float maxAbstraction = 600000;

        public Groundwater(Manager manager) {
            this.manager = manager;
            Storage = new Water(5000000000, 1, 5000000000);
        }

        public float GetMaxAbstraction {
            get {
                return maxAbstraction * Storage.Level * Storage.Level;
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

