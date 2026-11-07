using UnityEngine;

public class ChestplateAnimationsManager : MonoBehaviour
{
    [Header("Chestplate")]
    public GameObject chestplateObject;
    public Animator chestplateAnimator;

    [Header("Chestplate Rarity Overrides")]
    public AnimatorOverrideController[] chestplateOverrides =
        new AnimatorOverrideController[5];

    private int currentChestplateTier;

    // ============================================================
    // AWAKE
    // ============================================================

    void Awake()
    {
        currentChestplateTier =
            PlayerPrefs.GetInt("ChestplateTier", 0);

        ApplyChestplate();
    }

    // ============================================================
    // LATE UPDATE
    // ============================================================

    private void LateUpdate()
    {
        SyncParameters();
    }

    // ============================================================
    // APPLY CHESTPLATE
    // ============================================================

    void ApplyChestplate()
    {
        if (chestplateAnimator == null)
            return;

        // TIER 0 = NO CHESTPLATE

        if (currentChestplateTier == 0)
        {
            if (chestplateObject != null)
                chestplateObject.SetActive(false);

            return;
        }

        if (chestplateObject != null)
            chestplateObject.SetActive(true);

        if (chestplateOverrides == null ||
            chestplateOverrides.Length == 0)
        {
            return;
        }

        int index = Mathf.Clamp(
            currentChestplateTier - 1,
            0,
            chestplateOverrides.Length - 1
        );

        if (chestplateOverrides[index] != null)
        {
            chestplateAnimator.runtimeAnimatorController =
                chestplateOverrides[index];
        }
    }

    // ============================================================
    // SYNCHRONIZE WITH PLAYER
    // ============================================================

    void SyncParameters()
    {
        Player player = Player.Instance;

        if (player == null ||
            player.animator == null ||
            chestplateAnimator == null ||
            !chestplateAnimator.isActiveAndEnabled)
        {
            return;
        }

        Animator playerAnimator = player.animator;

        chestplateAnimator.speed = playerAnimator.speed;

        chestplateAnimator.SetFloat(
            "InputX",
            playerAnimator.GetFloat("InputX")
        );

        chestplateAnimator.SetFloat(
            "InputY",
            playerAnimator.GetFloat("InputY")
        );

        chestplateAnimator.SetFloat(
            "LastInputX",
            playerAnimator.GetFloat("LastInputX")
        );

        chestplateAnimator.SetFloat(
            "LastInputY",
            playerAnimator.GetFloat("LastInputY")
        );

        chestplateAnimator.SetBool(
            "isWalking",
            playerAnimator.GetBool("isWalking")
        );

        chestplateAnimator.SetBool(
            "isRunning",
            playerAnimator.GetBool("isRunning")
        );

        // Synchronize the active animation state and timing.

        AnimatorStateInfo playerState =
            playerAnimator.GetCurrentAnimatorStateInfo(0);

        AnimatorStateInfo chestplateState =
            chestplateAnimator.GetCurrentAnimatorStateInfo(0);

        if (playerAnimator.IsInTransition(0) ||
            chestplateAnimator.IsInTransition(0))
        {
            return;
        }

        if (playerState.fullPathHash != chestplateState.fullPathHash)
        {
            chestplateAnimator.Play(
                playerState.fullPathHash,
                0,
                playerState.normalizedTime
            );

            chestplateAnimator.Update(0f);
        }
        else
        {
            float timeDifference = Mathf.Abs(
                Mathf.Repeat(playerState.normalizedTime, 1f) -
                Mathf.Repeat(chestplateState.normalizedTime, 1f)
            );

            if (timeDifference > 0.5f)
                timeDifference = 1f - timeDifference;

            if (timeDifference > 0.02f)
            {
                chestplateAnimator.Play(
                    playerState.fullPathHash,
                    0,
                    playerState.normalizedTime
                );

                chestplateAnimator.Update(0f);
            }
        }
    }

    // ============================================================
    // TRIGGER ATTACK
    // ============================================================

    public void TriggerAttack()
    {
        if (chestplateAnimator == null)
            return;

        chestplateAnimator.ResetTrigger("Attack");
        chestplateAnimator.SetTrigger("Attack");
    }

    // ============================================================
    // TRIGGER HURT
    // ============================================================

    public void TriggerHurt()
    {
        if (chestplateAnimator == null)
            return;

        chestplateAnimator.ResetTrigger("Hurt");
        chestplateAnimator.SetTrigger("Hurt");
    }

    // ============================================================
    // TRIGGER DIE
    // ============================================================

    public void TriggerDie()
    {
        if (chestplateAnimator == null)
            return;

        chestplateAnimator.ResetTrigger("Die");
        chestplateAnimator.SetTrigger("Die");
    }

    // ============================================================
    // RESPAWN RESET
    // ============================================================

    public void ResetAnimator()
    {
        if (chestplateAnimator == null)
            return;

        currentChestplateTier =
            PlayerPrefs.GetInt("ChestplateTier", 0);

        if (currentChestplateTier == 0)
        {
            if (chestplateObject != null)
                chestplateObject.SetActive(false);

            return;
        }

        if (chestplateObject != null)
            chestplateObject.SetActive(true);

        ApplyChestplate();

        chestplateAnimator.Rebind();
        chestplateAnimator.Update(0f);

        chestplateAnimator.Play("Idle", 0, 0f);

        chestplateAnimator.speed = 1f;
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
