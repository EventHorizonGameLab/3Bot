using PlayerSM;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CheckPointManager : MonoBehaviour
{
    [Serializable]
    public class Trigger : UnityEvent { }

    [BoxGroup("Saving")]
    [SerializeField] private Trigger OnSaveStart = new();

    [BoxGroup("Saving")]
    [SerializeField] private Trigger OnSaveEnd = new();

    [Title("Debug")]
    [SerializeField] private bool _debug = false;

    private CheckpointInfo _currentCheckpoint;

    Dictionary<int, GameObject> itemsInfoDictiornary = new();

    public void Start() { }

    public void SaveCheckpoint()
    {
        if (_debug) Debug.Log("Saving Checkpoint");

        OnSaveStart?.Invoke();

        itemsInfoDictiornary.Clear();

        _currentCheckpoint = new CheckpointInfo();

        // Salva stato del giocatore
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            PlayerController playerState = player.GetComponent<PlayerController>();
            GunSettings gun = player.GetComponentInChildren<GunSettings>();
            Health health = player.GetComponent<Health>();
            MagnetSystem magneticGun = player.GetComponentInChildren<MagnetSystem>();

            _currentCheckpoint.PlayerInfo = new PlayerInfo
            {
                Position = player.transform.position,
                Rotation = player.transform.rotation,
                State = playerState.CurrentState,
                CurrentAmmo = gun.currentAmmo,
                StorageAmmo = gun.storageAmmo,
                CurrentHealth = health.health,
                Slot = magneticGun.Slot
            };
        }

        if(GameObject.Find("Timer").TryGetComponent<Timer>(out var timer))
        {
            _currentCheckpoint.TimerInfo = new TimerInfo
            {
                Time = timer.TimeValue,
            };
        }

        //// Salva stato dei nemici
        //Enemy[] enemies = FindObjectsOfType<Enemy>();
        //foreach (var enemy in enemies)
        //{
        //    _currentCheckpoint.EnemiesInfo.Add(new EnemyInfo
        //    {
        //        Name = enemy.name,
        //        IsActive = enemy.gameObject.activeSelf,
        //        Position = enemy.transform.position,
        //        Rotation = enemy.transform.rotation,
        //        CurrentHealth = enemy.Health,
        //        State = enemy.CurrentState // Supponendo che Enemy abbia una FSM
        //    });
        //}

        ObjectID[] items = FindObjectsOfType<ObjectID>();
        foreach (var item in items)
        {
            _currentCheckpoint.ItemsInfo.Add(new ItemInfo
            {
                ItemID = item.objectID,
                IsActive = item.gameObject.activeSelf,
                Position = item.transform.position,
                Rotation = item.transform.rotation,
                Parent = item.transform.parent,
                Scale = item.transform.lossyScale,
                Gravity = item.GetComponent<Rigidbody>().useGravity
            });
            itemsInfoDictiornary.Add(item.objectID, item.gameObject);
        }

        if (_debug) Debug.Log("Checkpoint Saved!");

        OnSaveEnd?.Invoke();
    }

    // Ripristina lo stato del checkpoint salvato
    public void LoadCheckpoint()
    {
        if (_debug) Debug.Log("Loading Checkpoint");

        if (_currentCheckpoint == null)
        {
            Debug.LogWarning("No checkpoint saved!");

            // reload scene+

            return;
        }

        // Ripristina stato del giocatore
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null && _currentCheckpoint.PlayerInfo != null)
        {
            var playerInfo = _currentCheckpoint.PlayerInfo;
            PlayerController playerState = player.GetComponent<PlayerController>();
            GunSettings gun = player.GetComponentInChildren<GunSettings>();
            Health health = player.GetComponent<Health>();
            MagnetSystem magneticGun = player.GetComponentInChildren<MagnetSystem>();

            player.transform.SetPositionAndRotation(playerInfo.Position, playerInfo.Rotation);
            gun.currentAmmo = playerInfo.CurrentAmmo;
            gun.storageAmmo = playerInfo.StorageAmmo;
            playerState.CurrentState = playerInfo.State;
            health.health = playerInfo.CurrentHealth;
            magneticGun.Slot = playerInfo.Slot;
        }

        if (GameObject.Find("Timer").TryGetComponent<Timer>(out var timer))
        {
            var timerInfo = _currentCheckpoint.TimerInfo;
            timer.TimeValue = timerInfo.Time;
        }

        //// Ripristina stato dei nemici
        //foreach (var enemyInfo in _currentCheckpoint.EnemiesInfo)
        //{
        //    GameObject enemy = GameObject.Find(enemyInfo.Name);
        //    if (enemy != null)
        //    {
        //        enemy.SetActive(enemyInfo.IsActive);
        //        enemy.transform.position = enemyInfo.Position;
        //        enemy.transform.rotation = enemyInfo.Rotation;

        //        var enemyScript = enemy.GetComponent<Enemy>();
        //        if (enemyScript != null)
        //        {
        //            enemyScript.Health = enemyInfo.CurrentHealth;
        //            enemyScript.CurrentState = enemyInfo.State;
        //        }
        //    }
        //}

        foreach (var itemInfo in _currentCheckpoint.ItemsInfo)
        {
            GameObject item = itemsInfoDictiornary[itemInfo.ItemID];
            item.SetActive(itemInfo.IsActive);
            item.transform.SetPositionAndRotation(itemInfo.Position, itemInfo.Rotation);
            item.transform.localScale = itemInfo.Scale;
            item.transform.SetParent(itemInfo.Parent);
            item.GetComponent<Rigidbody>().useGravity = itemInfo.Gravity;
        }

        if (_debug) Debug.Log("Checkpoint Loaded!");
    }

    #region Data Structures

    public class CheckpointInfo
    {
        public PlayerInfo PlayerInfo { get; set; } = new PlayerInfo();
        public TimerInfo TimerInfo { get; set; } = new TimerInfo();
        public List<EnemyInfo> EnemiesInfo { get; set; } = new List<EnemyInfo>();
        public List<ItemInfo> ItemsInfo { get; set; } = new List<ItemInfo>();
    }

    public class PlayerInfo
    {
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
        public int State { get; set; }
        public int CurrentAmmo { get; set; }
        public int StorageAmmo { get; set; }
        public float CurrentHealth { get; set; }
        public GameObject Slot { get; set; }
    }

    public class EnemyInfo
    {
        public bool IsActive { get; set; }
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
        public float CurrentHealth { get; set; }
        public float CurrentAmmo { get; set; }
        //public string State { get; set; } // Identificativo della FSM del nemico
    }

    public class ItemInfo
    {
        public int ItemID { get; set; }
        public bool IsActive { get; set; }
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
        public Transform Parent { get; set; }
        public Vector3 Scale { get; set; }
        public bool Gravity { get; set; }
    }

    public class TimerInfo
    {
        public float Time { get; set; }
    }

    #endregion

}
