using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class TopBar : MonoBehaviour {

        public Text date;
        public Text population;

        public Button speedControl;

        public HexGrid grid;

        // Start is called before the first frame update
        void Start() {
            GameTime.Set(1800, Month.Jan, 1, Weekday.Mon);
            GameTime.SetSpeed(Speed.X1);
            speedControl.GetComponentInChildren<Text>().text = Speed.X1.ToString();
            // Debug.Log(GameTime.Get());
        }

        // Update is called once per frame
        void Update() {
            GameTime.UpdateTime();
            date.text = GameTime.GetLongForm();

            population.text = "Population: " + grid.GetTotalPopulation().ToString();
        }

        public void SpeedChange() {

            Speed speed;

            if (GameTime.GetSpeed() == Speed.X8) {
                speed = Speed.X1;
                GameTime.SetSpeed(speed);
            }
            else {
                speed = GameTime.GetSpeed();
                GameTime.SetSpeed(++speed);
            }

            speedControl.GetComponentInChildren<Text>().text = speed.ToString();

            Sprite image = speedControl.GetComponent<Image>().sprite;
        }
    }
}