using System;
using UnityEngine;
using System.Collections;
using System.IO;
using System.Linq;
using Invert.Core.GraphDesigner;
using uFrame2;
using UnityEditor;

public class KernelConfigWindow : EditorWindow {
    private SceneSelectionItem[] _kernelSceneOptions;
    private int _selectedIndex;

    [MenuItem("uFrame/Kernel Configuration")]
    public static void Init()
    {
        var window = GetWindow<KernelConfigWindow>();
        window.Repaint();
        window.Show();
    }

    public Rect Bounds
    {
        get
        {
            return new Rect(0, 0, this.position.width, this.position.height);
        }
    }

    public Rect PaddedBounds
    {
        get
        {
            return new Rect(0, 0, position.width, position.height).PadSides(15);
        }
    }

    public int SelectedIndex
    {
        get { return _selectedIndex; }
        set { _selectedIndex = value; }
    }

    public SceneSelectionItem[] KernelSceneOptions
    {
        get { return new[]
        {
            new SceneSelectionItem()
            {
                SceneName = null,
                Title = "None",
                Index = -1,
                Scene = null
            }
        }.Concat(EditorBuildSettings.scenes.Select((s,i)=> new SceneSelectionItem()
        {
            Scene = s,
            Title = Path.GetFileNameWithoutExtension(s.path),
            SceneName = Path.GetFileNameWithoutExtension(s.path),
            Index = i
        })).ToArray(); }
        set { _kernelSceneOptions = value; }
    }

    public void Awake()
    {
        var selectedScene = KernelRegister.RootKernelScene;

        if (string.IsNullOrEmpty(selectedScene))
        {
            SelectedIndex = 0;
        }
        else
        {
        SelectedIndex = Array.IndexOf(EditorBuildSettings.scenes.Select(s=>Path.GetFileNameWithoutExtension(s.path)).ToArray(),selectedScene);
        }
    }

    public void OnGUI()
    {
        var kernelSceneSelectionRowBounds = PaddedBounds.WithHeight(40);
        EditorGUI.BeginChangeCheck();
        var selectedIndex = EditorGUI.Popup(kernelSceneSelectionRowBounds, "Select Kernel Scene", SelectedIndex,
            KernelSceneOptions.Select(s => s.Title).ToArray());
        if(EditorGUI.EndChangeCheck())
        {
            if (selectedIndex >= 0 && selectedIndex < KernelSceneOptions.Length)
            {
                SelectedIndex = selectedIndex;
                KernelRegister.RootKernelScene = KernelSceneOptions[selectedIndex].SceneName;
            }
        }
    }

    public class SceneSelectionItem
    {
        public string SceneName { get; set; }
        public EditorBuildSettingsScene Scene { get; set; }
        public int Index { get; set; }
        public string Title { get; set; }
    }

}
