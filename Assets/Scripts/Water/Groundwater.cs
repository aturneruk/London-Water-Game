using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Water {

    public class Groundwater {

        private readonly CellManager manager;
        public Water Storage;
        private float maxAbstraction = 600000; // L/day
        private float maxInfiltration = 600000; // L/day

        public Groundwater(CellManager manager) {
            this.manager = manager;
            Storage = new Water(1000000000, 1, 5000000000);
        }

        public float MaxDailyAbstraction {
            get {

                if (maxAbstraction * Storage.Level * Storage.Level >= Storage.Volume) {
                    return maxAbstraction * Storage.Level * Storage.Level;
                }
                else {
                    return Storage.Volume;
                }
            }
        }

        public float MaxMonthlyAbstraction {
            get {
                return MaxDailyAbstraction * 30;
            }
        }

        private float MaxDailyInfiltration {
            get {
                return maxInfiltration;

            }
        }

        private float MaxMonthlyInfiltration {
            get {
                return MaxDailyInfiltration * 30;

            }
        }

        public float MaxInfiltration {
            get {

                if (Storage.RemainingCapacity == null) {
                    return 0;
                }
                else {
                    if (MaxMonthlyInfiltration <= Storage.RemainingCapacity) {
                        return MaxMonthlyInfiltration;
                    }
                    else {
                        return (float)Storage.RemainingCapacity;
                    }
                }
            }
        }

        public Water Abstract(float abstraction) {
            if (abstraction <= MaxMonthlyAbstraction) {

                Storage.Volume -= abstraction;
                return new Water(abstraction, Storage.Quality);
                }
            else {
                abstraction = MaxMonthlyAbstraction;
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

