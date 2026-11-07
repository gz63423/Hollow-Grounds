using UnityEngine;

public class WishlistButton : MonoBehaviour
{
    private const string SteamStoreURL =
        "https://store.steampowered.com/app/5212390/";

    public void OpenWishlistPage()
    {
        Application.OpenURL(SteamStoreURL);
    }
}