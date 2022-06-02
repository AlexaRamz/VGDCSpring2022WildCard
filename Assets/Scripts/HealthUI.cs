using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HealthUI : MonoBehaviour
{
    public Transform heartsLocation;
    public GameObject heartPrefab;
    public GameObject player;

    public float heartSpacing = 50f;
    private GameObject heartHolder;
    private List<GameObject> heartList = new List<GameObject>();
    private int heartCount;
    private Health playerHealth;


    // Start is called before the first frame update
    void Start()
    {
        playerHealth = player.GetComponent<Health>();
        heartCount = playerHealth.GetPlayerHealth();

        for (int i = 0; i < heartCount; i++)
        {
            Vector3 newPos = new Vector3(heartsLocation.position.x + (heartSpacing * i),
                heartsLocation.position.y, heartsLocation.position.z);

            heartHolder = Instantiate(heartPrefab);
            heartHolder.transform.SetParent(gameObject.transform, false);
            heartHolder.transform.position = newPos;

            //Debug.Log(heartHolder);
            heartList.Add(heartHolder);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResetHearts()
    {
        foreach (GameObject heart in heartList)
        {
            HeartDisplay heartDisplay = heart.GetComponent<HeartDisplay>();
            heartDisplay.TurnHeartRed();
        }
    }
    public void LoseAHeart()
    {

        heartCount = playerHealth.GetPlayerHealth();

        if (heartCount >= 0)
        {
            HeartDisplay heartDisplay = heartList[heartCount].GetComponent<HeartDisplay>();
            heartDisplay.TurnHeartGrey();



            //Destroy(heartList.Last());
            //heartList.Remove(heartList.Last());
        }
    }
}
