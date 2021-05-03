using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Water {

    public class RiverCell : MonoBehaviour {

        public int index;
        public RiverThames river;
        public HexCell hexCell;

        public RiverCell PreviousCell { get; set; }
        public RiverCell NextCell { get; set; }

        public List<CellManager> DischargeCells = new List<CellManager>();
        public List<CellManager> AbstractionCells = new List<CellManager>();

        public Water DischargeWater = new Water(0, 1);
        public Water AbstractionWater = new Water(0, 1);

        public Water Flow { get; private set; }

        public float MaxAbstraction {
            get {
                float maxAbstraction = 600000 * 30;

                if (index == 0) {
                    if ((river.GetInflow(this).Volume - AbstractionWater.Volume) == 0) {
                        maxAbstraction = 0;
                    }
                    else if (maxAbstraction > (river.GetInflow(this).Volume - AbstractionWater.Volume)) {
                        maxAbstraction = river.GetInflow(this).Volume - AbstractionWater.Volume;
                    }
                }
                else {
                    if ((PreviousCell.Flow.Volume - AbstractionWater.Volume) == 0) {
                        maxAbstraction = 0;
                    }
                    else if (maxAbstraction > (PreviousCell.Flow.Volume - AbstractionWater.Volume)) {
                        maxAbstraction = PreviousCell.Flow.Volume - AbstractionWater.Volume;
                    }
                }

                return maxAbstraction;
            }
        }

        private void Awake() {
            Flow = new Water(5200000000 * 30, 1);
        }

        public void UpdateFlow() {

            Flow = new Water(0, 1);

            if (PreviousCell) {
                Flow += PreviousCell.Flow;
            }

            Flow += river.GetInflow(this);

            foreach (CellManager manager in DischargeCells) {
                Flow += manager.wasteRouter.GetOverlandFlow();
            }

            Flow += DischargeWater;
            Flow -= AbstractionWater;

            DischargeWater.Volume = 0;
            AbstractionWater.Volume = 0;

            if (Flow.Volume < 0) {
                hexCell.MainColor = Color.white;
                throw new System.ArgumentOutOfRangeException("Negative river flow in river cell " + index + ". Flow is " + Flow.Volume);
            }
        }

        public void Discharge(Water discharge) {

            if (DischargeWater.Volume + discharge.Volume > 0) {
                DischargeWater.Quality = (DischargeWater.Product + discharge.Product) / (DischargeWater.Volume + discharge.Volume);
            }
            else {
                DischargeWater.Quality = 1;
            }

            DischargeWater.Volume += discharge.Volume;
        }

        public Water Abstract(float demand) {

            if (demand <= MaxAbstraction) {
                AbstractionWater.Volume += demand;
                return new Water(demand, Flow.Quality);
            }
            else {
                AbstractionWater.Volume += MaxAbstraction;
                return new Water(MaxAbstraction, Flow.Quality);
            }

            // return new Water(0, 1);
        }
    }
}