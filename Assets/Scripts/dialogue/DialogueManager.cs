using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    Dialogue currentDialogue;
    Queue<string> sentences;

    public int maxLetters = 1;

    public Canvas textBox;
    public Text textDisplay;
    IEnumerator currentCoroutine;

    public bool talking = false;
    bool typing;
    bool responding;
    string currentSentence;
    Sprite finalIcon;
    public float typeSpeed = 0.04f;
    public float pauseTime = 0.8f;

    public DialogueTrigger currentTrigger;

    void Start()
    {
        sentences = new Queue<string>();
    }

    public void StartDialogue(Dialogue dialogue)
    {
        currentDialogue = dialogue;
        StartCoroutine(StartDelay());
        sentences.Clear();
        textDisplay.text = "";
        textBox.enabled = true;

        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }
        DisplayNextSentence();
    }
    IEnumerator StartDelay()
    {
        yield return new WaitForSeconds(0.1f);
        talking = true;
    }
    IEnumerator EndDelay()
    {
        yield return new WaitForSeconds(0.1f);
        talking = false;
    }
    IEnumerator TypeText(string sentence)
    {
        typing = true;
        int n = maxLetters;
        var lines = sentence.Split('\n');
        List<string> newLines = new List<string>();
        foreach (string line in lines)
        {
            string newLine = line;
            if (n != 0 && line.Length > n)
            {
                bool found = false;
                for (int i = 0; i < n; i++)
                {
                    if (line.Substring(n - i, 1) == " " && found == false)
                    {
                        newLine = newLine.Insert(n - i + 1, "\n");
                        found = true;
                    }
                }
            }
            newLines.Add(newLine);
        }
        string newSentence = string.Join("\n", newLines);
        currentSentence = newSentence;
        int lineCount = 0;
        foreach (string line in newLines)
        {
            int charIndex = 0;
            string originalText = textDisplay.text;
            while (charIndex < line.Length)
            {
                yield return new WaitForSeconds(typeSpeed);
                //Add newline at the last space before max letters reached

                charIndex += 1;
                textDisplay.text = originalText + line.Substring(0, charIndex);
            }
            lineCount += 1;
            if (lineCount != newLines.Count)
            {
                textDisplay.text = originalText + line.Substring(0, charIndex) + "\n";
                yield return new WaitForSeconds(pauseTime);
            }
        }
        typing = false;
    }
    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }
        textDisplay.text = "";
        string sentence = sentences.Dequeue();
        currentSentence = sentence;
        currentCoroutine = TypeText(sentence);
        StartCoroutine(currentCoroutine);
    }

    void EndDialogue()
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }
        textDisplay.text = "";
        currentSentence = null;
        textBox.enabled = false;
        StartCoroutine(EndDelay());
        currentTrigger = null;

        currentDialogue = null;
    }
    void Update()
    {
        if (talking && Input.GetKeyDown(KeyCode.Return))
        {
            if (typing)
            {
                if (currentCoroutine != null)
                {
                    StopCoroutine(currentCoroutine);
                }
                typing = false;
                textDisplay.text = currentSentence;
            }
            else
            {
                DisplayNextSentence();
            }
        }
    }
}
