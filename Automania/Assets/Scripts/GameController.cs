using UnityEngine;

public class GameController : MonoBehaviour
{
    private static GameController instance;

    public static GameController Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<GameController>();
                if (instance == null)
                {
                    instance = new GameObject().AddComponent<GameController>();
                }
            }

            return instance;
        }
    }

    [SerializeField] private LevelBuilder levelBuilder;

    public void ChangeRoom()
    {
        levelBuilder.ChangeRoom();
    }
}
