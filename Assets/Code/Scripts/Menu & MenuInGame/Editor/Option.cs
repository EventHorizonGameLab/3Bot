using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(MenuOptions))]
public class GraphicsSettingsEditor : Editor
{
    private readonly List<System.Type> componentTypes = new()
    {
        typeof(BrightnessController),
        typeof(ResolutionController),
        typeof(FullScreenController),
        typeof(ColorBlindnessController),
        typeof(MixerController),
        typeof(MouseSensibility),
        typeof(InvertY)
    };

    private List<System.Type> addedComponents = new();

    private void OnEnable()
    {
        Undo.undoRedoPerformed += UpdateAddedComponentsList;
        UpdateAddedComponentsList();
    }

    private void OnDisable()
    {
        Undo.undoRedoPerformed -= UpdateAddedComponentsList;
    }

    private void UpdateAddedComponentsList()
    {
        addedComponents.Clear();

        var settings = (MenuOptions)target;
        var components = settings.GetComponents<Component>();

        foreach (var component in components)
        {
            if (component == settings) continue;
            addedComponents.Add(component.GetType());
        }
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GUILayout.Space(10);

        // Draw a button in the inspector
        if (GUILayout.Button("Add Component"))
        {
            // Open a popup menu to select the component to add
            GenericMenu menu = new GenericMenu();
            foreach (var componentType in componentTypes)
            {
                if (!addedComponents.Contains(componentType))
                {
                    menu.AddItem(new GUIContent(componentType.Name), false, () => { AddComponent(componentType); });
                }
            }
            menu.ShowAsContext();
        }
    }

    private void AddComponent(System.Type componentType)
    {
        var settings = (MenuOptions)target;

        // Add the selected component to the script
        var newComponent = settings.gameObject.AddComponent(componentType);

        // If the component was added successfully, remove it from the options list
        if (newComponent != null)
        {
            addedComponents.Add(componentType);
        }
    }
}
