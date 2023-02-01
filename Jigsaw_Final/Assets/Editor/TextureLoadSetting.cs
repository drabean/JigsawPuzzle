using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TextureLoadSetting : AssetPostprocessor
{
    void OnPreprocessTexture()
    {
        TextureImporter ti = (assetImporter as TextureImporter);
        ti.spritePivot = new Vector2(0.5f, 0.5f);
        ti.isReadable = true;
    }
}
