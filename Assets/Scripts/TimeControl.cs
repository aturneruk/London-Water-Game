using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeControl : MonoBehaviour
{


    // Start is called before the first frame update
    void Start()
    {
        GameTime.Set(1800, (Month)0, 1);
        // Debug.Log(GameTime.Get());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
