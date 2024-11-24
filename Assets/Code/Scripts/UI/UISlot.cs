using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

public class UISlot : MonoBehaviour
{
    [Serializable]
    public struct SpriteBinding
    {
        [TableColumnWidth(57, Resizable = false)]
        [PreviewField(Alignment = ObjectFieldAlignment.Center)]
        public Sprite Sprite;

        [ValueDropdown(nameof(GetComponentTypeOptions))]
        public string ComponentTypeName;

        public Type ComponentType => Type.GetType(ComponentTypeName); 

        private static IEnumerable<string> GetComponentTypeOptions()
        {
            return new List<string>
        {
            typeof(MedKit).FullName,
            typeof(BulletMagazine).FullName,
            typeof(Explosive).FullName
        };
        }
    }


    [Title("Settings")]
    [TableList(DrawScrollView = true, MaxScrollViewHeight = 200, MinScrollViewHeight = 100)]
    [SerializeField, Tooltip("List of Component-Sprite bindings")]
    private List<SpriteBinding> spriteBindings = new();

    private Dictionary<Type, Sprite> bindingDictionary = new();

    [Title("Debug")]
    [SerializeField] private bool _debug = false;

    public static event Action<Sprite> OnSpriteChanged;

    private void Awake()
    {
        RebuildDictionary();
    }

    private void OnEnable()
    {
        MagnetSystem.OnObjectStored += FindReference;
    }

    private void OnDisable()
    {
        MagnetSystem.OnObjectStored -= FindReference;
    }

    private void RebuildDictionary()
    {
        bindingDictionary.Clear();
        foreach (var binding in spriteBindings)
        {
            if (string.IsNullOrEmpty(binding.ComponentTypeName))
            {
                if (_debug) Debug.LogWarning("ComponentTypeName is empty in one of the sprite bindings.");
                continue;
            }

            Type type = binding.ComponentType;
            if (type == null)
            {
                Debug.LogWarning($"ComponentType '{binding.ComponentTypeName}' could not be resolved.");
                continue;
            }

            if (!bindingDictionary.ContainsKey(type))
            {
                bindingDictionary.Add(type, binding.Sprite);
            }
            else if (_debug)
            {
                Debug.LogWarning($"Duplicate key found: {type}");
            }
        }
    }

    private void FindReference(GameObject targetObject)
    {
        if (_debug) Debug.Log($"FindReference Requested for {(targetObject != null ? targetObject.name : "null")}");

        if (targetObject != null)
        {
            foreach (var binding in spriteBindings)
            {
                if (targetObject.TryGetComponent(binding.ComponentType, out _))
                {
                    if (_debug) Debug.Log($"FindReference Found: {binding.ComponentType.Name} on {targetObject.name}");

                    OnSpriteChanged?.Invoke(binding.Sprite);
                    return;

                }
            }
        }

        if (_debug) Debug.Log("FindReference Not Found");
        OnSpriteChanged?.Invoke(null);
    }

    private void OnValidate()
    {
        RebuildDictionary();
    }
}
