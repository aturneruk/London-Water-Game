using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI {

    public class Population : MonoBehaviour {

        public Text population;
        public HexGrid grid;

        void Update() {
            population.text = "Population: " + grid.GetTotalPopulation().ToString();
        }
    }
}
