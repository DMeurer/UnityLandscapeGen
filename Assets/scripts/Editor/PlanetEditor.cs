using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Planet))]
public class PlanetEditor : Editor
{
    private Planet _planet;

    private Editor _shapeEditor;
    private Editor _colorEditor;

    public override void OnInspectorGUI()
    {
        using var check = new EditorGUI.ChangeCheckScope();
        base.OnInspectorGUI();
        if (check.changed)
        {
            _planet.GeneratePlanet();
        }
        
        if (GUILayout.Button("Generate Planet"))
        {
            _planet.GeneratePlanet();
        }

        // draw settings editor
        DrawSettingsEditor(_planet.shapeSettings, _planet.OnShapeSettingsUpdated, ref _planet.shapeSettingsFoldout,
            ref _shapeEditor);
        DrawSettingsEditor(_planet.colorSettings, _planet.OnColorSettingsUpdated, ref _planet.colorSettingsFoldout,
            ref _colorEditor);
    }

    private static void DrawSettingsEditor(Object settings, System.Action onSettingsUpdated, ref bool foldout,
        ref Editor editor)
    {
        if (settings == null) return;
        
        foldout = EditorGUILayout.InspectorTitlebar(foldout, settings);

        // check if settings have changed
        using var check = new EditorGUI.ChangeCheckScope();

        // only draw if foldout is true
        if (foldout == false) return;
        // draw default editor
        // Editor editor = CreateEditor(settings);
        CreateCachedEditor(settings, null, ref editor);
        editor.OnInspectorGUI();
        // check if settings have changed
        if (!check.changed) return;
        if (onSettingsUpdated != null)
        {
            onSettingsUpdated();
        }
    }

    private void OnEnable()
    {
        _planet = (Planet)target;
    }
}