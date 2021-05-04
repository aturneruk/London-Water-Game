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

        public Water Discharges = new Water(0, 1);
        public Water Abstractions = new Water(0, 1);

        public List<Water> Flows = new List<Water>();
        public Water flow;

        public double MaxAbstraction {
            get {
                double maxAbstraction = 600000 * 30;
                if (flow.Volume == 0) {
                    maxAbstraction = 0;
                }
                else if (maxAbstraction > flow.Volume) {
                    maxAbstraction = flow.Volume;
                }
                return maxAbstraction;
            }
        }

        public double ReservoirMaxAbstraction {
            get {
                double maxAbstraction = double.PositiveInfinity;
                if (flow.Volume == 0) {
                    maxAbstraction = 0;
                }
                else if (maxAbstraction > flow.Volume) {
                    maxAbstraction = flow.Volume;
                }
                return maxAbstraction;
            }
        }

        private void Awake() {
            flow = new Water(5200000000 * 30, 1);
        }

        public void UpdateFlow() {

            Abstractions.Volume = 0;
            Discharges.Volume = 0;

            if (NextCell) {
                flow = new Water(0, 1);
            }
            else {
                river.outflow += flow.Volume;
                flow = new Water(0, 1);
            }

            if (PreviousCell) {
                flow += PreviousCell.flow;
            }
            else {
                Water inflow = river.GetInflow(this);
                flow += inflow;
                river.inflow += inflow.Volume;
            }

            if (flow.Volume < 0) {
                hexCell.MainColor = Color.white;
                throw new System.ArgumentOutOfRangeException("Negative river flow in river cell " + index + ". Flow is " + flow.Volume);
            }
        }

        public void Discharge(Water discharge) {

            if (flow.Volume + discharge.Volume > 0) {
                flow.Quality = (flow.Product + discharge.Product) / (flow.Volume + discharge.Volume);
            }
            else {
                flow.Quality = 0;
            }

            flow.Volume += discharge.Volume;
            Discharges.Volume += discharge.Volume;
        }

        public Water Abstract(double demand) {

            if (demand <= MaxAbstraction) {
                flow.Volume -= demand;
                Abstractions.Volume += demand;
                return new Water(demand, flow.Quality);
            }
            else {
                flow.Volume -= MaxAbstraction;
                Abstractions.Volume += MaxAbstraction;
                return new Water(MaxAbstraction, flow.Quality);
            }
        }

        public Water ReservoirAbstract(double demand) {

            if (demand <= ReservoirMaxAbstraction) {
                flow.Volume -= demand;
                Abstractions.Volume += demand;
                return new Water(demand, flow.Quality);
            }
            else {
                flow.Volume -= ReservoirMaxAbstraction;
                Abstractions.Volume += ReservoirMaxAbstraction;
                return new Water(ReservoirMaxAbstraction, flow.Quality);
            }
        }

    }
}