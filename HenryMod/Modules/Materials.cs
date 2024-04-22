using System.Collections.Generic;
using UnityEngine;

namespace HenryMod.Modules
{
    internal static class Materials
    {
        private static List<Material> cachedMaterials = new List<Material>();

        internal static Shader hotpoo = RoR2.LegacyResourcesAPI.Load<Shader>("Shaders/Deferred/HGStandard");

        public static Material LoadMaterial(this AssetBundle assetBundle, string materialName) => CreateHopooMaterialFromBundle(assetBundle, materialName);
        public static Material CreateHopooMaterialFromBundle(this AssetBundle assetBundle, string materialName)
        {
            Material tempMat = cachedMaterials.Find(mat =>
            {
                materialName.Replace(" (Instance)", "");
                return mat.name.Contains(materialName);
            });
            if (tempMat) {
                Log.Debug($"{tempMat.name} has already been loaded. returning cached");
                return tempMat;
            }
            tempMat = assetBundle.LoadAsset<Material>(materialName);

            if (!tempMat)
            {
                Log.ErrorAssetBundle(materialName, assetBundle.name);
                return new Material(hotpoo);
            }

            return tempMat.ConvertDefaultShaderToHopoo();
        }

        public static Material SetHopooMaterial(this Material tempMat) => ConvertDefaultShaderToHopoo(tempMat);
        public static Material ConvertDefaultShaderToHopoo(this Material tempMat)
        {
            if (cachedMaterials.Contains(tempMat)) {
                Log.Debug($"{tempMat.name} has already been converted. returning cached");
                return tempMat;
            }

            string name = tempMat.shader.name.ToLowerInvariant();
            if (!name.StartsWith("standard") && !name.StartsWith("autodesk"))
            {
                Log.Debug($"{tempMat.name} is not unity standard shader. aborting material conversion");
                return tempMat;
            }

            float? bumpScale = null;
            Color? emissionColor = null;

            //grab values before the shader changes
            if (tempMat.IsKeywordEnabled("_NORMALMAP"))
            {
                bumpScale = tempMat.GetFloat("_BumpScale");
            }
            if (tempMat.IsKeywordEnabled("_EMISSION"))
            {
                emissionColor = tempMat.GetColor("_EmissionColor");
            }

            //set shader
            tempMat.shader = hotpoo;

            //apply values after shader is set
            tempMat.SetTexture("_EmTex", tempMat.GetTexture("_EmissionMap"));
            tempMat.EnableKeyword("DITHER");
            
            if (bumpScale != null)
            {
                tempMat.SetFloat("_NormalStrength", (float)bumpScale);
                tempMat.SetTexture("_NormalTex", tempMat.GetTexture("_BumpMap"));
            }
            if (emissionColor != null)
            {
                tempMat.SetColor("_EmColor", (Color)emissionColor);
                tempMat.SetFloat("_EmPower", 1);
            }

            //set this keyword in unity if you want your model to show backfaces
            //in unity, right click the inspector tab and choose Debug
            if (tempMat.IsKeywordEnabled("NOCULL"))
            {
                tempMat.SetInt("_Cull", 0);
            }
            //set this keyword in unity if you've set up your model for limb removal item displays (eg. goat hoof) by setting your model's vertex colors
            if (tempMat.IsKeywordEnabled("LIMBREMOVAL"))
            {
                tempMat.SetInt("_LimbRemovalOn", 1);
            }

            cachedMaterials.Add(tempMat);
            return tempMat;
        }

        /// <summary>
        /// Makes this a unique material if we already have this material cached (i.e. you want an altered version). New material will not be cached
        /// <para>If it was not cached in the first place, simply returns as it is already unique.</para>
        /// </summary>
        public static Material MakeUnique(this Material material)
        {
            if (cachedMaterials.Contains(material))
            {
                return new Material(material);
            }
            return material;
        }

        public static Material SetColor(this Material material, Color color)
        {
            material.SetColor("_Color", color);
            return material;
        }

        public static Material SetNormal(this Material material, float normalStrength = 1)
        {
            material.SetFloat("_NormalStrength", normalStrength);
            return material;
        }

        public static Material SetEmission(this Material material) => SetEmission(material, 1);
        public static Material SetEmission(this Material material, float emission) => SetEmission(material, emission, Color.white);
        public static Material SetEmission(this Material material, float emission, Color emissionColor)
        {
            material.SetFloat("_EmPower", emission);
            material.SetColor("_EmColor", emissionColor);
            return material;
        }
        public static Material SetCull(this Material material, bool cull = false)
        {
            material.SetInt("_Cull", cull ? 1 : 0);
            return material;
        }

        public static Material SetSpecular(this Material material, float strength)
        {
            material.SetFloat("_SpecularStrength", strength);
            return material;
        }
        public static Material SetSpecular(this Material material, float strength, float exponent)
        {
            material.SetFloat("_SpecularStrength", strength);
            material.SetFloat("SpecularExponent", exponent);
            return material;
        }
    }
}