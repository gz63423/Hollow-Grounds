using UnityEngine;

public class BootsAnimationsManager : MonoBehaviour
{
    [Header("Boots")]
    public GameObject bootsObject;
    public Animator bootsAnimator;

    [Header("Boots Rarity Overrides")]
    public AnimatorOverrideController[] bootsOverrides =
        new AnimatorOverrideController[5];

    private int currentBootsTier;

    // ============================================================
    // AWAKE
    // ============================================================

    void Awake()
    {
        currentBootsTier =
            PlayerPrefs.GetInt("BootsTier", 0);

        ApplyBoots();
    }

    // ============================================================
    // LATE UPDATE
    // ============================================================

    private void LateUpdate()
    {
        if (Player.Instance == null ||
            Player.Instance.animator == null ||
            bootsAnimator == null ||
            !bootsAnimator.isActiveAndEnabled)
        {
            return;
        }

        Animator playerAnimator = Player.Instance.animator;

        bootsAnimator.speed = playerAnimator.speed;

        bootsAnimator.SetFloat("InputX", playerAnimator.GetFloat("InputX"));
        bootsAnimator.SetFloat("InputY", playerAnimator.GetFloat("InputY"));
        bootsAnimator.SetFloat("LastInputX", playerAnimator.GetFloat("LastInputX"));
        bootsAnimator.SetFloat("LastInputY", playerAnimator.GetFloat("LastInputY"));

        bootsAnimator.SetBool("isWalking", playerAnimator.GetBool("isWalking"));
        bootsAnimator.SetBool("isRunning", playerAnimator.GetBool("isRunning"));
    }

    // ============================================================
    // APPLY BOOTS
    // ============================================================

    void ApplyBoots()
    {
        if (bootsAnimator == null)
            return;

        // TIER 0 = NO BOOTS

        if (currentBootsTier == 0)
        {
            if (bootsObject != null)
                bootsObject.SetActive(false);

            return;
        }

        if (bootsObject != null)
            bootsObject.SetActive(true);

        int index = Mathf.Clamp(
            currentBootsTier - 1,
            0,
            bootsOverrides.Length - 1
        );

        if (bootsOverrides[index] != null)
        {
            bootsAnimator.runtimeAnimatorController =
                bootsOverrides[index];
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

        if (!bootsAnimator.isActiveAndEnabled)
            return;

        bootsAnimator.speed = playerAnimator.speed;

        bootsAnimator.SetFloat(
            "InputX",
            playerAnimator.GetFloat("InputX")
        );

        bootsAnimator.SetFloat(
            "InputY",
            playerAnimator.GetFloat("InputY")
        );

        bootsAnimator.SetFloat(
            "LastInputX",
            playerAnimator.GetFloat("LastInputX")
        );

        bootsAnimator.SetFloat(
            "LastInputY",
            playerAnimator.GetFloat("LastInputY")
        );

        bootsAnimator.SetBool(
            "isWalking",
            playerAnimator.GetBool("isWalking")
        );

        bootsAnimator.SetBool(
            "isRunning",
            playerAnimator.GetBool("isRunning")
        );

        // Synchronize animation state and timing.
        AnimatorStateInfo playerState =
            playerAnimator.GetCurrentAnimatorStateInfo(0);

        AnimatorStateInfo bootsState =
            bootsAnimator.GetCurrentAnimatorStateInfo(0);

        if (playerState.fullPathHash != bootsState.fullPathHash)
        {
            bootsAnimator.Play(
                playerState.fullPathHash,
                0,
                playerState.normalizedTime
            );

            bootsAnimator.Update(0f);
        }
        else
        {
            float timeDifference = Mathf.Abs(
                Mathf.Repeat(playerState.normalizedTime, 1f) -
                Mathf.Repeat(bootsState.normalizedTime, 1f)
            );

            if (timeDifference > 0.5f)
                timeDifference = 1f - timeDifference;

            if (timeDifference > 0.02f)
            {
                bootsAnimator.Play(
                    playerState.fullPathHash,
                    0,
                    playerState.normalizedTime
                );

                bootsAnimator.Update(0f);
            }
        }
    }

    // ============================================================
    // TRIGGER ATTACK
    // ============================================================

    public void TriggerAttack()
    {
        if (bootsAnimator == null)
            return;

        bootsAnimator.ResetTrigger("Attack");
        bootsAnimator.SetTrigger("Attack");
    }

    // ============================================================
    // TRIGGER HURT
    // ============================================================

    public void TriggerHurt()
    {
        if (bootsAnimator == null)
            return;

        bootsAnimator.ResetTrigger("Hurt");
        bootsAnimator.SetTrigger("Hurt");
    }

    // ============================================================
    // TRIGGER DIE
    // ============================================================

    public void TriggerDie()
    {
        if (bootsAnimator == null)
            return;

        bootsAnimator.ResetTrigger("Die");
        bootsAnimator.SetTrigger("Die");
    }

    // ============================================================
    // RESPAWN RESET
    // ============================================================

    public void ResetAnimator()
    {
        if (bootsAnimator == null)
            return;

        currentBootsTier =
            PlayerPrefs.GetInt("BootsTier", 0);

        ApplyBoots();

        if (currentBootsTier == 0)
            return;

        bootsAnimator.Rebind();
        bootsAnimator.Update(0f);

        bootsAnimator.Play("Idle", 0, 0f);

        bootsAnimator.speed = 1f;
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
