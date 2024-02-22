using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using LCLobotomyFireInTheHole.Patches;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace LCLobotomyFireInTheHole
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class LobotomyModBase : BaseUnityPlugin
    {
        private const string modGUID = "z33r0x.lclobotomy";
        private const string modName = "Lethal Company Lobotomy";
        private const string modVersion = "1.0.0.0";

        private readonly Harmony harmony = new Harmony(modGUID);

        private static LobotomyModBase Instance;

        internal static AudioClip[] demonSFX;
        internal static AudioClip normalSFX;

        internal static ManualLogSource mls;
        
        internal static Dictionary<GameObject, Int64> lastNormalSFX = new Dictionary<GameObject, Int64>() { };
        internal static Dictionary<GameObject, int> nextDelays = new Dictionary<GameObject, int>() { };

        internal static Texture2D normalTexture;
        internal static Texture2D demonTexture;

        internal static Sprite normalSprite;
        internal static Sprite demonSprite;

        internal static GameObject billboardPrefab;

        internal static Dictionary<AudioSource, GameObject> currentObjects = new Dictionary<AudioSource, GameObject>() { };

        private const string dllName = "LCLobotomyFireInTheHole.dll";

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            
            mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);

            mls.LogInfo(modName + " has awaken");

            string location = ((BaseUnityPlugin)Instance).Info.Location;
            string directory = location.TrimEnd(dllName.ToCharArray());

            AssetBundle bundleDemon = AssetBundle.LoadFromFile(directory + "fireinthehole");
            AssetBundle bundleNormal = AssetBundle.LoadFromFile(directory + "highpitchfireinthehole");
            
            AssetBundle bundleTextureNormal = AssetBundle.LoadFromFile(directory + "normalface");
            AssetBundle bundleTextureDemon = AssetBundle.LoadFromFile(directory + "demonface");
            AssetBundle bundleBillboard = AssetBundle.LoadFromFile(directory + "billboardbundle");

            if ((UnityEngine.Object)bundleDemon == (UnityEngine.Object)null || (UnityEngine.Object)bundleNormal == (UnityEngine.Object)null)
            {
                mls.LogError("failed to load assets!");
                return;
            }

            demonSFX = bundleDemon.LoadAllAssets<AudioClip>();
            normalSFX = bundleNormal.LoadAllAssets<AudioClip>()[0];

            normalTexture = bundleTextureNormal.LoadAllAssets<Texture2D>()[0];
            demonTexture = bundleTextureDemon.LoadAllAssets<Texture2D>()[0];
            billboardPrefab = bundleBillboard.LoadAllAssets<GameObject>()[0];

            demonSprite = Sprite.Create(demonTexture, new Rect(0, 0, demonTexture.width, demonTexture.height), new Vector2(0.5f, 0.5f), 100);
            normalSprite = Sprite.Create(normalTexture, new Rect(0, 0, normalTexture.width, normalTexture.height), new Vector2(0.5f, 0.5f), 100);

            mls.LogInfo(normalSFX);

            harmony.PatchAll(typeof(CrawlerAIPatch));

            mls.LogInfo("Loaded Lobotomy mod!");
        }
    }
}
