using PlayerSM;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

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

    Dictionary<int, GameObject> itemsInfoDictionary = new();
    Dictionary<int, GameObject> enemyInfoDictionary = new();

    public void SaveCheckpoint()
    {
        if (_debug) Debug.Log("Saving Checkpoint");

        OnSaveStart?.Invoke();

        itemsInfoDictionary.Clear();

        _currentCheckpoint = new CheckpointInfo();

        // Save Player Info
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
                StateStatus = new List<bool>(playerState.GetStateStatus().Values),
                CurrentAmmo = gun.currentAmmo,
                StorageAmmo = gun.storageAmmo,
                CurrentHealth = health.health,
                Slot = magneticGun.Slot
            };
        }

        // Save Timer Info
        if (GameObject.Find("Timer").TryGetComponent<Timer>(out var timer))
        {
            _currentCheckpoint.TimerInfo = new TimerInfo
            {
                Time = timer.TimeValue,
            };
        }

        // Save Enemies Info
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        foreach (var enemy in enemies)
        {
            int enemyId = enemy.GetComponent<EnemyID>().enemyID;

            _currentCheckpoint.EnemiesInfo.Add(new EnemyInfo
            {
                EnemyID = enemyId,
                IsActive = enemy.gameObject.activeSelf,
                Position = enemy.transform.position,
                Rotation = enemy.transform.rotation,
                CurrentHealth = enemy.GetComponent<Health>().health,
                CurrentAmmo = enemy.GetComponentInChildren<GunSettings>().currentAmmo
            });
            enemyInfoDictionary.Add(enemyId, enemy.gameObject);
        }

        // Save Items Info
        ObjectID[] items = FindObjectsOfType<ObjectID>();
        foreach (var item in items)
        {
            var rb = item.GetComponent<Rigidbody>();
            _currentCheckpoint.ItemsInfo.Add(new ItemInfo
            {
                ItemID = item.objectID,
                IsActive = item.gameObject.activeSelf,
                Position = item.transform.position,
                Rotation = item.transform.rotation,
                Parent = item.transform.parent,
                Scale = item.transform.lossyScale,
                Gravity = rb != null && rb.useGravity,
                RbConstraints = rb != null ? rb.constraints : RigidbodyConstraints.None,
                ColliderEnable = item.GetComponent<Collider>() != null && item.GetComponent<Collider>().enabled
            });
            itemsInfoDictionary.Add(item.objectID, item.gameObject);
        }

        if (_debug) Debug.Log("Checkpoint Saved!");

        OnSaveEnd?.Invoke();
    }

    public void LoadCheckpoint()
    {
        if (_debug) Debug.Log("Loading Checkpoint");

        if (_currentCheckpoint == null)
        {
            Debug.LogWarning("No checkpoint saved!");
            SceneSwitch.instance.ReLoadScene(SceneManager.GetActiveScene().name);
            return;
        }

        // Load Player Info
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

            var stateStatus = new Dictionary<IPlayerState, bool>();
            for (int i = 0; i < playerState.GetStateStatus().Count; i++)
            {
                stateStatus[playerState.GetStateStatus().Keys.ElementAt(i)] = playerInfo.StateStatus[i];
            }
            playerState.SetStateStatus(stateStatus);
        }

        // Load Timer Info
        if (GameObject.Find("Timer").TryGetComponent<Timer>(out var timer))
        {
            var timerInfo = _currentCheckpoint.TimerInfo;
            timer.TimeValue = timerInfo.Time;
        }

        // Load Enemies Info
        foreach (var enemyInfo in _currentCheckpoint.EnemiesInfo)
        {
            if (!enemyInfoDictionary.TryGetValue(enemyInfo.EnemyID, out var enemy)) continue;

            enemy.SetActive(enemyInfo.IsActive);
            enemy.transform.SetPositionAndRotation(enemyInfo.Position, enemyInfo.Rotation);
            enemy.GetComponent<Health>().health = enemyInfo.CurrentHealth;
            enemy.GetComponentInChildren<GunSettings>().currentAmmo = enemyInfo.CurrentAmmo;
        }

        // Load Items Info
        foreach (var itemInfo in _currentCheckpoint.ItemsInfo)
        {
            if (!itemsInfoDictionary.TryGetValue(itemInfo.ItemID, out var item)) continue;

            item.SetActive(itemInfo.IsActive);
            item.transform.SetPositionAndRotation(itemInfo.Position, itemInfo.Rotation);
            item.transform.localScale = itemInfo.Scale;
            item.transform.SetParent(itemInfo.Parent);

            if (item.TryGetComponent<Rigidbody>(out var rb))
            {
                rb.useGravity = itemInfo.Gravity;
                rb.constraints = itemInfo.RbConstraints;
            }

            if (item.TryGetComponent<Collider>(out var collider))
            {
                collider.enabled = itemInfo.ColliderEnable;
            }
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
        public List<bool> StateStatus { get; set; }
        public int CurrentAmmo { get; set; }
        public int StorageAmmo { get; set; }
        public float CurrentHealth { get; set; }
        public GameObject Slot { get; set; }
    }

    public class EnemyInfo
    {
        public int EnemyID { get; set; }
        public bool IsActive { get; set; }
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
        public float CurrentHealth { get; set; }
        public int CurrentAmmo { get; set; }
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
        public RigidbodyConstraints RbConstraints { get; set; }
        public bool ColliderEnable { get; set; }
    }

    public class TimerInfo
    {
        public float Time { get; set; }
    }

    #endregion
}
