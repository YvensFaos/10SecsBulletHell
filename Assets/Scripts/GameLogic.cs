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

    [Header("Parameters")] 
    [SerializeField]
    private float rebirthCost = 0.8f;

    private int _xpPerRound = 1;

    [Header("Controllers")] 
    [SerializeField] private XpPointsLabel xpPointsLabel;
    [SerializeField] private CameraShakeControl cameraShake;
    [SerializeField] private CounterBehavior counterBehavior;
    [SerializeField] private PlayerUpgradeManager playerUpgrade;
    [SerializeField] private UpgradePanelController upgradePanel;
    [SerializeField] private GameObject gameOverScreen;

    [Header("LeanPool Prefabs")] 
    [SerializeField] private Transform playerBulletsTransform;
    [SerializeField] private Transform enemyBulletsTransform;
    [SerializeField] private Transform damageParticlesTransform;
    [SerializeField] private Transform destructionParticlesTransform;

    [Header("Cheats")] 
    [SerializeField] private bool infiniteXp;

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
        
        #if !UNITY_EDITOR
            infiniteXp = false;
        #endif
    }

    private int _xpPoints;

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
        _xpPoints += _xpPerRound;
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
        _player.AllowControl(false);
    }

    public void UnpauseGame()
    {
        Time.timeScale = 1.0f;
        _player.AllowControl(true);
    }

    public void OpenUpdateMenu()
    {
        upgradePanel.gameObject.SetActive(true);
    }

    public void IncreaseXpPerRound()
    {
        _xpPerRound++;
    }

    public void RebirthPlayer()
    {
        _xpPoints = Mathf.FloorToInt(_xpPoints - (_xpPoints * rebirthCost));
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
    
    public Transform PlayerBulletsTransform() => playerBulletsTransform;
    public Transform EnemyBulletsTransform() => enemyBulletsTransform;
    public Transform DamageParticlesTransform() => damageParticlesTransform;
    public Transform DestructionParticlesTransform() => destructionParticlesTransform;
}