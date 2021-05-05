using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Water {
    public class GridManager : MonoBehaviour {

        HexGrid hexGrid;
        RiverThames riverThames;

        List<CellManager> cellManagers = new List<CellManager>();
        List<List<CellManager>> cellManagersByLevel = new List<List<CellManager>>();
        List<Reservoir> reservoirs = new List<Reservoir>();
        public List<RiverCell> riverCells = new List<RiverCell>();

        List<double> volumes = new List<double>();

        public delegate void ManagerAction();
        public static event ManagerAction UpdateManager;

        public void Awake() {
            hexGrid = gameObject.GetComponent<HexGrid>();
            riverThames = gameObject.GetComponent<RiverThames>();
        }

        private void OnEnable() {
            GameTime.NewMonth += HexGridWaterUpdate;
        }

        private void OnDisable() {
            GameTime.NewMonth -= HexGridWaterUpdate;
        }

        public void AddCellManager(HexCell cell) {
            CellManager cellManager = cell.waterManager = cell.gameObject.AddComponent<CellManager>();
            cell.waterManager.gridManager = this;
            cellManagers.Add(cellManager);
        }

        public void RemoveCellManager(CellManager manager) {
            cellManagers.Remove(manager);
            Destroy(manager);
        }

        public void AddCellManagerLevel(int level, List<CellManager> cellManagers) {

            if (level + 1 > cellManagersByLevel.Count) {
                int req = level - cellManagersByLevel.Count + 1;
                for (int i = 0; i < req; i++) {
                    cellManagersByLevel.Add(new List<CellManager>());
                }
            }

            cellManagersByLevel[level] = cellManagers;
        }

        public void AddCellManagerLevel(int level, List<HexCell> hexCells) {

            if (level + 1 > cellManagersByLevel.Count) {
                int req = level - cellManagersByLevel.Count + 1;
                for (int i = 0; i < req; i++) {
                    cellManagersByLevel.Add(new List<CellManager>());
                }
            }

            foreach (HexCell cell in hexCells) {
                cellManagersByLevel[level].Add(cell.waterManager);
            }
        }

        public void AddReservoir(Reservoir reservoir) {
            reservoirs.Add(reservoir);
        }

        public void HexGridWaterUpdate() {

            DailyRefresh();

            foreach (Reservoir reservoir in reservoirs) {
                reservoir.SetSupply();
            }

            // Update each individual CellManager now
            UpdateManager();

            foreach (Reservoir reservoir in reservoirs) {
                reservoir.SetStorage();
            }

            OverlandFlow();

            double volume = 0;

            for (int i = 0; i < cellManagers.Count; i++) {
                volume += cellManagers[i].wasteRouter.waste.Volume;
                volume += cellManagers[i].groundwater.Storage.Volume;
            }

            for (int i = 0; i < reservoirs.Count; i++) {
                volume += reservoirs[i].Storage.Volume;
            }

            for (int i = 0; i < riverCells.Count; i++) {
                volume += riverCells[i].flow.Volume;
            }

            if (volumes.Count > 100) {
                volumes.RemoveAt(0);
            }

            volumes.Add(volume);

            double inflow = riverThames.inflow;
            double outflow = riverThames.outflow;

            if (volumes.Count >= 2) {
                double deltaS = volumes[volumes.Count - 2] - volumes[volumes.Count - 1];
                double sum = deltaS + inflow - outflow;
                //Debug.Log("Change in storage: " + deltaS);
                if (Mathf.Abs((float)sum) > 1) {
                    Debug.LogError("Total delta: " + sum);
                }
            }
        }

        private void OverlandFlow() {

            foreach (CellManager manager in cellManagersByLevel[1]) {
                manager.riverDischargeCell.Discharge(manager.wasteRouter.GetOverlandFlow());
            }

            for (int i = 1; i < cellManagersByLevel.Count; i++) {
                foreach (CellManager manager in cellManagersByLevel[i]) {
                    foreach (CellManager previousManager in manager.overlandFlowPrevious) {
                        manager.wasteRouter.WasteInput(previousManager.wasteRouter.GetOverlandFlow());
                    }
                }
            }
        }

        public void DailyRefresh() {

            riverThames.inflow = 0;
            riverThames.outflow = 0;

            for (int i = riverCells.Count - 1; i >= 0; i--) {
                riverCells[i].UpdateFlow();
            }
        }

    }
}

