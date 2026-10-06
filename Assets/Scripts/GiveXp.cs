using UnityEngine;

public class GiveXP : MonoBehaviour
{
    public int xpReward = 10;


    public void Give()
    {
        Debug.Log(
            "Giving XP: " +
            xpReward
        );


        if (AccountLevel.Instance != null)
        {
            AccountLevel.Instance.AddXP(xpReward);
        }
        else
        {
            Debug.LogError(
                "AccountLevel Instance not found!"
            );
        }
    }
}