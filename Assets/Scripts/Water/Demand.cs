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

        public float GetDailyDemand {
            get {
                return demandPerCapita * manager.hexCell.Population.Size;
            }            
        }

        public float GetMonthlyDemand {
            get {
                return GetDailyDemand * 30;
            }
        }
    }
}

