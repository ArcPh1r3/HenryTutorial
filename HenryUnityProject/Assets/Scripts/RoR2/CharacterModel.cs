using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class CharacterModel : MonoBehaviour
{
    public CharacterModel.RendererInfo[] baseRendererInfos = Array.Empty<CharacterModel.RendererInfo>();

    public CharacterModel.LightInfo[] baseLightInfos = Array.Empty<CharacterModel.LightInfo>();

    [Serializable]
    public struct RendererInfo
    {

        public Renderer renderer;
        public Material defaultMaterial;

        public ShadowCastingMode defaultShadowCastingMode;

        public bool ignoreOverlays;

        public bool hideOnDeath;
    }

    // Token: 0x0200063F RID: 1599
    [Serializable]
    public struct LightInfo
    {
        public Light light;

        public Color defaultColor;
    }
}
