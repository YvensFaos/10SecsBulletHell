using UnityEngine;

public class DifficultyController : MonoBehaviour
{
    [SerializeField]
    private GameObject maximumLevelReachedObject;

    public void ClickMe()
    {
        var maxLevelReached = GameLogic.GetInstance().Level.MaxLevelReached();
        if (!maxLevelReached)
        {
            GameLogic.GetInstance().IncreaseXpPerRound();
            maxLevelReached = GameLogic.GetInstance().Level.SpawnMore(true);
        }

        if (maxLevelReached)
        {
            maximumLevelReachedObject.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}
