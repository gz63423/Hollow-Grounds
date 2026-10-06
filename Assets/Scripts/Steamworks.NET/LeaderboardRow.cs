using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Steamworks;

public class LeaderboardRow : MonoBehaviour
{
    public TMP_Text rankText;
    public RawImage profilePicture;
    public TMP_Text playerNameText;
    public TMP_Text statText;


    private readonly Color playerGreen =
        new Color(
            0.0235f,
            1f,
            0f,
            1f
        );



    public void SetData(
        int rank,
        string playerName,
        Texture profileTexture,
        int statValue,
        bool isCurrency,
        CSteamID steamID
    )
    {
        // Set the leaderboard row data.
        if (rankText != null)
        {
            rankText.text =
                rank.ToString();
        }


        if (playerNameText != null)
        {
            playerNameText.text =
                playerName;
        }


        if (statText != null)
        {
            if (isCurrency)
            {
                float netWorth =
                    statValue / 100f;

                statText.text =
                    MoneyFormatter.FormatMoney(netWorth);
            }
            else
            {
                statText.text =
                    statValue.ToString("N0");
            }
        }


        // Profile picture.
        if (
            profileTexture != null &&
            profilePicture != null
        )
        {
            profilePicture.texture =
                profileTexture;
        }


        // Only make the CURRENT PLAYER'S row green.
        SetPlayerRowColor(
            steamID
        );


        // Load Steam avatar.
        LoadSteamAvatar(
            steamID
        );
    }



    // ============================================================
    // PLAYER ROW COLOR
    // ============================================================

    private void SetPlayerRowColor(
        CSteamID steamID
    )
    {
        if (!SteamManager.Initialized)
        {
            return;
        }


        CSteamID currentPlayerID =
            SteamUser.GetSteamID();


        // Only color the row green if this is
        // the actual player currently playing.
        if (steamID == currentPlayerID)
        {
            SetTextGreen(rankText);
            SetTextGreen(playerNameText);
            SetTextGreen(statText);
        }
    }



    private void SetTextGreen(
        TMP_Text text
    )
    {
        if (text == null)
        {
            return;
        }


        text.color =
            playerGreen;
    }



    // ============================================================
    // LOAD STEAM AVATAR
    // ============================================================

    private void LoadSteamAvatar(
        CSteamID steamID
    )
    {
        if (!SteamManager.Initialized)
            return;


        int avatarHandle =
            SteamFriends.GetLargeFriendAvatar(
                steamID
            );


        if (avatarHandle == -1)
        {
            // Steam is still loading the avatar.
            return;
        }


        if (avatarHandle == 0)
        {
            // No avatar available.
            return;
        }


        uint width;
        uint height;


        bool success =
            SteamUtils.GetImageSize(
                avatarHandle,
                out width,
                out height
            );


        if (
            !success ||
            width == 0 ||
            height == 0
        )
        {
            return;
        }


        byte[] imageData =
            new byte[
                width *
                height *
                4
            ];


        bool imageSuccess =
            SteamUtils.GetImageRGBA(
                avatarHandle,
                imageData,
                imageData.Length
            );


        if (!imageSuccess)
        {
            return;
        }


        // Steam's image data needs to be vertically flipped
        // before being displayed by Unity.
        byte[] flippedImageData =
            new byte[
                imageData.Length
            ];


        int rowSize =
            (int)width * 4;


        for (
            int y = 0;
            y < height;
            y++
        )
        {
            int sourceRow =
                y * rowSize;


            int destinationRow =
                (
                    (int)height -
                    1 -
                    y
                ) * rowSize;


            System.Array.Copy(
                imageData,
                sourceRow,
                flippedImageData,
                destinationRow,
                rowSize
            );
        }


        Texture2D avatarTexture =
            new Texture2D(
                (int)width,
                (int)height,
                TextureFormat.RGBA32,
                false
            );


        avatarTexture.LoadRawTextureData(
            flippedImageData
        );


        avatarTexture.Apply();


        if (profilePicture != null)
        {
            profilePicture.texture =
                avatarTexture;
        }
    }
}

