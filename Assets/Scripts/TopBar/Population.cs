using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TopBar {

    public class Population : MonoBehaviour {

        public Text population;
        public HexGrid grid;

        // Start is called before the first frame update
        void Start() {
        }


        // Update is called once per frame
        void Update() {
            population.text = "Population: " + grid.GetTotalPopulation().ToString();
        }
    }
}

