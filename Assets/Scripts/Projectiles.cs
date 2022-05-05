using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectiles : MonoBehaviour
{
    public Transform launchPoint;
    public GameObject bulletPrefab;
    [SerializeField, Range(0f, 300f)] private float rotationSpeed = 10.0f;
    [SerializeField, Range(0f, 20f)] private float bulletSpeed = 20f;
    public float shootTime = 0.5f;
    public float timeForDeath = 2.0f;

    private float timer;
    private Bullet bulletInfo;


    // Start is called before the first frame update
    void Start()
    {
        bulletInfo = bulletPrefab.GetComponent<Bullet>();

        timer = shootTime;
        Shoot();
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown("space"))
        //{
        //    Shoot();
        //}

        bulletInfo.SetSpeed(bulletSpeed);

        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
        else
        {
            StartCoroutine(Shoot());
            timer = shootTime;
        }

        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
    }

    private IEnumerator Shoot()
    {
        Vector3 rot = launchPoint.rotation.eulerAngles;
        rot = new Vector3(rot.x, rot.y, rot.z + 180);

        GameObject bullet = Instantiate(bulletPrefab, launchPoint.position, launchPoint.rotation);
        GameObject bulletTwo = Instantiate(bulletPrefab, launchPoint.position, Quaternion.Euler(rot));

        yield return new WaitForSeconds(timeForDeath);

        Destroy(bullet);
        Destroy(bulletTwo);
    }
}
