using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIShaderSprite : UISprite
{

    // Set to null if you just want to restore it to the original NGUI's shader.
    [HideInInspector]
    [SerializeField]
    Shader mShader;
    Material mMaterial;

    // cache materials
    static Dictionary<int, Dictionary<int, Material>> mCachedAtlasMaterials = new Dictionary<int, Dictionary<int, Material>>();

    /// <summary>
    /// Substitute UISprite's material to dynamically change a shader of the given atlas material.
    /// </summary>
    public override Material material
    {
        get
        {
            if (mShader != null)
            {
                if (mMaterial == null || mChanged)
                {
                    Dictionary<int, Material> shaderMaterials;
                    if (!mCachedAtlasMaterials.TryGetValue(atlas.GetInstanceID(), out shaderMaterials))
                        mCachedAtlasMaterials[atlas.GetInstanceID()] = shaderMaterials = new Dictionary<int, Material>();

                    if (!shaderMaterials.TryGetValue(mShader.GetInstanceID(), out mMaterial) || mMaterial == null)
                    {
                        if (mAtlas != null)
                            shaderMaterials[mShader.GetInstanceID()] = mMaterial = new Material(mAtlas.spriteMaterial) { shader = mShader };
                    }
                }
                return mMaterial;
            }
            return (mAtlas != null) ? mAtlas.spriteMaterial : null;
        }
    }

    /// <summary>
    /// Specified sprite to change material with the given shader so enables a shader on each of a sprite. 
    /// Pass null if it needs to be restored and NGUI handles a material.
    /// </summary>
    public void SetShader(Shader shd)
    {
        mShader = shd;
        panel.RebuildAllDrawCalls(); // need to force redraw to prevent not applying the shader on the sprite.
    }

}
