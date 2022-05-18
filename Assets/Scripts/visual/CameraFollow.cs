using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public GameObject plr;
    public float offsetY = 2f;
    void Start()
    {
        
    }

    void Update()
    {
        transform.position = new Vector3(plr.transform.position.x, plr.transform.position.y + offsetY, transform.position.z);
    }
}
