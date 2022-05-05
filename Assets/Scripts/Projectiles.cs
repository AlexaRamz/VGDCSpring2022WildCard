using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectiles : MonoBehaviour
{
    public Transform launchPoint;
    public GameObject bulletPrefab;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        Instantiate(bulletPrefab, launchPoint.position, launchPoint.rotation);
    }
}
