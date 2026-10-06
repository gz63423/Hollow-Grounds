using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneScreenFader : MonoBehaviour
{
    public float fadeDuration = 1f;

    private SpriteRenderer sr;
    private bool isTransitioning = false;

    // ============================================================
    // AWAKE
    // ============================================================

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();

        SetAlpha(1f);
    }

    // ============================================================
    // START
    // ============================================================

    void Start()
    {
        StartCoroutine(FadeIn());
    }

    // ============================================================
    // LOAD SCENE BY NAME
    // ============================================================

    public void LoadScene(string sceneName)
    {
        if (!isTransitioning)
        {
            StartCoroutine(
                FadeOutAndLoad(sceneName)
            );
        }
    }

    // ============================================================
    // LOAD SCENE BY BUILD INDEX
    // ============================================================

    public void LoadScene(int sceneIndex)
    {
        if (!isTransitioning)
        {
            StartCoroutine(
                FadeOutAndLoad(sceneIndex)
            );
        }
    }

    // ============================================================
    // FADE OUT + LOAD BY NAME
    // ============================================================

    private IEnumerator FadeOutAndLoad(string sceneName)
    {
        isTransitioning = true;

        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;

            SetAlpha(
                Mathf.Lerp(
                    0f,
                    1f,
                    t / fadeDuration
                )
            );

            yield return null;
        }

        SetAlpha(1f);

        SceneManager.LoadScene(sceneName);
    }

    // ============================================================
    // FADE OUT + LOAD BY BUILD INDEX
    // ============================================================

    private IEnumerator FadeOutAndLoad(int sceneIndex)
    {
        isTransitioning = true;

        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;

            SetAlpha(
                Mathf.Lerp(
                    0f,
                    1f,
                    t / fadeDuration
                )
            );

            yield return null;
        }

        SetAlpha(1f);

        SceneManager.LoadScene(sceneIndex);
    }

    // ============================================================
    // FADE IN
    // ============================================================

    private IEnumerator FadeIn()
    {
        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;

            SetAlpha(
                Mathf.Lerp(
                    1f,
                    0f,
                    t / fadeDuration
                )
            );

            yield return null;
        }

        SetAlpha(0f);
    }

    // ============================================================
    // SET ALPHA
    // ============================================================

    void SetAlpha(float a)
    {
        if (sr == null)
            return;

        Color c = sr.color;

        c.a = a;

        sr.color = c;
    }
}