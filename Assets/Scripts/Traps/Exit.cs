using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (activated) return;

        if (collision.GetComponent<PlayerController>() != null)
        {
            activated = true;
            StartCoroutine(ExitSequence());
        }
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
            objectToActivate2.SetActive(true);

        // Wait 1 more second and change scene
        yield return new WaitForSeconds(1f);

        if (objectToActivate != null)
            objectToActivate.SetActive(true);

        AnalyticsManager.Instance.EndLevel();

        if (!string.IsNullOrEmpty(sceneToLoad))
            SceneManager.LoadScene(sceneToLoad);
    }
}