using UnityEngine;
using UnityEditor;
using System.Collections;

[CanEditMultipleObjects]
[CustomEditor(typeof(UIShaderSprite), true)]
public class UIShaderSpriteInspector : UISpriteInspector
{
    protected override bool ShouldDrawProperties()
    {
        ShaderMenuUtility.ShaderField("Shader", serializedObject, "mShader", GUILayout.MinWidth(20f));
        return base.ShouldDrawProperties();
    }
}
