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

        public Water DischargeWater = new Water(0, 1);
        public Water AbstractionWater = new Water(0, 1);

        public Water Flow { get; private set; }

        public float MaxAbstraction {
            get {
                float maxAbstraction = 600000 * 30;

                if (index == 0) {
                    if (maxAbstraction > river.GetInflow(this).Volume) {
                        maxAbstraction = river.GetInflow(this).Volume;
                    }
                }
                else {
                    if (maxAbstraction > PreviousCell.Flow.Volume) {
                        maxAbstraction = PreviousCell.Flow.Volume;
                    }
                }                

                return maxAbstraction;
            }
        }

        private void Awake() {
            Flow = new Water(5200000000 * 30, 1);
        }

        public void UpdateFlow() {

            Flow = river.GetInflow(this);

            if (PreviousCell) {
                Flow += PreviousCell.Flow;
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