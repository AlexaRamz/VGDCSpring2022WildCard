using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileManager : MonoBehaviour
{
    public Transform launchPoint;
    public GameObject bulletPrefab;
    [SerializeField, Range(0f, 300f)] private float rotationSpeed = 10.0f;
    [SerializeField, Range(0f, 40f)] private float bulletSpeed = 20f;
    [SerializeField, Range(0f, 360f)] private float angle = 0f;
    public float shootTime = 0.5f;
    public float timeForDeath = 2.0f;
    public bool continuous;

    private float timer;

    public enum targetTag
    {
        Enemy,
        Player,
    }
    public targetTag target;

    // Start is called before the first frame update
    void Start()
    {
        timer = shootTime;
        StartCoroutine(Double(angle));
    }

    // Update is called once per frame
    void Update()
    {
        if (continuous)
        {
            if (timer > 0)
            {
                timer -= Time.deltaTime;
            }
            else
            {
                StartCoroutine(Double(angle));
                timer = shootTime;
            }

            launchPoint.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
        }
    }

    IEnumerator Double(float angle)
    {
        Shoot(angle);
        yield return new WaitForSeconds(0.15f);
        Shoot(angle);
    }
    public void Shoot(float rotAngle = 0)
    {
        StartCoroutine(ShootBullet(rotAngle));
    }
    private IEnumerator ShootBullet(float rotAngle = 0)
    {
        Vector3 rot = launchPoint.rotation.eulerAngles;
        rot = new Vector3(rot.x, rot.y, rot.z + rotAngle);
        GameObject bullet = Instantiate(bulletPrefab, launchPoint.position, Quaternion.Euler(rot));

        Bullet bulletInfo = bullet.GetComponent<Bullet>();
        bulletInfo.SetSpeed(bulletSpeed);
        bulletInfo.target = target.ToString();

        yield return new WaitForSeconds(timeForDeath);

        Destroy(bullet);
    }
}
