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
            PlayerPrefs.GetInt(
                "LeggingsTier",
                0
            );


        ApplyLeggings();
    }


    // ============================================================
    // UPDATE
    // ============================================================

    void Update()
    {
        if (leggingsAnimator == null)
            return;


        SyncParameters();
    }


    // ============================================================
    // APPLY LEGGINGS
    // ============================================================

    void ApplyLeggings()
    {
        if (leggingsAnimator == null)
            return;


        // ========================================================
        // TIER 0 = NO LEGGINGS
        // ========================================================

        if (currentLeggingsTier == 0)
        {
            if (leggingsObject != null)
                leggingsObject.SetActive(false);


            return;
        }


        // ========================================================
        // TIER 1-5
        // ========================================================

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


        // Movement
        leggingsAnimator.SetFloat(
            "InputX",
            playerAnimator.GetFloat("InputX")
        );


        leggingsAnimator.SetFloat(
            "InputY",
            playerAnimator.GetFloat("InputY")
        );


        // Last facing direction
        leggingsAnimator.SetFloat(
            "LastInputX",
            playerAnimator.GetFloat("LastInputX")
        );


        leggingsAnimator.SetFloat(
            "LastInputY",
            playerAnimator.GetFloat("LastInputY")
        );


        // Movement states
        leggingsAnimator.SetBool(
            "isWalking",
            playerAnimator.GetBool("isWalking")
        );


        leggingsAnimator.SetBool(
            "isRunning",
            playerAnimator.GetBool("isRunning")
        );


        // Keep playback speed identical
        leggingsAnimator.speed =
            playerAnimator.speed;
    }


    // ============================================================
    // TRIGGER FUNCTIONS
    // ============================================================

    public void TriggerAttack()
    {
        if (leggingsAnimator == null)
            return;


        leggingsAnimator.ResetTrigger(
            "Attack"
        );


        leggingsAnimator.SetTrigger(
            "Attack"
        );
    }


    public void TriggerHurt()
    {
        if (leggingsAnimator == null)
            return;


        leggingsAnimator.ResetTrigger(
            "Hurt"
        );


        leggingsAnimator.SetTrigger(
            "Hurt"
        );
    }


    public void TriggerDie()
    {
        if (leggingsAnimator == null)
            return;


        leggingsAnimator.ResetTrigger(
            "Die"
        );


        leggingsAnimator.SetTrigger(
            "Die"
        );
    }


    // ============================================================
    // RESPAWN RESET
    // ============================================================

    public void ResetAnimator()
    {
        if (leggingsAnimator == null)
            return;


        currentLeggingsTier =
            PlayerPrefs.GetInt(
                "LeggingsTier",
                0
            );


        ApplyLeggings();


        // Don't attempt to reset an inactive Tier 0 leggings object
        if (currentLeggingsTier == 0)
            return;


        leggingsAnimator.Rebind();


        leggingsAnimator.Update(0f);


        leggingsAnimator.Play(
            "Idle",
            0,
            0f
        );


        leggingsAnimator.speed =
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