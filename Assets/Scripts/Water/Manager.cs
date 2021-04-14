using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Water {
    public class Manager : MonoBehaviour {

        public HexCell hexCell;
        private Groundwater groundwater;
        private Demand waterDemand;

        public float demand;
        public float supply;
        public float supplyRatio;
        public float supplyQuality;

        public string FormattedDemand {
            get {
                if (demand < 10000) {
                    return Mathf.Round(demand).ToString();
                }
                else if (demand >= 10000 && demand < 1000000) {
                    return (demand / 1000f).ToString("G3") + "k";
                }
                else if (demand >= 1000000) {
                    return (demand / 1000000f).ToString("G3") + "M";
                }
                else {
                    throw new System.ArgumentException("Something has gone wrong formatting the demand to a string");
                }
            }
        }

        public string FormattedSupply {
            get {
                if (supply < 10000) {
                    return Mathf.Round(supply).ToString();
                }
                else if (supply >= 10000 && supply < 1000000) {
                    return (supply / 1000f).ToString("G3") + "k";
                }
                else if (supply >= 1000000) {
                    return (supply / 1000000f).ToString("G3") + "M";
                }
                else {
                    throw new System.ArgumentException("Something has gone wrong formatting the supply to a string");
                }
            }
        }

        public float groundwaterLevel;
        public float groundwaterQuality;
        private float groundwaterSupply;
        private float maxGroundwaterAbstraction;


        private void Awake() {
            hexCell = gameObject.GetComponent<HexCell>();
            groundwater = new Groundwater(this);
            waterDemand = new Demand(this);
        }

        private void OnEnable() {
            GameTime.NewDay += UpdateWater;
        }

        private void OnDisable() {
            GameTime.NewDay -= UpdateWater;
        }

        public void UpdateWater() {
            Collate();
            Calculate();
            Distribute();
        }

        private void Collate() {
            demand = waterDemand.GetDemand;

            maxGroundwaterAbstraction = groundwater.GetMaxAbstraction;

            groundwaterQuality = groundwater.Quality;

            // rainfall, river data and pipe data to go here
        }

        private void Calculate() {
            //if (maxGroundwaterAbstraction >= demand) {
            //    groundwaterSupply = groundwater.Abstract(demand);
            //}
            //else {
            //    groundwaterSupply = groundwater.Abstract(maxGroundwaterAbstraction);
            //}

            groundwaterSupply = groundwater.Abstract(demand);
            supply = groundwaterSupply; // + river + pipes

            supplyRatio = supply / demand;

            supplyQuality = (groundwaterSupply * groundwaterQuality) / supply; // weighted average of different source qualities
        }

        private void Distribute() {
            groundwaterLevel = groundwater.Level;
            groundwaterQuality = groundwater.Quality;
        }

    }
}


