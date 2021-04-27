using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Water {
    public class GridManager : MonoBehaviour {

        List<CellManager> cellManagers = new List<CellManager>();

        List<Reservoir> reservoirs = new List<Reservoir>();

        public void AddCellManager(HexCell cell) {

            CellManager cellManager = cell.waterManager = cell.gameObject.AddComponent<CellManager>();
            cell.waterManager.gridManager = this;
            cellManagers.Add(cellManager);

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

            foreach (Reservoir reservoir in reservoirs) {
                reservoir.DistributeWater();
            }
            UpdateManager();
        }
    }
}

