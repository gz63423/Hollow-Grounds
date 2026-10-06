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
        currentHelmetTier =
            PlayerPrefs.GetInt(
                "HelmetTier",
                0
            );

        ApplyHelmet();
    }


    // ============================================================
    // LATE UPDATE
    // ============================================================

    void LateUpdate()
    {
        if (helmetAnimator == null)
            return;

        SyncParameters();
    }


    // ============================================================
    // APPLY HELMET
    // ============================================================

    void ApplyHelmet()
    {
        if (helmetAnimator == null)
            return;


        // ========================================================
        // TIER 0 = HAIR
        // ========================================================

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


        // ========================================================
        // TIER 1-5
        // ========================================================

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
    // SYNC PLAYER PARAMETERS
    // ============================================================

    void SyncParameters()
    {
        Player player =
            Player.Instance;


        if (player == null)
            return;


        Animator playerAnimator =
            player.animator;


        if (playerAnimator == null)
            return;


        // ========================================================
        // MOVEMENT PARAMETERS
        // ========================================================

        helmetAnimator.SetFloat(
            "InputX",
            playerAnimator.GetFloat("InputX")
        );


        helmetAnimator.SetFloat(
            "InputY",
            playerAnimator.GetFloat("InputY")
        );


        // ========================================================
        // LAST FACING DIRECTION
        // ========================================================

        helmetAnimator.SetFloat(
            "LastInputX",
            playerAnimator.GetFloat("LastInputX")
        );


        helmetAnimator.SetFloat(
            "LastInputY",
            playerAnimator.GetFloat("LastInputY")
        );


        // ========================================================
        // MOVEMENT STATES
        // ========================================================

        helmetAnimator.SetBool(
            "isWalking",
            playerAnimator.GetBool("isWalking")
        );


        helmetAnimator.SetBool(
            "isRunning",
            playerAnimator.GetBool("isRunning")
        );


        // ========================================================
        // FORCE ANIMATION STATE SYNC
        // ========================================================

        AnimatorStateInfo playerState =
            playerAnimator.GetCurrentAnimatorStateInfo(0);


        AnimatorStateInfo helmetState =
            helmetAnimator.GetCurrentAnimatorStateInfo(0);


        if (playerState.fullPathHash !=
            helmetState.fullPathHash)
        {
            helmetAnimator.Play(
                playerState.fullPathHash,
                0,
                playerState.normalizedTime
            );


            helmetAnimator.Update(0f);
        }


        // ========================================================
        // KEEP ANIMATION SPEED IDENTICAL
        // ========================================================

        helmetAnimator.speed =
            playerAnimator.speed;
    }


    // ============================================================
    // TRIGGER ATTACK
    // ============================================================

    public void TriggerAttack()
    {
        if (helmetAnimator == null)
            return;


        helmetAnimator.ResetTrigger(
            "Attack"
        );


        helmetAnimator.SetTrigger(
            "Attack"
        );
    }


    // ============================================================
    // TRIGGER HURT
    // ============================================================

    public void TriggerHurt()
    {
        if (helmetAnimator == null)
            return;


        helmetAnimator.ResetTrigger(
            "Hurt"
        );


        helmetAnimator.SetTrigger(
            "Hurt"
        );
    }


    // ============================================================
    // TRIGGER DIE
    // ============================================================

    public void TriggerDie()
    {
        if (helmetAnimator == null)
            return;


        helmetAnimator.ResetTrigger(
            "Die"
        );


        helmetAnimator.SetTrigger(
            "Die"
        );
    }


    // ============================================================
    // RESPAWN RESET
    // ============================================================

    public void ResetAnimator()
    {
        if (helmetAnimator == null)
            return;


        currentHelmetTier =
            PlayerPrefs.GetInt(
                "HelmetTier",
                0
            );


        ApplyHelmet();


        helmetAnimator.Rebind();


        helmetAnimator.Update(0f);


        if (helmetObject != null &&
            helmetObject.activeInHierarchy)
        {
            helmetAnimator.Play(
                "Idle",
                0,
                0f
            );


            helmetAnimator.Update(0f);
        }


        helmetAnimator.speed =
            1f;
    }


    // ============================================================
    // ANIMATION EVENT FUNCTIONS
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