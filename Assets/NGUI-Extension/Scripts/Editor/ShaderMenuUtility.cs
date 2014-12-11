//////////////////////////////////////////////////////////////////////////
//
// ShaderMenuUtility
//
// Original code is from here:
//    http://answers.unity3d.com/questions/307421/select-shader-in-custom-editor-gui.html 
//
//////////////////////////////////////////////////////////////////////////
using UnityEngine;
using UnityEditor;
using System.Linq;


/// <summary>
/// 
/// </summary>
public static class ShaderMenuUtility
{

    #region Helper

    private static GUIContent _tempText = new GUIContent();

    private static GUIContent TempContent(string text)
    {
        _tempText.text = text;
        return _tempText;
    }

    #endregion

    #region Shader Name Cache

    private static GUIContent[] _shaderNames;

    //-------------------------------------------------------------------------
    private static void ClearShaderCache()
    {
        _shaderNames = null;
    }

    private static Material _dummyMaterial;

    //-------------------------------------------------------------------------
    private static void PrepareShaderCache()
    {
        if (_dummyMaterial == null)
        {
            _dummyMaterial = new Material(Shader.Find("Diffuse"));
            _dummyMaterial.hideFlags = HideFlags.HideAndDontSave;
        }

        // This is a little wasteful, but unfortunately needed :/
        UnityEditorInternal.InternalEditorUtility.SetupShaderMenu(_dummyMaterial);

        // Fetch shaders and filter
        Shader[] shaders = (Shader[])UnityEngine.Resources.FindObjectsOfTypeAll(typeof(Shader));
        shaders = shaders.Where(s => s != null && s.name != "" && !s.name.StartsWith("__")).ToArray();

        // Generate list of shader names
        _shaderNames = shaders.Select(s => new GUIContent(s.name)).ToArray();
    }

    #endregion

    //-------------------------------------------------------------------------
    // EditorGUI style version
    public static Shader ShaderField(Rect position, string label, Shader shader)
    {
        int controlID = GUIUtility.GetControlID(FocusType.Passive);
        EventType eventType = Event.current.GetTypeForControl(controlID);

        // Clear shader cache before layout is processed for control
        if (eventType == EventType.Layout)
            ClearShaderCache();

        if (!string.IsNullOrEmpty(label))
            position = EditorGUI.PrefixLabel(position, controlID, TempContent(label));

        // Prepare list of shaders
        if (_shaderNames == null)
            PrepareShaderCache();

        int selectedIndex = (shader != null)
            ? System.Array.FindIndex(_shaderNames, c => c.text == shader.name)
            : -1;

        EditorGUI.BeginChangeCheck();
        selectedIndex = EditorGUI.Popup(position, selectedIndex, _shaderNames);
        if (EditorGUI.EndChangeCheck())
        {
            shader = (selectedIndex >= 0 && selectedIndex < _shaderNames.Length)
                ? Shader.Find(_shaderNames[selectedIndex].text)
                : null;
        }

        return shader;
    }

    //-------------------------------------------------------------------------
    // EditorGUILayout style version
    public static Shader ShaderField(string label, Shader shader, params GUILayoutOption[] options)
    {
        Rect position = GUILayoutUtility.GetRect(GUIContent.none, EditorStyles.popup, options);
        return ShaderField(position, label, shader);
    }

    //-------------------------------------------------------------------------
    /// <summary>
    /// Used to reflect a shader setting on UIShaderSprite component.
    /// </summary>
    public static SerializedProperty ShaderField(string label, SerializedObject serializedObject, string property, params GUILayoutOption[] options)
    {
        SerializedProperty sp = serializedObject.FindProperty(property);
        if (sp != null)
        {
            Shader shader = sp.objectReferenceValue as UnityEngine.Object as Shader;
            if (shader == null)
            {
                shader = Shader.Find("Unlit/Transparent Colored");
            }
            Rect position = GUILayoutUtility.GetRect(GUIContent.none, EditorStyles.popup, options);
            shader = ShaderField(position, label, shader);
            sp.objectReferenceValue = shader;
        }
        return sp;
    }

}