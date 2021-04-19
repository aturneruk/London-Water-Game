using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Water {
    public class Manager : MonoBehaviour {

        public HexCell hexCell;
        public Groundwater groundwater;
        private Demand waterDemand;
        private WasteRouter wasteRouter;

        public float demandVolume;
        public Water Supply;
        public float supplyRatio;

        public Water Sewage;

        public Water groundwaterSupply;
        private float maxGroundwaterAbstraction;

        public string FormattedDemand {
            get {
                if (demandVolume < 10000) {
                    return Mathf.Round(demandVolume).ToString();
                }
                else if (demandVolume >= 10000 && demandVolume < 1000000) {
                    return (demandVolume / 1000f).ToString("G3") + "k";
                }
                else if (demandVolume >= 1000000) {
                    return (demandVolume / 1000000f).ToString("G3") + "M";
                }
                else {
                    throw new System.ArgumentException("Something has gone wrong formatting the demand to a string");
                }
            }
        }

        public string FormattedSupply {
            get {
                if (Supply.Volume < 10000) {
                    return Mathf.Round(Supply.Volume).ToString();
                }
                else if (Supply.Volume >= 10000 && Supply.Volume < 1000000) {
                    return (Supply.Volume / 1000f).ToString("G3") + "k";
                }
                else if (Supply.Volume >= 1000000) {
                    return (Supply.Volume / 1000000f).ToString("G3") + "M";
                }
                else {
                    throw new System.ArgumentException("Something has gone wrong formatting the supply to a string");
                }
            }
        }

        private void Awake() {
            hexCell = gameObject.GetComponent<HexCell>();
            groundwater = new Groundwater(this);
            waterDemand = new Demand(this);
            wasteRouter = new WasteRouter(this);
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
            demandVolume = waterDemand.GetDemand;

            maxGroundwaterAbstraction = groundwater.GetMaxAbstraction;

            groundwaterSupply.Quality = groundwater.Storage.Quality;

            // rainfall, river data and pipe data to go here
        }

        private void Calculate() {
            //if (maxGroundwaterAbstraction >= demand) {
            //    groundwaterSupply = groundwater.Abstract(demand);
            //}
            //else {
            //    groundwaterSupply = groundwater.Abstract(maxGroundwaterAbstraction);
            //}

            groundwaterSupply = groundwater.Abstract(demandVolume);
            Supply = groundwaterSupply; // + river + pipes

            if (demandVolume > 0) {
                supplyRatio = Supply.Volume / demandVolume;
            }
            else if (demandVolume == 0) {
                supplyRatio = 1;
            }
            else {
                throw new System.ArgumentOutOfRangeException("Demand is less than 0 in cell " + hexCell.index);
            }

            if (Supply.Volume > 0) {
                Supply.Quality = (groundwaterSupply.Product) / Supply.Volume; // weighted average of different source qualities
            }

            Sewage = new Water(Supply.Volume, 0f);
            wasteRouter.WasteInput(Sewage);            
        }

        private void Distribute() {
            wasteRouter.DistributeWaste();

            groundwaterSupply.Quality = groundwater.Storage.Quality;
        }
    }
}


