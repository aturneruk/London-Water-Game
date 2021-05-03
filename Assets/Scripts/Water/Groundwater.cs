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
            Storage = new Water(1000000000, 1, 5000000000);
        }

        private float MaxDailyAbstraction {
            get {
                if (Storage.Level < 1e-6) {
                    return Storage.Volume;
                }
                else {
                    return maxAbstraction * Storage.Level * Storage.Level;
                }
            }
        }

        private float MaxMonthlyAbstraction {
            get {
                return MaxDailyAbstraction * 30;
            }
        }

        public float MaxAbstraction {
            get {
                if (Storage.Volume == 0) {
                    return 0;
                }
                else if (MaxMonthlyAbstraction > Storage.Volume) {
                    return Storage.Volume;
                }
                else {
                    return MaxMonthlyAbstraction;
                }
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
            if (abstraction <= MaxAbstraction) {
                Storage.Volume -= abstraction;
                return new Water(abstraction, Storage.Quality);
            }
            else {
                abstraction = MaxAbstraction;
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

