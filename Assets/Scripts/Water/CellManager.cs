using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Water {
    public class CellManager : MonoBehaviour {

        public GridManager gridManager;
        public HexCell hexCell;
        private HexGrid hexGrid;

        public List<CellManager> overlandFlowPrevious = new List<CellManager>();
        public CellManager overlandFlowNext;

        public RiverCell riverAbstractionCell;
        public RiverCell riverDischargeCell;

        public Groundwater groundwater;
        private Demand waterDemand;
        public WasteRouter wasteRouter;

        public List<Reservoir> reservoirs = new List<Reservoir>();

        public Water Demand;
        private Water reducedDemand;
        public Water Supply;
        public float supplyRatio;

        public Water Sewage;

        public Water groundwaterSupply;
        private double maxGroundwaterAbstraction;

        public Water riverSupply;
        private double maxRiverAbstraction;

        public Water reservoirSupply;

        private void Start() {
            hexCell = gameObject.GetComponent<HexCell>();
            hexGrid = hexCell.GetComponentInParent<HexGrid>();

            groundwater = new Groundwater(this);
            waterDemand = new Demand(this);
            wasteRouter = new WasteRouter(this);
        }

        private void OnEnable() {
            GridManager.UpdateManager += UpdateWater;
        }

        private void OnDisable() {
            GridManager.UpdateManager -= UpdateWater;
        }

        public void UpdateWater() {
            Collate();
            Calculate();
            Distribute();
        }

        private void Collate() {

            if (hexCell.GetComponent<CellPopulation>()) {
                Demand = new Water(waterDemand.GetMonthlyDemand, 1);
                reducedDemand = Demand;
            }

            maxGroundwaterAbstraction = groundwater.MaxAbstraction;

            if (hexCell.riverDistance == 1 && riverAbstractionCell) {
                maxRiverAbstraction = riverAbstractionCell.MaxAbstraction;
            }

            // rainfall and pipe data to go here
        }

        private void Calculate() {

            reservoirSupply.Volume = 0;
            groundwaterSupply.Volume = 0;
            riverSupply.Volume = 0;

            bool complete = false;

            // stop if demand is met or allocation reaches last source
            while (reducedDemand.Volume > 0 && !complete) {

                Water supplied;

                // Do reservoirs first
                foreach (Reservoir reservoir in reservoirs) {
                    supplied = reservoir.Abstract(reducedDemand.Volume);
                    reservoirSupply += supplied;
                    reducedDemand.Volume -= supplied.Volume;
                }

                // Then prefer river vs groundwater based on quality (but prefer river if equal)

                if (riverAbstractionCell) {
                    if (riverAbstractionCell.flow.Quality >= groundwater.Storage.Quality) {
                        supplied = riverAbstractionCell.Abstract(reducedDemand.Volume);
                        riverSupply += supplied;
                        reducedDemand.Volume -= supplied.Volume;

                        supplied = groundwater.Abstract(reducedDemand.Volume);
                        groundwaterSupply += supplied;
                        reducedDemand.Volume -= supplied.Volume;
                    }
                    else {
                        supplied = groundwater.Abstract(reducedDemand.Volume);
                        groundwaterSupply += supplied;
                        reducedDemand.Volume -= supplied.Volume;

                        supplied = riverAbstractionCell.Abstract(reducedDemand.Volume);
                        riverSupply += supplied;
                        reducedDemand.Volume -= supplied.Volume;
                    }
                }
                else {
                    supplied = groundwater.Abstract(reducedDemand.Volume);
                    groundwaterSupply += supplied;
                    reducedDemand.Volume -= supplied.Volume;
                }

                if (riverSupply.Volume < 0) {
                    Debug.Log(hexCell.index);
                    Debug.Log(riverSupply.Volume);
                    Debug.Log(reducedDemand.Volume);
                }

                complete = true;
            }

            Supply = reservoirSupply + groundwaterSupply + riverSupply;

            if (Demand.Volume > 0) {
                supplyRatio = (float)(Supply.Volume / Demand.Volume);
            }
            else if (Demand.Volume == 0) {
                supplyRatio = 1;
            }
            else {
                throw new System.ArgumentOutOfRangeException("Demand is less than 0 in cell " + hexCell.index);
            }

            if (supplyRatio > 1 || supplyRatio < 0) {
                throw new System.ArgumentOutOfRangeException("The supply ratio in cell " + hexCell.index + " is out of bounds. Supply = " + Supply.Volume + ". Demand =  " + Demand.Volume);
            }

            Sewage = new Water(Supply.Volume, 0f);

            if (Sewage.Volume != Supply.Volume) {
                Debug.Log("error in cell " + hexCell.index);
            }

            wasteRouter.WasteInput(Sewage);
        }

        private void Distribute() {
            wasteRouter.DistributeWaste();
        }
    }
}


