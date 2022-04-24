using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class speeeen : MonoBehaviour {
    public float angle;
    public float spinSpd;

    // Start is called before the first frame update
    void Start() {
        angle = 0;
    }

    // Update is called once per frame
    void Update() {
        angle += spinSpd * Time.deltaTime;
        angle %= 360;

        transform.eulerAngles = new Vector3(0,0,angle);
    }
}
