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
        private float maxGroundwaterAbstraction;

        public Water riverSupply;
        private float maxRiverAbstraction;

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
            Demand = new Water(waterDemand.GetMonthlyDemand, 1);
            reducedDemand = Demand;

            maxGroundwaterAbstraction = groundwater.MaxAbstraction;
            groundwaterSupply.Quality = groundwater.Storage.Quality;

            if (hexCell.riverDistance == 1 && riverAbstractionCell) {
                maxRiverAbstraction = riverAbstractionCell.MaxAbstraction;
                riverSupply.Quality = riverAbstractionCell.Flow.Quality;
            }

            // rainfall and pipe data to go here
        }

        private void Calculate() {

            reservoirSupply.Volume = 0;
            groundwaterSupply.Volume = 0;
            riverSupply.Volume = 0;

            bool complete = false;

            // stop if demand is met or allocation reaches end
            while (reducedDemand.Volume > 0 && !complete) {

                // Do reservoirs first
                foreach (Reservoir reservoir in reservoirs) {
                    Water supplied = reservoir.Abstract(reducedDemand);
                    reservoirSupply += supplied;
                    reducedDemand.Volume -= supplied.Volume;
                }

                // Then prefer river vs groundwater based on quality (but prefer river)
                if (riverSupply.Quality >= groundwaterSupply.Quality && riverAbstractionCell) {

                    Water supplied = riverAbstractionCell.Abstract(reducedDemand.Volume);
                    riverSupply += supplied;
                    reducedDemand.Volume -= supplied.Volume;

                    if (riverSupply.Volume < 0) {
                        Debug.Log(hexCell.index);
                        Debug.Log(riverSupply.Volume);
                        Debug.Log(reducedDemand.Volume);
                    }

                    supplied = groundwater.Abstract(reducedDemand.Volume);
                    groundwaterSupply += supplied;
                    reducedDemand.Volume -= supplied.Volume;                  
                }
                else {

                    Water supplied = groundwater.Abstract(reducedDemand.Volume);
                    groundwaterSupply += supplied;
                    reducedDemand.Volume -= supplied.Volume;

                    if (riverAbstractionCell) {
                        supplied = riverAbstractionCell.Abstract(reducedDemand.Volume);
                        riverSupply += supplied;
                        reducedDemand.Volume -= supplied.Volume;
                    }
                }

                complete = true;
            }

            Supply = reservoirSupply + groundwaterSupply + riverSupply;

            if (Demand.Volume > 0) {
                supplyRatio = Supply.Volume / Demand.Volume;
            }
            else if (Demand.Volume == 0) {
                supplyRatio = 1;
            }
            else {
                throw new System.ArgumentOutOfRangeException("Demand is less than 0 in cell " + hexCell.index);
            }

            Sewage = new Water(Supply.Volume, 0f);

            if (Sewage.Volume != Supply.Volume) {
                Debug.Log("error in cell " + hexCell.index);
            }

            wasteRouter.WasteInput(Sewage);
        }

        private void Distribute() {
            wasteRouter.DistributeWaste();

            groundwaterSupply.Quality = groundwater.Storage.Quality;
        }
    }
}


