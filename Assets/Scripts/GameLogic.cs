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
   }

   public void StartLevel()
   {
      _timeMeter.StartTimeMeter();
      _player.AllowControl(true);
   }

   public void NotifyTimesUp()
   {
      
   }

   public void NotifyPlayerIsDead()
   {
      _timeMeter.StopTimeMeter();
   }
}
