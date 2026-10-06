using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

[DefaultExecutionOrder(-1000)]
public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }


    private const string SaveFileName = "save.json";

    // IMPORTANT:
    // Change this value before releasing the game.
    // It is used to encrypt and authenticate the save.
    //
    // This is NOT a perfect anti-cheat system because the key
    // exists inside the game. It is designed to prevent normal
    // players from simply opening/editing save.json.
    private const string SaveSecret =
        "HG_2026_CHANGE_THIS_SECRET_BEFORE_RELEASE_9F4A7C2D";

    private string SavePath =>
        Path.Combine(Application.persistentDataPath, SaveFileName);

    [Serializable]
    private class SaveData
    {
        // Player progression
        public float PlayerMoney;

        public int HelmetTier;
        public float HelmetHPBonus;

        public int ChestplateTier;
        public float ChestplateHPBonus;

        public int LeggingsTier;
        public float LeggingsHPBonus;

        public int BootsTier;
        public float BootsHPBonus;

        public int HouseLevel;

        public int RebirthCount;

        // NEW:
        // Saves whether the next run must start at Wave 1.
        public int StartAtWaveOne;

        // Account progression
        public int AccountLevel;
        public int AccountXP;

        // Statistics
        public float TimePlayed;
        public float NetWorth;
        public int HighestWave;
        public int EnemiesKilled;
        public int BossesKilled;
        public int DamageDealt;
        public int PlayerDeaths;

        // Encyclopedia
        public int EnemyUnlocked_Scout;
        public int EnemyUnlocked_Ravager;
        public int EnemyUnlocked_Stalker;
        public int EnemyUnlocked_Spitter;
        public int EnemyUnlocked_Boss;

        // Rebirth intro
        public int RebirthIntroCompleted;
    }

    [Serializable]
    private class EncryptedSave
    {
        public int version;
        public string iv;
        public string data;
        public string signature;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadOrCreateSave();
    }

    private void Start()
    {
        InvokeRepeating(nameof(SaveNow), 5f, 5f);
    }

    // =========================================================
    // PUBLIC SAVE
    // =========================================================

    public void SaveNow()
    {
        try
        {
            PlayerPrefs.Save();

            SaveData data = CreateSaveData();

            string json = JsonUtility.ToJson(data);

            string encrypted = EncryptAndSign(json);

            File.WriteAllText(SavePath, encrypted);

            Debug.Log("SAVE MANAGER: Save written successfully.");
        }
        catch (Exception e)
        {
            Debug.LogError(
                "SAVE MANAGER: Failed to save!\n" +
                e
            );
        }
    }

    public void ForceSave()
    {
        SaveNow();
    }

    // =========================================================
    // LOAD / MIGRATION
    // =========================================================

    private void LoadOrCreateSave()
    {
        Debug.Log("SAVE MANAGER: Save path = " + SavePath);

        try
        {
            if (File.Exists(SavePath))
            {
                string encryptedFile = File.ReadAllText(SavePath);

                if (string.IsNullOrWhiteSpace(encryptedFile))
                {
                    Debug.LogWarning(
                        "SAVE MANAGER: save.json is empty. " +
                        "Using existing PlayerPrefs."
                    );

                    SaveNow();
                    return;
                }

                if (TryDecryptAndVerify(encryptedFile, out string json))
                {
                    LoadFromJson(json);

                    Debug.Log(
                        "SAVE MANAGER: Encrypted save loaded successfully."
                    );
                }
                else
                {
                    Debug.LogError(
                        "SAVE MANAGER: SAVE INTEGRITY CHECK FAILED!\n" +
                        "The save may have been modified or corrupted.\n" +
                        "The existing PlayerPrefs will be preserved."
                    );

                    // Do NOT automatically overwrite the save here.
                    // This prevents a suspicious cloud/local save from
                    // immediately destroying the player's existing data.
                }
            }
            else
            {
                Debug.Log(
                    "SAVE MANAGER: No save.json found.\n" +
                    "Creating encrypted save from existing PlayerPrefs."
                );

                // First-time migration:
                // Existing PlayerPrefs become the initial JSON save.
                SaveNow();
            }
        }
        catch (Exception e)
        {
            Debug.LogError(
                "SAVE MANAGER: Failed to load save!\n" +
                e
            );
        }
    }

    private void LoadFromJson(string json)
    {
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        if (data == null)
        {
            throw new Exception("Save JSON could not be parsed.");
        }

        // Player progression
        PlayerPrefs.SetFloat("PlayerMoney", data.PlayerMoney);

        PlayerPrefs.SetInt("HelmetTier", data.HelmetTier);
        PlayerPrefs.SetFloat("HelmetHPBonus", data.HelmetHPBonus);

        PlayerPrefs.SetInt("ChestplateTier", data.ChestplateTier);
        PlayerPrefs.SetFloat("ChestplateHPBonus", data.ChestplateHPBonus);

        PlayerPrefs.SetInt("LeggingsTier", data.LeggingsTier);
        PlayerPrefs.SetFloat("LeggingsHPBonus", data.LeggingsHPBonus);

        PlayerPrefs.SetInt("BootsTier", data.BootsTier);
        PlayerPrefs.SetFloat("BootsHPBonus", data.BootsHPBonus);

        PlayerPrefs.SetInt("HouseLevel", data.HouseLevel);
        PlayerPrefs.SetInt("RebirthCount", data.RebirthCount);

        // NEW:
        // Load the saved Wave 1 reset state.
        PlayerPrefs.SetInt(
            "StartAtWaveOne",
            data.StartAtWaveOne
        );

        // Account progression
        PlayerPrefs.SetInt("AccountLevel", data.AccountLevel);
        PlayerPrefs.SetInt("AccountXP", data.AccountXP);

        // Statistics
        PlayerPrefs.SetFloat("TimePlayed", data.TimePlayed);
        PlayerPrefs.SetFloat("NetWorth", data.NetWorth);
        PlayerPrefs.SetInt("HighestWave", data.HighestWave);
        PlayerPrefs.SetInt("EnemiesKilled", data.EnemiesKilled);
        PlayerPrefs.SetInt("BossesKilled", data.BossesKilled);
        PlayerPrefs.SetInt("DamageDealt", data.DamageDealt);
        PlayerPrefs.SetInt("PlayerDeaths", data.PlayerDeaths);

        // Encyclopedia
        PlayerPrefs.SetInt(
            "EnemyUnlocked_Scout",
            data.EnemyUnlocked_Scout
        );

        PlayerPrefs.SetInt(
            "EnemyUnlocked_Ravager",
            data.EnemyUnlocked_Ravager
        );

        PlayerPrefs.SetInt(
            "EnemyUnlocked_Stalker",
            data.EnemyUnlocked_Stalker
        );

        PlayerPrefs.SetInt(
            "EnemyUnlocked_Spitter",
            data.EnemyUnlocked_Spitter
        );

        PlayerPrefs.SetInt(
            "EnemyUnlocked_Boss",
            data.EnemyUnlocked_Boss
        );

        // Rebirth intro
        PlayerPrefs.SetInt(
            "RebirthIntroCompleted",
            data.RebirthIntroCompleted
        );

        PlayerPrefs.Save();
    }

    // =========================================================
    // CREATE SAVE DATA
    // =========================================================

    private SaveData CreateSaveData()
    {
        SaveData data = new SaveData();

        // Player progression
        data.PlayerMoney =
            PlayerPrefs.GetFloat("PlayerMoney", 0f);

        data.HelmetTier =
            PlayerPrefs.GetInt("HelmetTier", 0);

        data.HelmetHPBonus =
            PlayerPrefs.GetFloat("HelmetHPBonus", 0f);

        data.ChestplateTier =
            PlayerPrefs.GetInt("ChestplateTier", 0);

        data.ChestplateHPBonus =
            PlayerPrefs.GetFloat("ChestplateHPBonus", 0f);

        data.LeggingsTier =
            PlayerPrefs.GetInt("LeggingsTier", 0);

        data.LeggingsHPBonus =
            PlayerPrefs.GetFloat("LeggingsHPBonus", 0f);

        data.BootsTier =
            PlayerPrefs.GetInt("BootsTier", 0);

        data.BootsHPBonus =
            PlayerPrefs.GetFloat("BootsHPBonus", 0f);

        data.HouseLevel =
            PlayerPrefs.GetInt("HouseLevel", 0);

        data.RebirthCount =
            PlayerPrefs.GetInt("RebirthCount", 0);

        // NEW:
        // Save the Wave 1 reset state.
        data.StartAtWaveOne =
            PlayerPrefs.GetInt(
                "StartAtWaveOne",
                0
            );

        // Account progression
        data.AccountLevel =
            PlayerPrefs.GetInt("AccountLevel", 1);

        data.AccountXP =
            PlayerPrefs.GetInt("AccountXP", 0);

        // Statistics
        data.TimePlayed =
            PlayerPrefs.GetFloat("TimePlayed", 0f);

        data.NetWorth =
            PlayerPrefs.GetFloat("NetWorth", 0f);

        data.HighestWave =
            PlayerPrefs.GetInt("HighestWave", 0);

        data.EnemiesKilled =
            PlayerPrefs.GetInt("EnemiesKilled", 0);

        data.BossesKilled =
            PlayerPrefs.GetInt("BossesKilled", 0);

        data.DamageDealt =
            PlayerPrefs.GetInt("DamageDealt", 0);

        data.PlayerDeaths =
            PlayerPrefs.GetInt("PlayerDeaths", 0);

        // Encyclopedia
        data.EnemyUnlocked_Scout =
            PlayerPrefs.GetInt(
                "EnemyUnlocked_Scout",
                0
            );

        data.EnemyUnlocked_Ravager =
            PlayerPrefs.GetInt(
                "EnemyUnlocked_Ravager",
                0
            );

        data.EnemyUnlocked_Stalker =
            PlayerPrefs.GetInt(
                "EnemyUnlocked_Stalker",
                0
            );

        data.EnemyUnlocked_Spitter =
            PlayerPrefs.GetInt(
                "EnemyUnlocked_Spitter",
                0
            );

        data.EnemyUnlocked_Boss =
            PlayerPrefs.GetInt(
                "EnemyUnlocked_Boss",
                0
            );

        // Rebirth intro
        data.RebirthIntroCompleted =
            PlayerPrefs.GetInt(
                "RebirthIntroCompleted",
                0
            );

        return data;
    }

    // =========================================================
    // ENCRYPTION + INTEGRITY
    // =========================================================

    private string EncryptAndSign(string plainText)
    {
        byte[] plainBytes =
            Encoding.UTF8.GetBytes(plainText);

        byte[] key =
            DeriveKey(SaveSecret);

        byte[] iv = new byte[16];

        using (RandomNumberGenerator rng =
               RandomNumberGenerator.Create())
        {
            rng.GetBytes(iv);
        }

        byte[] encryptedBytes;

        using (Aes aes = Aes.Create())
        {
            aes.Key = key;
            aes.IV = iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            using (ICryptoTransform encryptor =
                   aes.CreateEncryptor())
            {
                encryptedBytes =
                    encryptor.TransformFinalBlock(
                        plainBytes,
                        0,
                        plainBytes.Length
                    );
            }
        }

        string ivBase64 =
            Convert.ToBase64String(iv);

        string dataBase64 =
            Convert.ToBase64String(encryptedBytes);

        string signature =
            CreateSignature(ivBase64, dataBase64);

        EncryptedSave save =
            new EncryptedSave
            {
                version = 1,
                iv = ivBase64,
                data = dataBase64,
                signature = signature
            };

        return JsonUtility.ToJson(
            save,
            true
        );
    }

    private bool TryDecryptAndVerify(
        string encryptedJson,
        out string plainText)
    {
        plainText = null;

        try
        {
            EncryptedSave save =
                JsonUtility.FromJson<EncryptedSave>(
                    encryptedJson
                );

            if (save == null)
            {
                Debug.LogError(
                    "SAVE MANAGER: Invalid encrypted save format."
                );

                return false;
            }

            if (save.version != 1)
            {
                Debug.LogError(
                    "SAVE MANAGER: Unsupported save version: " +
                    save.version
                );

                return false;
            }

            string expectedSignature =
                CreateSignature(
                    save.iv,
                    save.data
                );

            if (!SecureEquals(
                save.signature,
                expectedSignature))
            {
                Debug.LogError(
                    "SAVE MANAGER: Save signature is invalid."
                );

                return false;
            }

            byte[] iv =
                Convert.FromBase64String(save.iv);

            byte[] encryptedBytes =
                Convert.FromBase64String(save.data);

            byte[] key =
                DeriveKey(SaveSecret);

            byte[] decryptedBytes;

            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using (ICryptoTransform decryptor =
                       aes.CreateDecryptor())
                {
                    decryptedBytes =
                        decryptor.TransformFinalBlock(
                            encryptedBytes,
                            0,
                            encryptedBytes.Length
                        );
                }
            }

            plainText =
                Encoding.UTF8.GetString(
                    decryptedBytes
                );

            return true;
        }
        catch (Exception e)
        {
            Debug.LogError(
                "SAVE MANAGER: Failed to decrypt save.\n" +
                e.Message
            );

            return false;
        }
    }

    private string CreateSignature(
        string ivBase64,
        string dataBase64)
    {
        byte[] key =
            DeriveKey(SaveSecret);

        string content =
            ivBase64 + "." + dataBase64;

        byte[] contentBytes =
            Encoding.UTF8.GetBytes(content);

        using (HMACSHA256 hmac =
               new HMACSHA256(key))
        {
            byte[] hash =
                hmac.ComputeHash(contentBytes);

            return Convert.ToBase64String(hash);
        }
    }

    private byte[] DeriveKey(string secret)
    {
        using (SHA256 sha =
               SHA256.Create())
        {
            return sha.ComputeHash(
                Encoding.UTF8.GetBytes(secret)
            );
        }
    }

    private bool SecureEquals(
        string a,
        string b)
    {
        if (a == null || b == null)
            return false;

        byte[] aBytes =
            Encoding.UTF8.GetBytes(a);

        byte[] bBytes =
            Encoding.UTF8.GetBytes(b);

        if (aBytes.Length != bBytes.Length)
            return false;

        int result = 0;

        for (int i = 0; i < aBytes.Length; i++)
        {
            result |= aBytes[i] ^ bBytes[i];
        }

        return result == 0;
    }

    // =========================================================
    // APPLICATION LIFECYCLE
    // =========================================================

    private void OnApplicationPause(
        bool pauseStatus)
    {
        if (pauseStatus)
        {
            SaveNow();
        }
    }

    private void OnApplicationQuit()
    {
        SaveNow();
    }
}