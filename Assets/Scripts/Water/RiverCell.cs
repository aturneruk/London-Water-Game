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

        private Water[] flow = new Water[2];

        private Water inflow;

        public Water Flow {
            get {
                if (flow[0].Volume != 0) {
                    return flow[0];
                }
                else if (flow[1].Volume != 0) {
                    return flow[1];
                }
                else {
                    return new Water(0, 0);
                }
            }
        }

        private void Awake() {

            flow[0] = new Water(5200000000, 1);
            flow[1] = new Water(0, 1);

        }

        public void UpdateFlow(bool toggle) {

            Discharge(river.GetInflow(this));

            if (toggle == true) {
                if (index > 0) {
                    flow[1] = PreviousCell.flow[0];
                    PreviousCell.flow[0] = new Water(0, 1);
                }
                flow[1].Volume += inflow.Volume;
                if (flow[1].Volume + inflow.Volume > 0) {
                    flow[1].Quality = (flow[1].Product + inflow.Product) / (flow[1].Volume + inflow.Volume);
                }
            }
            else if (toggle == false) {
                if (index > 0) {
                    flow[0] = PreviousCell.flow[1];
                    PreviousCell.flow[1] = new Water(0, 1);
                }
                flow[0].Volume += inflow.Volume;
                if (flow[0].Volume + inflow.Volume > 0) {
                    flow[0].Quality = (flow[0].Product + inflow.Product) / (flow[0].Volume + inflow.Volume);
                }
            }
            else {
                throw new System.ArgumentNullException("The toggle must be either true or false;");
            }            

            inflow.Volume = 0;

            if (flow[0].Volume == 0 && flow[1].Volume == 0) {
                hexCell.MainColor = Color.white;
            }
        }

        public void Discharge(Water discharge) {           


            if (inflow.Volume + discharge.Volume > 0) {
                inflow.Quality = (inflow.Product + discharge.Product) / (inflow.Volume + discharge.Volume);
            }
            else {
                inflow.Quality = 1;
            }

            inflow.Volume += discharge.Volume;
        }

        //public Water Abstract(float demand) {

        //    if (demand <= Water.Volume) {
        //        return new Water(demand, Water.Quality);
        //    }
        //    else {
        //        return new Water(Water.Volume, Water.Quality);
        //    }
        //}
    }
}