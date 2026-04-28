using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class TutorialTextController : MonoBehaviour
{
    [Header("Video")]
    public VideoPlayer videoPlayer;

    [Header("UI References")]
    public TMP_Text mechanicNameText;
    public TMP_Text inputText;

    [Header("End Transition")]
    public GameObject endObject;
    private bool hasFinished = false;

    [Header("Typing Settings")]
    public float typingSpeed = 0.03f;

    private Coroutine typingCoroutine;
    private int currentStep = -1;

    private string[] mechanicNames =
    {
        "THE RITUAL OF ARROW",
        "THE RITUAL OF BOMB",
        "THE RITUAL OF STONE"
    };

    private string[] inputs =
    {
        "PRESS LEFT SHIFT",
        "PRESS E",
        "PRESS Q"
    };

    private void Update()
    {
        if (!videoPlayer.isPlaying)
            return;

        float t = (float)videoPlayer.time;

        // STEP 0 (0 - 4 sec)
        if (t >= 0f && t < 4f && currentStep != 0)
        {
            currentStep = 0;
            ShowStep(0);
        }

        // STEP 1 (4 - 8 sec)
        else if (t >= 4f && t < 8f && currentStep != 1)
        {
            currentStep = 1;
            ShowStep(1);
        }

        // STEP 2 (8 - 14 sec)
        else if (t >= 8f && t < 14f && currentStep != 2)
        {
            currentStep = 2;
            ShowStep(2);
        }

        // END (after 14 sec)
        if (t >= 14f && !hasFinished)
        {
            hasFinished = true;
            StartCoroutine(EndSequence());
        }
    }

    private IEnumerator EndSequence()
    {
        // Activate object
        if (endObject != null)
            endObject.SetActive(true);

        // Wait 1 second
        yield return new WaitForSeconds(1f);

        // Load next scene
        SceneManager.LoadScene("Level 1");
    }

    public void ShowStep(int index)
    {
        if (index < 0 || index >= mechanicNames.Length)
            return;

        StopCurrentTyping();
        typingCoroutine = StartCoroutine(PlayStep(index));
    }

    private IEnumerator PlayStep(int index)
    {
        yield return TypeText(mechanicNameText, mechanicNames[index]);

        yield return new WaitForSeconds(0.15f);

        yield return TypeText(inputText, inputs[index]);
    }

    private IEnumerator TypeText(TMP_Text textComponent, string content)
    {
        textComponent.text = "";

        for (int i = 0; i < content.Length; i++)
        {
            textComponent.text += content[i];
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    private void StopCurrentTyping()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = null;
    }

    public void ClearText()
    {
        StopCurrentTyping();
        mechanicNameText.text = "";
        inputText.text = "";
        currentStep = -1;
        hasFinished = false;
    }
}