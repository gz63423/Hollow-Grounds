using UnityEngine;

public class EnemyBloodEffect : MonoBehaviour
{
    // =========================================================
    // SPRITE
    // =========================================================

    [Header("Blood Sprite")]

    [Tooltip("Drag your black square blood sprite here.")]
    public Sprite bloodSprite;


    // =========================================================
    // HIT EFFECT
    // =========================================================

    [Header("Hit Effect")]

    public int hitParticleCount = 8;

    public float hitMinSize = 0.8f;
    public float hitMaxSize = 1.5f;

    public float hitMinSpeed = 3f;
    public float hitMaxSpeed = 6f;

    public float hitLifetime = 2f;

    [Range(0f, 360f)]
    public float hitSpreadAngle = 360f;

    public float hitDrag = 1.5f;


    // =========================================================
    // DEATH EFFECT
    // =========================================================

    [Header("Death Effect")]

    public int deathParticleCount = 25;

    public float deathMinSize = 0.9f;
    public float deathMaxSize = 2.2f;

    public float deathMinSpeed = 4f;
    public float deathMaxSpeed = 8f;

    public float deathLifetime = 2f;

    [Range(0f, 360f)]
    public float deathSpreadAngle = 360f;

    public float deathDrag = 1.25f;


    // =========================================================
    // VERTICAL ARC
    // =========================================================

    [Header("Vertical Arc")]

    [Tooltip("How far above the enemy the blood starts.")]
    public float spawnVerticalOffset = 0.5f;

    [Tooltip("How high the blood rises before coming back down.")]
    public float arcHeight = 2f;

    [Tooltip("How long the upward/downward arc lasts.")]
    public float arcDuration = 0.8f;

    [Tooltip("Makes the landing feel more natural.")]
    public AnimationCurve arcCurve =
        new AnimationCurve(
            new Keyframe(0f, 0f),
            new Keyframe(0.5f, 1f),
            new Keyframe(1f, 0f)
        );


    // =========================================================
    // GROUND SHAPE
    // =========================================================

    [Header("Ground Shape")]

    [Tooltip("Horizontal stretch after landing.")]
    public float finalHorizontalStretch = 1.8f;

    [Tooltip("Vertical squash after landing.")]
    public float finalVerticalSquash = 0.35f;

    [Tooltip("Stretch while the blood is flying.")]
    public float movementStretch = 1.25f;

    [Tooltip("Percentage of lifetime at which the blood starts flattening.")]
    [Range(0f, 1f)]
    public float groundShapeStart = 0.45f;


    // =========================================================
    // VISUAL
    // =========================================================

    [Header("Visual")]

    public Color bloodColor = Color.black;

    [Range(0f, 1f)]
    public float shrinkAmount = 0.35f;

    public float rotationSpeed = 180f;

    public int sortingOrderOffset = -1;


    // =========================================================
    // RANDOMIZATION
    // =========================================================

    [Header("Randomization")]

    public float spawnPositionRandomness = 0.15f;

    public bool randomInitialRotation = true;


    // =========================================================
    // INTERNAL
    // =========================================================

    private SpriteRenderer enemySpriteRenderer;


    // =========================================================
    // AWAKE
    // =========================================================

    private void Awake()
    {
        enemySpriteRenderer =
            GetComponent<SpriteRenderer>();
    }


    // =========================================================
    // HIT
    // =========================================================

    public void PlayHitEffect()
    {
        if (bloodSprite == null)
        {
            Debug.LogError(
                "EnemyBloodEffect: Blood Sprite is not assigned on " +
                gameObject.name
            );

            return;
        }

        SpawnBlood(
            hitParticleCount,
            hitMinSize,
            hitMaxSize,
            hitMinSpeed,
            hitMaxSpeed,
            hitLifetime,
            hitSpreadAngle,
            hitDrag
        );
    }


    // =========================================================
    // DEATH
    // =========================================================

    public void PlayDeathEffect()
    {
        if (bloodSprite == null)
        {
            Debug.LogError(
                "EnemyBloodEffect: Blood Sprite is not assigned on " +
                gameObject.name
            );

            return;
        }

        SpawnBlood(
            deathParticleCount,
            deathMinSize,
            deathMaxSize,
            deathMinSpeed,
            deathMaxSpeed,
            deathLifetime,
            deathSpreadAngle,
            deathDrag
        );
    }


    // =========================================================
    // SPAWN BLOOD
    // =========================================================

    private void SpawnBlood(
        int count,
        float minSize,
        float maxSize,
        float minSpeed,
        float maxSpeed,
        float lifetime,
        float spreadAngle,
        float drag)
    {
        for (int i = 0; i < count; i++)
        {
            // -------------------------------------------------
            // RANDOM GROUND POSITION
            // -------------------------------------------------

            Vector2 randomOffset =
                Random.insideUnitCircle *
                spawnPositionRandomness;


            // -------------------------------------------------
            // START POSITION
            // -------------------------------------------------

            Vector3 spawnPosition =
                transform.position +
                new Vector3(
                    randomOffset.x,
                    randomOffset.y +
                    spawnVerticalOffset,
                    0f
                );


            // -------------------------------------------------
            // CREATE BLOOD
            // -------------------------------------------------

            GameObject blood =
                new GameObject(
                    "Blood Square"
                );

            blood.transform.position =
                spawnPosition;


            // -------------------------------------------------
            // SPRITE RENDERER
            // -------------------------------------------------

            SpriteRenderer renderer =
                blood.AddComponent<SpriteRenderer>();

            renderer.sprite =
                bloodSprite;

            renderer.color =
                bloodColor;

            if (enemySpriteRenderer != null)
            {
                renderer.sortingLayerID =
                    enemySpriteRenderer.sortingLayerID;

                renderer.sortingOrder =
                    enemySpriteRenderer.sortingOrder +
                    sortingOrderOffset;
            }


            // -------------------------------------------------
            // SIZE
            // -------------------------------------------------

            float size =
                Random.Range(
                    minSize,
                    maxSize
                );

            blood.transform.localScale =
                Vector3.one * size;


            // -------------------------------------------------
            // RANDOM ROTATION
            // -------------------------------------------------

            if (randomInitialRotation)
            {
                blood.transform.rotation =
                    Quaternion.Euler(
                        0f,
                        0f,
                        Random.Range(
                            0f,
                            360f
                        )
                    );
            }


            // -------------------------------------------------
            // RANDOM OUTWARD DIRECTION
            // -------------------------------------------------

            float angle =
                Random.Range(
                    0f,
                    spreadAngle
                );

            Vector2 direction =
                new Vector2(
                    Mathf.Cos(
                        angle *
                        Mathf.Deg2Rad
                    ),
                    Mathf.Sin(
                        angle *
                        Mathf.Deg2Rad
                    )
                ).normalized;


            // -------------------------------------------------
            // RANDOM SPEED
            // -------------------------------------------------

            float speed =
                Random.Range(
                    minSpeed,
                    maxSpeed
                );


            // -------------------------------------------------
            // PARTICLE
            // -------------------------------------------------

            BloodSquareParticle particle =
                blood.AddComponent<
                    BloodSquareParticle>();

            particle.Initialize(
                direction,
                speed,
                lifetime,
                drag,
                shrinkAmount,
                rotationSpeed,
                finalHorizontalStretch,
                finalVerticalSquash,
                movementStretch,
                groundShapeStart,
                spawnVerticalOffset,
                arcHeight,
                arcDuration,
                arcCurve
            );
        }
    }
}


// =============================================================
// BLOOD SQUARE PARTICLE
// =============================================================

public class BloodSquareParticle : MonoBehaviour
{
    private Vector2 direction;

    private float speed;

    private float lifetime;

    private float drag;

    private float shrinkAmount;

    private float rotationSpeed;

    private float horizontalStretch;

    private float verticalSquash;

    private float movementStretch;

    private float groundShapeStart;

    private float spawnVerticalOffset;

    private float arcHeight;

    private float arcDuration;

    private AnimationCurve arcCurve;

    private float elapsed;

    private Vector3 startingScale;

    private SpriteRenderer spriteRenderer;

    private Vector3 groundPosition;


    // =========================================================
    // INITIALIZE
    // =========================================================

    public void Initialize(
        Vector2 movementDirection,
        float movementSpeed,
        float particleLifetime,
        float particleDrag,
        float particleShrinkAmount,
        float particleRotationSpeed,
        float finalHorizontalStretchValue,
        float finalVerticalSquashValue,
        float movementStretchValue,
        float groundShapeStartValue,
        float verticalOffset,
        float verticalArcHeight,
        float verticalArcDuration,
        AnimationCurve verticalArcCurve)
    {
        direction =
            movementDirection.normalized;

        speed =
            movementSpeed;

        lifetime =
            particleLifetime;

        drag =
            particleDrag;

        shrinkAmount =
            particleShrinkAmount;

        rotationSpeed =
            Random.Range(
                -particleRotationSpeed,
                particleRotationSpeed
            );

        horizontalStretch =
            finalHorizontalStretchValue;

        verticalSquash =
            finalVerticalSquashValue;

        movementStretch =
            movementStretchValue;

        groundShapeStart =
            groundShapeStartValue;

        spawnVerticalOffset =
            verticalOffset;

        arcHeight =
            verticalArcHeight;

        arcDuration =
            verticalArcDuration;

        arcCurve =
            verticalArcCurve;

        spriteRenderer =
            GetComponent<SpriteRenderer>();

        startingScale =
            transform.localScale;

        // The ground position is the position the blood
        // eventually lands at.
        groundPosition =
            transform.position -
            new Vector3(
                0f,
                spawnVerticalOffset,
                0f
            );
    }


    // =========================================================
    // UPDATE
    // =========================================================

    private void Update()
    {
        elapsed +=
            Time.deltaTime;


        // =====================================================
        // NORMALIZED TIME
        // =====================================================

        float normalizedTime =
            Mathf.Clamp01(
                elapsed /
                lifetime
            );


        // =====================================================
        // OUTWARD GROUND MOVEMENT
        // =====================================================

        groundPosition +=
            (Vector3)(
                direction *
                speed *
                Time.deltaTime
            );


        // =====================================================
        // FRICTION
        // =====================================================

        speed =
            Mathf.MoveTowards(
                speed,
                0f,
                drag *
                Time.deltaTime
            );


        // =====================================================
        // VERTICAL ARC
        // =====================================================

        float arcProgress =
            Mathf.Clamp01(
                elapsed /
                Mathf.Max(
                    0.01f,
                    arcDuration
                )
            );

        float arcValue =
            arcCurve.Evaluate(
                arcProgress
            );

        float verticalPosition =
            arcValue *
            arcHeight;


        // =====================================================
        // FINAL POSITION
        // =====================================================

        transform.position =
            groundPosition +
            new Vector3(
                0f,
                verticalPosition,
                0f
            );


        // =====================================================
        // ROTATION
        // =====================================================

        transform.Rotate(
            0f,
            0f,
            rotationSpeed *
            Time.deltaTime
        );


        // =====================================================
        // SHAPE
        // =====================================================

        Vector3 shapeScale =
            startingScale;


        if (normalizedTime < groundShapeStart)
        {
            // -------------------------------------------------
            // FLYING
            // -------------------------------------------------

            float movementProgress =
                Mathf.InverseLerp(
                    0f,
                    groundShapeStart,
                    normalizedTime
                );

            float currentStretch =
                Mathf.Lerp(
                    1f,
                    movementStretch,
                    movementProgress
                );

            shapeScale.x *=
                currentStretch;

            shapeScale.y *=
                Mathf.Lerp(
                    1f,
                    0.9f,
                    movementProgress
                );
        }
        else
        {
            // -------------------------------------------------
            // LANDING
            // -------------------------------------------------

            float groundProgress =
                Mathf.InverseLerp(
                    groundShapeStart,
                    1f,
                    normalizedTime
                );

            groundProgress =
                Mathf.SmoothStep(
                    0f,
                    1f,
                    groundProgress
                );

            float currentHorizontal =
                Mathf.Lerp(
                    movementStretch,
                    horizontalStretch,
                    groundProgress
                );

            float currentVertical =
                Mathf.Lerp(
                    0.9f,
                    verticalSquash,
                    groundProgress
                );

            shapeScale.x *=
                currentHorizontal;

            shapeScale.y *=
                currentVertical;
        }


        // =====================================================
        // OVERALL SHRINK
        // =====================================================

        float shrinkProgress =
            Mathf.Lerp(
                1f,
                1f - shrinkAmount,
                normalizedTime
            );

        shapeScale *=
            shrinkProgress;


        transform.localScale =
            shapeScale;


        // =====================================================
        // FADE
        // =====================================================

        if (spriteRenderer != null)
        {
            Color color =
                spriteRenderer.color;

            float fadeStart =
                0.65f;

            if (normalizedTime > fadeStart)
            {
                float fadeProgress =
                    Mathf.InverseLerp(
                        fadeStart,
                        1f,
                        normalizedTime
                    );

                color.a =
                    Mathf.Lerp(
                        1f,
                        0f,
                        fadeProgress
                    );

                spriteRenderer.color =
                    color;
            }
        }


        // =====================================================
        // DESTROY
        // =====================================================

        if (elapsed >= lifetime)
        {
            Destroy(gameObject);
        }
    }
}
