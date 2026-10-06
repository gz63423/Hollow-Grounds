using UnityEngine;
using TMPro;

public class WaveText : MonoBehaviour
{
    public WaveSpawner waveSpawner;
    public TextMeshProUGUI waveText;


    void Start()
    {
        UpdateWaveText();
    }


    void Update()
    {
        UpdateWaveText();
    }


    void UpdateWaveText()
    {
        if (waveSpawner == null || waveText == null)
            return;


        // ==========================================
        // ENDLESS MODE
        // ==========================================

        if (waveSpawner.IsEndlessMode)
        {
            waveText.text =
                "Endless Mode";

            return;
        }


        // ==========================================
        // THE FINALE
        // ==========================================

        if (
            waveSpawner.betaVersion &&
            waveSpawner.CurrentWaveNumber ==
            waveSpawner.betaMaxWave
        )
        {
            waveText.text =
                "The Finale";

            return;
        }


        // ==========================================
        // NORMAL WAVES
        // ==========================================

        waveText.text =
            "Wave " +
            waveSpawner.CurrentWaveNumber;
    }
}

