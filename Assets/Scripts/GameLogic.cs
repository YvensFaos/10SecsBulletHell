using UnityEngine;
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

   [SerializeField]
   private XpPointsLabel xpPointsLabel;
   [SerializeField]
   private CameraShakeControl cameraShake;
   [SerializeField] 
   private CounterBehavior counterBehavior;

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
         return;
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
   }

   public bool TryToSpendXpPoints(int cost)
   {
      if (_xpPoints - cost >= 0)
      {
         _xpPoints -= cost;
         return true;
      }

      return false;
   }

   public int GetXpPoints() => _xpPoints;
   public void CameraShake(float time) => cameraShake.ShakeCameraFor(time);
   public void CameraShake(float time, float amplitude, float frequency, Vector3 pivot) => cameraShake.ShakeCameraFor(time, amplitude, frequency, pivot);
}
