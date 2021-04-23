using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Stochasticity : MonoBehaviour
{
    public float RiverScarcity { get; private set; }

    private void Awake() {
        RiverScarcity = 1;
    }


    private void OnEnable() {
        GameTime.NewYear += UpdateScarcity;
    }

    private void OnDisable() {
        GameTime.NewYear -= UpdateScarcity;
          
    }

    private void UpdateScarcity() {
        RiverScarcity = 1;
    }


}
