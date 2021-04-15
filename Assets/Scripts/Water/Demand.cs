using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Water {

    public class Demand {

        private int demandPerCapita;

        private readonly Manager manager;

        public Demand(Manager manager) {
            this.manager = manager;
            demandPerCapita = 20; // L/day
        }

        public float GetDemand {
            get {
                return demandPerCapita * manager.hexCell.Population.Size;
            }
        }
    }
}

