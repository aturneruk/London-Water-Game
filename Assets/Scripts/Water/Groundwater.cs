using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Water {

    public class Groundwater {

        private readonly CellManager manager;
        public Water Storage;
        private double maxAbstraction = 600000; // L/day
        private double maxInfiltration = 600000; // L/day

        public Groundwater(CellManager manager) {
            this.manager = manager;
            Storage = new Water(1000000000, 1, 5000000000);
        }

        private double MaxDailyAbstraction {
            get {
                if (Storage.Level < 1e-6) {
                    return Storage.Volume;
                }
                else {
                    return maxAbstraction * Storage.Level * Storage.Level;
                }
            }
        }

        private double MaxMonthlyAbstraction {
            get {
                return MaxDailyAbstraction * 30;
            }
        }

        public double MaxAbstraction {
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

        private double MaxDailyInfiltration {
            get {
                return maxInfiltration;
            }
        }

        private double MaxMonthlyInfiltration {
            get {
                return MaxDailyInfiltration * 30;
            }
        }

        public double MaxInfiltration {
            get {

                if (Storage.RemainingCapacity == null) {
                    return 0;
                }
                else {
                    if (MaxMonthlyInfiltration <= Storage.RemainingCapacity) {
                        return MaxMonthlyInfiltration;
                    }
                    else {
                        return (double)Storage.RemainingCapacity;
                    }
                }
            }
        }

        public Water Abstract(double abstraction) {
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

