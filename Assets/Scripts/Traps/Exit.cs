using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Exit : MonoBehaviour
{
    [Header("Player Reference")]
    public PlayerController player;

    [Header("Animator")]
    [SerializeField] private Animator animator;

    [Header("Objects")]
    [SerializeField] private GameObject objectToActivate;
    [SerializeField] private GameObject objectToActivate2;

    [Header("Scene")]
    [SerializeField] private string sceneToLoad;

    private bool activated;

    private void Start()
    {
        StartCoroutine(FadeOut(objectToActivate2, 1f));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (activated) return;

        if (collision.GetComponent<PlayerController>() != null)
        {
            collision.GetComponent<PlayerController>().controlsLocked = true;
            activated = true;
            StartCoroutine(ExitSequence());
        }
    }

    private IEnumerator FadeIn(GameObject obj, float duration)
    {
        Image img = obj.GetComponent<Image>();
        if (img == null) yield break;

        Color c = img.color;
        c.a = 0f;
        img.color = c;

        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float normalized = t / duration;

            c.a = Mathf.Lerp(0f, 1f, normalized);
            img.color = c;

            yield return null;
        }

        c.a = 1f;
        img.color = c;
    }

    private IEnumerator FadeOut(GameObject obj, float duration)
    {
        Image img = obj.GetComponent<Image>();
        if (img == null) yield break;

        Color c = img.color;
        float startAlpha = c.a;

        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float normalized = t / duration;

            c.a = Mathf.Lerp(startAlpha, 0f, normalized);
            img.color = c;

            yield return null;
        }

        c.a = 0f;
        img.color = c;
    }


    private IEnumerator ExitSequence()
    {
        // Blinking event
        if (player != null)
            player.Blinking();

        // Disable player completely
        if (player != null)
            player.enabled = false;

        // Play animation
        if (animator != null)
            animator.enabled = true;

        if (objectToActivate != null)
        {
            objectToActivate.SetActive(true);

            TMP_Text tmp = objectToActivate.GetComponentInChildren<TMP_Text>();
            if (tmp != null)
            {
                int lives = GameManager.Instance != null ? GameManager.Instance.playerLives : 0;
                tmp.text = $"LEVEL COMPLETE\nWITH {lives} LIVES";
            }
        }

        // Wait 5 seconds and spawn prefab
        yield return new WaitForSeconds(5f);

        if (objectToActivate2 != null)
        {
            objectToActivate2.SetActive(true);
            StartCoroutine(FadeIn(objectToActivate2, 1f));
        }

        // Wait 1 more second and change scene
        yield return new WaitForSeconds(2f);

        if (objectToActivate != null)
            objectToActivate.SetActive(true);

        AnalyticsManager.Instance.EndLevel();

        if (!string.IsNullOrEmpty(sceneToLoad))
            SceneManager.LoadScene(sceneToLoad);
    }
}