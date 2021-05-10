using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics.Distributions;

public static class Stochasticity {
    private static Gamma riverFlows = new Gamma(8.22, 0.0253);

    public static double RiverFlowMultiplier {
        get {
            return riverFlows.Sample() / 325.1d;
         }
    }
}
