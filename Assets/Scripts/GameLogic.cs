using System.Collections;
using System.Collections.Generic;
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
   }

   public void NotifyPlayerIsDead()
   {
      
   }
}
