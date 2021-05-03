using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Water {
    public class GridManager : MonoBehaviour {

        List<CellManager> cellManagers = new List<CellManager>();

        List<List<CellManager>> cellManagersByLevel = new List<List<CellManager>>();

        List<Reservoir> reservoirs = new List<Reservoir>();

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


        private void OnEnable() {
            GameTime.NewMonth += HexGridWaterUpdate;
        }

        private void OnDisable() {
            GameTime.NewMonth -= HexGridWaterUpdate;
        }

        public delegate void ManagerAction();
        public static event ManagerAction UpdateManager;

        public void HexGridWaterUpdate() {

            gameObject.GetComponent<RiverThames>().DailyRefresh();
            OverlandFlow();

            foreach (Reservoir reservoir in reservoirs) {
                reservoir.DistributeWater();
            }

            float[] qualities = new float[cellManagers.Count];
            float[] levels = new float[cellManagers.Count];

            for (int i = 0; i < cellManagers.Count; i++) {
                qualities[i] = cellManagers[i].groundwater.Storage.Quality;
                levels[i] = cellManagers[i].groundwater.Storage.Level;
            }

            Debug.Log(Mathf.Min(qualities));
            Debug.Log(Mathf.Min(levels));


            // Update each individual CellManager now
            UpdateManager();

        }

        private void OverlandFlow() {

            foreach (List<CellManager> sublist in cellManagersByLevel) {
                foreach (CellManager manager in sublist) {
                    foreach (CellManager previousManager in manager.overlandFlowPrevious) {
                        manager.wasteRouter.WasteInput(previousManager.wasteRouter.GetOverlandFlow());
                    }
                }
            }
        }
    }
}

