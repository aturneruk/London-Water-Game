using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Water {

    public class Demand {

        private int demandPerCapita;

        private readonly CellManager manager;

        public Demand(CellManager manager) {
            this.manager = manager;
            demandPerCapita = 20; // L/day
        }

        public double GetDailyDemand {
            get {
                return demandPerCapita * manager.hexCell.Population.Size;
            }            
        }

        public double GetMonthlyDemand {
            get {
                return GetDailyDemand * 30;
            }
        }
    }
}

