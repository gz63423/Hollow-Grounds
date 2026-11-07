using UnityEngine;

public class HelmetAnimationsManager : MonoBehaviour
{
    [Header("Helmet")]
    public GameObject helmetObject;
    public Animator helmetAnimator;

    [Header("Helmet Rarity Overrides")]
    public AnimatorOverrideController[] helmetOverrides =
        new AnimatorOverrideController[5];

    [Header("Default Hair Helmet")]
    public RuntimeAnimatorController defaultHairHelmetController;

    private int currentHelmetTier;

    // ============================================================
    // AWAKE
    // ============================================================

    void Awake()
    {
        currentHelmetTier = PlayerPrefs.GetInt("HelmetTier", 0);

        ApplyHelmet();
    }

    // ============================================================
    // LATE UPDATE
    // ============================================================

    private void LateUpdate()
    {
        if (Player.Instance == null ||
            Player.Instance.animator == null ||
            helmetAnimator == null ||
            !helmetAnimator.isActiveAndEnabled)
        {
            return;
        }

        Animator playerAnimator = Player.Instance.animator;

        // Match the player's animation playback speed.
        helmetAnimator.speed = playerAnimator.speed;

        // Match movement and facing parameters.
        helmetAnimator.SetFloat("InputX", playerAnimator.GetFloat("InputX"));
        helmetAnimator.SetFloat("InputY", playerAnimator.GetFloat("InputY"));
        helmetAnimator.SetFloat("LastInputX", playerAnimator.GetFloat("LastInputX"));
        helmetAnimator.SetFloat("LastInputY", playerAnimator.GetFloat("LastInputY"));

        helmetAnimator.SetBool("isWalking", playerAnimator.GetBool("isWalking"));
        helmetAnimator.SetBool("isRunning", playerAnimator.GetBool("isRunning"));
    }

    // ============================================================
    // APPLY HELMET
    // ============================================================

    void ApplyHelmet()
    {
        if (helmetAnimator == null)
            return;

        // TIER 0 = HAIR

        if (currentHelmetTier == 0)
        {
            if (helmetObject != null)
                helmetObject.SetActive(true);

            if (defaultHairHelmetController != null)
            {
                helmetAnimator.runtimeAnimatorController =
                    defaultHairHelmetController;
            }

            return;
        }

        // TIER 1-5

        if (helmetObject != null)
            helmetObject.SetActive(true);

        int index = Mathf.Clamp(
            currentHelmetTier - 1,
            0,
            helmetOverrides.Length - 1
        );

        if (helmetOverrides[index] != null)
        {
            helmetAnimator.runtimeAnimatorController =
                helmetOverrides[index];
        }
    }

    // ============================================================
    // SYNCHRONIZE WITH PLAYER
    // ============================================================

    void SyncParameters()
    {
        Player player = Player.Instance;

        if (player == null || player.animator == null)
            return;

        Animator playerAnimator = player.animator;

        if (!helmetAnimator.isActiveAndEnabled)
            return;

        // Keep the armor animation playback speed identical.
        helmetAnimator.speed = playerAnimator.speed;

        // Synchronize animation parameters.
        helmetAnimator.SetFloat(
            "InputX",
            playerAnimator.GetFloat("InputX")
        );

        helmetAnimator.SetFloat(
            "InputY",
            playerAnimator.GetFloat("InputY")
        );

        helmetAnimator.SetFloat(
            "LastInputX",
            playerAnimator.GetFloat("LastInputX")
        );

        helmetAnimator.SetFloat(
            "LastInputY",
            playerAnimator.GetFloat("LastInputY")
        );

        helmetAnimator.SetBool(
            "isWalking",
            playerAnimator.GetBool("isWalking")
        );

        helmetAnimator.SetBool(
            "isRunning",
            playerAnimator.GetBool("isRunning")
        );

        // Keep the helmet on the same animation state and frame.
        AnimatorStateInfo playerState =
            playerAnimator.GetCurrentAnimatorStateInfo(0);

        AnimatorStateInfo helmetState =
            helmetAnimator.GetCurrentAnimatorStateInfo(0);

        if (playerState.fullPathHash != helmetState.fullPathHash)
        {
            helmetAnimator.Play(
                playerState.fullPathHash,
                0,
                playerState.normalizedTime
            );

            helmetAnimator.Update(0f);
        }
        else
        {
            // Correct timing differences without restarting the animation.
            float timeDifference = Mathf.Abs(
                Mathf.Repeat(playerState.normalizedTime, 1f) -
                Mathf.Repeat(helmetState.normalizedTime, 1f)
            );

            if (timeDifference > 0.5f)
                timeDifference = 1f - timeDifference;

            if (timeDifference > 0.02f)
            {
                helmetAnimator.Play(
                    playerState.fullPathHash,
                    0,
                    playerState.normalizedTime
                );

                helmetAnimator.Update(0f);
            }
        }
    }

    // ============================================================
    // TRIGGER ATTACK
    // ============================================================

    public void TriggerAttack()
    {
        if (helmetAnimator == null)
            return;

        helmetAnimator.ResetTrigger("Attack");
        helmetAnimator.SetTrigger("Attack");
    }

    // ============================================================
    // TRIGGER HURT
    // ============================================================

    public void TriggerHurt()
    {
        if (helmetAnimator == null)
            return;

        helmetAnimator.ResetTrigger("Hurt");
        helmetAnimator.SetTrigger("Hurt");
    }

    // ============================================================
    // TRIGGER DIE
    // ============================================================

    public void TriggerDie()
    {
        if (helmetAnimator == null)
            return;

        helmetAnimator.ResetTrigger("Die");
        helmetAnimator.SetTrigger("Die");
    }

    // ============================================================
    // RESPAWN RESET
    // ============================================================

    public void ResetAnimator()
    {
        if (helmetAnimator == null)
            return;

        currentHelmetTier = PlayerPrefs.GetInt("HelmetTier", 0);

        ApplyHelmet();

        helmetAnimator.Rebind();
        helmetAnimator.Update(0f);

        if (helmetObject != null &&
            helmetObject.activeInHierarchy)
        {
            helmetAnimator.Play("Idle", 0, 0f);
            helmetAnimator.Update(0f);
        }

        helmetAnimator.speed = 1f;
    }

    // ============================================================
    // ANIMATION EVENTS
    // ============================================================

    public void DealAttackDamage()
    {
        // Intentionally empty.
    }

    public void EndAttack()
    {
        // Intentionally empty.
    }
}
