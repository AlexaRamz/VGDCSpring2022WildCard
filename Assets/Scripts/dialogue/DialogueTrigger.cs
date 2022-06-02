using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    bool inRange = false;
    public Dialogue[] dialogues;
    DialogueManager dialogueSys;
    public bool randomized;
    void Start()
    {
        dialogueSys = FindObjectOfType<DialogueManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            inRange = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            inRange = false;
        }
    }
    void Update()
    {
        if (inRange)
        {
            if (Input.GetKeyDown(KeyCode.Return) && dialogueSys.talking == false)
            {
                Dialogue dialogue = dialogues[0];
                if (randomized && dialogues.Length > 1)
                {
                    dialogue = dialogues[Random.Range(0, dialogues.Length)];
                }
                dialogueSys.StartDialogue(dialogue);
                dialogueSys.currentTrigger = this;
            }
        }
    }
}
