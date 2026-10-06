using UnityEngine;
using System.Collections;

public class EnemyHitFlash : MonoBehaviour
{
    [Header("Hit Flash")]
    public Material hitFlashMaterial;
    public float flashDuration = 0.08f;

    private SpriteRenderer[] spriteRenderers;
    private Material[][] originalMaterials;

    private Coroutine flashCoroutine;

    private void Awake()
    {
        // Find every SpriteRenderer on this enemy and its children
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);

        // Store each renderer's original materials
        originalMaterials = new Material[spriteRenderers.Length][];

        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            if (spriteRenderers[i] != null)
            {
                originalMaterials[i] = spriteRenderers[i].sharedMaterials;
            }
        }
    }

    public void Flash()
    {
        if (hitFlashMaterial == null)
        {
            Debug.LogWarning(
                $"EnemyHitFlash on {gameObject.name} has no Hit Flash Material assigned."
            );

            return;
        }

        // If already flashing, restart the flash
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
        }

        flashCoroutine = StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        // Force every sprite to use the hit flash material
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            if (spriteRenderers[i] == null)
                continue;

            Material[] flashMaterials = new Material[
                spriteRenderers[i].sharedMaterials.Length
            ];

            for (int j = 0; j < flashMaterials.Length; j++)
            {
                flashMaterials[j] = hitFlashMaterial;
            }

            spriteRenderers[i].sharedMaterials = flashMaterials;
        }

        // Keep the flash active
        yield return new WaitForSeconds(flashDuration);

        // Restore original materials
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            if (spriteRenderers[i] == null)
                continue;

            if (originalMaterials[i] != null)
            {
                spriteRenderers[i].sharedMaterials = originalMaterials[i];
            }
        }

        flashCoroutine = null;
    }
}

