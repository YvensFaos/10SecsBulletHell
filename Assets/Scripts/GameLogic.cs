using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLogic : MonoBehaviour
{
    private static GameLogic _instance;

    public static GameLogic GetInstance()
    {
        return _instance;
    }

    private PlayerBehavior _player;
    public PlayerBehavior Player => _player;

    private TimeMeter _timeMeter;

    private LevelManager _levelManager;
    public LevelManager Level => _levelManager;

    [Header("Parameters")] [SerializeField]
    private float rebirthCost = 0.8f;

    [Header("Controllers")] [SerializeField]
    private XpPointsLabel xpPointsLabel;

    [SerializeField] private CameraShakeControl cameraShake;
    [SerializeField] private CounterBehavior counterBehavior;
    [SerializeField] private PlayerUpgradeManager playerUpgrade;
    [SerializeField] private UpgradePanelController upgradePanel;
    [SerializeField] private GameObject gameOverScreen;

    [Header("Cheats")] [SerializeField] private bool infiniteXp;

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(_instance.gameObject);
        }

        _instance = this;

        _player = FindObjectOfType<PlayerBehavior>();
        if (_player == null)
        {
            Debug.LogError("No Player Found In The Scene.");
            return;
        }

        _timeMeter = FindObjectOfType<TimeMeter>();
        if (_timeMeter == null)
        {
            Debug.LogError("No Time Meter Found In The Scene.");
            return;
        }

        _levelManager = FindObjectOfType<LevelManager>();
        if (_levelManager == null)
        {
            Debug.LogError("No Level Manager Found In The Scene.");
        }
    }

    private int _xpPoints;

    // private void Start()
    // {
    //    // StartLevel();
    // }

    public void StartGame()
    {
        _player.AllowControl(true);
        counterBehavior.gameObject.SetActive(true);
    }

    public void StartLevel()
    {
        _timeMeter.StartTimeMeter();
        _levelManager.SpawnMore(false);
    }

    public void NotifyTimesUp()
    {
        ++_xpPoints;
        if (xpPointsLabel != null)
        {
            xpPointsLabel.UpdateXpPointsLabel(_xpPoints);
        }

        StartLevel();
    }

    public void NotifyPlayerIsDead()
    {
        _timeMeter.StopTimeMeter();
        gameOverScreen.SetActive(true);
    }

    public bool TryToSpendXpPoints(int cost)
    {
        if (infiniteXp)
        {
            return true;
        }

        if (_xpPoints - cost >= 0)
        {
            _xpPoints -= cost;
            xpPointsLabel.UpdateXpPointsLabel(_xpPoints);
            return true;
        }

        return false;
    }

    public void PauseGame()
    {
        Time.timeScale = 0.0f;
    }

    public void UnpauseGame()
    {
        Time.timeScale = 1.0f;
    }

    public void OpenUpdateMenu()
    {
        upgradePanel.gameObject.SetActive(true);
    }

    public void RebirthPlayer()
    {
        _xpPoints = Mathf.FloorToInt(_xpPoints * rebirthCost);
        xpPointsLabel.UpdateXpPointsLabel(_xpPoints);
        _player.RebirthPlayer();
        _timeMeter.StartTimeMeter();
    }

    public void RestartScene()
    {
        var currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    public int GetXpPoints() => _xpPoints;
    public void CameraShake(float time) => cameraShake.ShakeCameraFor(time);

    public void CameraShake(float time, float amplitude, float frequency, Vector3 pivot) =>
        cameraShake.ShakeCameraFor(time, amplitude, frequency, pivot);

    public PlayerUpgradeManager UpgradeManager() => playerUpgrade;
}