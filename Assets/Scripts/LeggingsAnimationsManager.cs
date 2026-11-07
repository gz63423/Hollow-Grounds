using UnityEngine;

public class LeggingsAnimationsManager : MonoBehaviour
{
    [Header("Leggings")]
    public GameObject leggingsObject;
    public Animator leggingsAnimator;

    [Header("Leggings Rarity Overrides")]
    public AnimatorOverrideController[] leggingsOverrides =
        new AnimatorOverrideController[5];

    private int currentLeggingsTier;

    // ============================================================
    // AWAKE
    // ============================================================

    void Awake()
    {
        currentLeggingsTier =
            PlayerPrefs.GetInt("LeggingsTier", 0);

        ApplyLeggings();
    }

    // ============================================================
    // LATE UPDATE
    // ============================================================

    private void LateUpdate()
    {
        if (Player.Instance == null ||
            Player.Instance.animator == null ||
            leggingsAnimator == null ||
            !leggingsAnimator.isActiveAndEnabled)
        {
            return;
        }

        Animator playerAnimator = Player.Instance.animator;

        leggingsAnimator.speed = playerAnimator.speed;

        leggingsAnimator.SetFloat("InputX", playerAnimator.GetFloat("InputX"));
        leggingsAnimator.SetFloat("InputY", playerAnimator.GetFloat("InputY"));
        leggingsAnimator.SetFloat("LastInputX", playerAnimator.GetFloat("LastInputX"));
        leggingsAnimator.SetFloat("LastInputY", playerAnimator.GetFloat("LastInputY"));

        leggingsAnimator.SetBool("isWalking", playerAnimator.GetBool("isWalking"));
        leggingsAnimator.SetBool("isRunning", playerAnimator.GetBool("isRunning"));
    }

    // ============================================================
    // APPLY LEGGINGS
    // ============================================================

    void ApplyLeggings()
    {
        if (leggingsAnimator == null)
            return;

        // TIER 0 = NO LEGGINGS

        if (currentLeggingsTier == 0)
        {
            if (leggingsObject != null)
                leggingsObject.SetActive(false);

            return;
        }

        if (leggingsObject != null)
            leggingsObject.SetActive(true);

        int index = Mathf.Clamp(
            currentLeggingsTier - 1,
            0,
            leggingsOverrides.Length - 1
        );

        if (leggingsOverrides[index] != null)
        {
            leggingsAnimator.runtimeAnimatorController =
                leggingsOverrides[index];
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

        if (!leggingsAnimator.isActiveAndEnabled)
            return;

        leggingsAnimator.speed = playerAnimator.speed;

        leggingsAnimator.SetFloat(
            "InputX",
            playerAnimator.GetFloat("InputX")
        );

        leggingsAnimator.SetFloat(
            "InputY",
            playerAnimator.GetFloat("InputY")
        );

        leggingsAnimator.SetFloat(
            "LastInputX",
            playerAnimator.GetFloat("LastInputX")
        );

        leggingsAnimator.SetFloat(
            "LastInputY",
            playerAnimator.GetFloat("LastInputY")
        );

        leggingsAnimator.SetBool(
            "isWalking",
            playerAnimator.GetBool("isWalking")
        );

        leggingsAnimator.SetBool(
            "isRunning",
            playerAnimator.GetBool("isRunning")
        );

        // Synchronize animation state and timing.
        AnimatorStateInfo playerState =
            playerAnimator.GetCurrentAnimatorStateInfo(0);

        AnimatorStateInfo leggingsState =
            leggingsAnimator.GetCurrentAnimatorStateInfo(0);

        if (playerState.fullPathHash != leggingsState.fullPathHash)
        {
            leggingsAnimator.Play(
                playerState.fullPathHash,
                0,
                playerState.normalizedTime
            );

            leggingsAnimator.Update(0f);
        }
        else
        {
            float timeDifference = Mathf.Abs(
                Mathf.Repeat(playerState.normalizedTime, 1f) -
                Mathf.Repeat(leggingsState.normalizedTime, 1f)
            );

            if (timeDifference > 0.5f)
                timeDifference = 1f - timeDifference;

            if (timeDifference > 0.02f)
            {
                leggingsAnimator.Play(
                    playerState.fullPathHash,
                    0,
                    playerState.normalizedTime
                );

                leggingsAnimator.Update(0f);
            }
        }
    }

    // ============================================================
    // TRIGGER ATTACK
    // ============================================================

    public void TriggerAttack()
    {
        if (leggingsAnimator == null)
            return;

        leggingsAnimator.ResetTrigger("Attack");
        leggingsAnimator.SetTrigger("Attack");
    }

    // ============================================================
    // TRIGGER HURT
    // ============================================================

    public void TriggerHurt()
    {
        if (leggingsAnimator == null)
            return;

        leggingsAnimator.ResetTrigger("Hurt");
        leggingsAnimator.SetTrigger("Hurt");
    }

    // ============================================================
    // TRIGGER DIE
    // ============================================================

    public void TriggerDie()
    {
        if (leggingsAnimator == null)
            return;

        leggingsAnimator.ResetTrigger("Die");
        leggingsAnimator.SetTrigger("Die");
    }

    // ============================================================
    // RESPAWN RESET
    // ============================================================

    public void ResetAnimator()
    {
        if (leggingsAnimator == null)
            return;

        currentLeggingsTier =
            PlayerPrefs.GetInt("LeggingsTier", 0);

        ApplyLeggings();

        if (currentLeggingsTier == 0)
            return;

        leggingsAnimator.Rebind();
        leggingsAnimator.Update(0f);

        leggingsAnimator.Play("Idle", 0, 0f);

        leggingsAnimator.speed = 1f;
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
