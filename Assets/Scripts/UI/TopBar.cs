using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class TopBar : MonoBehaviour {

        public HexGrid grid;

        public bool paused;

        [SerializeField]
        private Button speedControl, playPauseButton;

        [SerializeField]
        private Text population, moneyBalance, date;

        [SerializeField]
        private CanvasGroup playImage, pauseImage;

        void Start() {
            paused = true;
            SwapToImage(playImage);

            Money.SetMoney(1000000);

            GameTime.Set(1800, Month.Jan, 1, Weekday.Mon);
            GameTime.SetSpeed(Speed.X1);
            speedControl.GetComponentInChildren<Text>().text = Speed.X1.ToString();
        }

        void Update() {

            if (!paused) {
                GameTime.UpdateTime();
            }

            population.text = "Population: " + grid.GetComponent<GridPopulation>().GetTotalPopulation().ToString();
            moneyBalance.text = Money.FormatMoney(Money.Balance);
            date.text = GameTime.GetLongForm();
            
        }

        public void ChangeSpeed() {

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
        }

        public void TogglePlayPause() {

            paused = !paused;

            if (paused) {
                SwapToImage(playImage);
            }
            else if (!paused) {
                SwapToImage(pauseImage);
            }
            else {
                throw new System.ArgumentNullException("The play/pause boolean must be defined");
            }
        }

        public void SwapToImage(CanvasGroup image) {
            image.alpha = 1f;
            image.interactable = true;
            image.blocksRaycasts = true;

            if (image == playImage) {
                pauseImage.alpha = 0f;
                pauseImage.interactable = false;
                pauseImage.blocksRaycasts = false;
            }
            else if (image == pauseImage) {
                playImage.alpha = 0f;
                playImage.interactable = false;
                playImage.blocksRaycasts = false;
            }
            else {
                throw new System.ArgumentOutOfRangeException("The image CanvasGroup must either be the play or pause image");
            }            
        }
    }
}