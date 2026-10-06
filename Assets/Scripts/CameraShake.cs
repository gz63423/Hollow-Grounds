using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }


    [Header("Hit Shake")]
    public float hitShakeDuration = 0.08f;
    public float hitShakeStrength = 0.08f;


    private Vector3 originalLocalPosition;
    private Coroutine shakeCoroutine;


    void Awake()
    {
        if (Instance != null &&
            Instance != this)
        {
            Destroy(gameObject);
            return;
        }


        Instance = this;


        originalLocalPosition =
            transform.localPosition;
    }


    public void ShakeOnHit()
    {
        Shake(
            hitShakeDuration,
            hitShakeStrength
        );
    }


    public void Shake(
        float duration,
        float strength)
    {
        if (shakeCoroutine != null)
        {
            StopCoroutine(
                shakeCoroutine
            );
        }


        shakeCoroutine =
            StartCoroutine(
                ShakeRoutine(
                    duration,
                    strength
                )
            );
    }


    private IEnumerator ShakeRoutine(
        float duration,
        float strength)
    {
        float elapsed = 0f;


        while (elapsed < duration)
        {
            float progress =
                elapsed / duration;


            // Strong at the beginning,
            // smoothly fades out.
            float currentStrength =
                Mathf.Lerp(
                    strength,
                    0f,
                    progress
                );


            Vector2 offset =
                Random.insideUnitCircle *
                currentStrength;


            transform.localPosition =
                originalLocalPosition +
                new Vector3(
                    offset.x,
                    offset.y,
                    0f
                );


            elapsed +=
                Time.deltaTime;


            yield return null;
        }


        transform.localPosition =
            originalLocalPosition;


        shakeCoroutine =
            null;
    }
}

