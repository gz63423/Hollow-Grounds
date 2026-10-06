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
            PlayerPrefs.GetInt(
                "BootsTier",
                0
            );


        ApplyBoots();
    }


    // ============================================================
    // UPDATE
    // ============================================================

    void Update()
    {
        if (bootsAnimator == null)
            return;


        SyncParameters();
    }


    // ============================================================
    // APPLY BOOTS
    // ============================================================

    void ApplyBoots()
    {
        if (bootsAnimator == null)
            return;


        // ========================================================
        // TIER 0 = NO BOOTS
        // ========================================================

        if (currentBootsTier == 0)
        {
            if (bootsObject != null)
                bootsObject.SetActive(false);


            return;
        }


        // ========================================================
        // TIER 1-5
        // ========================================================

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
        bootsAnimator.SetFloat(
            "InputX",
            playerAnimator.GetFloat("InputX")
        );


        bootsAnimator.SetFloat(
            "InputY",
            playerAnimator.GetFloat("InputY")
        );


        // Last facing direction
        bootsAnimator.SetFloat(
            "LastInputX",
            playerAnimator.GetFloat("LastInputX")
        );


        bootsAnimator.SetFloat(
            "LastInputY",
            playerAnimator.GetFloat("LastInputY")
        );


        // Movement states
        bootsAnimator.SetBool(
            "isWalking",
            playerAnimator.GetBool("isWalking")
        );


        bootsAnimator.SetBool(
            "isRunning",
            playerAnimator.GetBool("isRunning")
        );


        // Keep playback speed identical
        bootsAnimator.speed =
            playerAnimator.speed;
    }


    // ============================================================
    // TRIGGER FUNCTIONS
    // ============================================================

    public void TriggerAttack()
    {
        if (bootsAnimator == null)
            return;


        bootsAnimator.ResetTrigger(
            "Attack"
        );


        bootsAnimator.SetTrigger(
            "Attack"
        );
    }


    public void TriggerHurt()
    {
        if (bootsAnimator == null)
            return;


        bootsAnimator.ResetTrigger(
            "Hurt"
        );


        bootsAnimator.SetTrigger(
            "Hurt"
        );
    }


    public void TriggerDie()
    {
        if (bootsAnimator == null)
            return;


        bootsAnimator.ResetTrigger(
            "Die"
        );


        bootsAnimator.SetTrigger(
            "Die"
        );
    }


    // ============================================================
    // RESPAWN RESET
    // ============================================================

    public void ResetAnimator()
    {
        if (bootsAnimator == null)
            return;


        currentBootsTier =
            PlayerPrefs.GetInt(
                "BootsTier",
                0
            );


        ApplyBoots();


        // Don't attempt to reset an inactive Tier 0 boots object
        if (currentBootsTier == 0)
            return;


        bootsAnimator.Rebind();


        bootsAnimator.Update(0f);


        bootsAnimator.Play(
            "Idle",
            0,
            0f
        );


        bootsAnimator.speed =
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