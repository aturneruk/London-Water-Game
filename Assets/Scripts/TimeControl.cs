using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TimeControl : MonoBehaviour
{
    public Text date;
    public Button speedControl;


    // Start is called before the first frame update
    void Start()
    {
        GameTime.Set(1800, (Month)0, 1);
        GameTime.SetSpeed(Speed.X1);
        // Debug.Log(GameTime.Get());
    }

    // Update is called once per frame
    void Update()
    {
        GameTime.UpdateTime();
        date.text = GameTime.GetShortForm();
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
