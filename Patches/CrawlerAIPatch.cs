using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LCLobotomyFireInTheHole.Patches
{
    [HarmonyPatch(typeof(CrawlerAI))]
    internal class CrawlerAIPatch
    {
        [HarmonyPatch("MakeScreech")]
        [HarmonyPrefix]
        static void MakeScreechPatch(ref AudioClip[] ___longRoarSFX)
        {
            AudioClip[] newLongRoar = LobotomyModBase.demonSFX;
            ___longRoarSFX = newLongRoar;

            LobotomyModBase.mls.LogInfo("Replaced longRoarSFX");
        }

        [HarmonyPatch("HitPlayerClientRpc")]
        [HarmonyPrefix]
        static void HitPlayerPatch(ref AudioClip ___bitePlayerSFX)
        {
            AudioClip newBitePlayer = LobotomyModBase.demonSFX[0];
            ___bitePlayerSFX = newBitePlayer;

            LobotomyModBase.mls.LogInfo("Replaced bitePlayerSFX");
        }

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        static void UpdatePatch(
            ref bool ___hasEnteredChaseMode,
            ref AudioSource ___creatureVoice,
            CrawlerAI __instance
            )
        {
            if (!LobotomyModBase.lastNormalSFX.ContainsKey(___creatureVoice.gameObject))
            {
                LobotomyModBase.lastNormalSFX.Add(___creatureVoice.gameObject, 0);
            }
            if (!LobotomyModBase.nextDelays.ContainsKey(___creatureVoice.gameObject))
            {
                LobotomyModBase.nextDelays.Add(___creatureVoice.gameObject, UnityEngine.Random.Range(1, 4));
            }

            Transform transform = __instance.transform;
            GameObject obj = transform.gameObject;

            bool foundBillboard = false;
            foreach (Transform child in transform)
            {
                if (child.name == "billboard")
                {
                    foundBillboard = true;
                    GameObject cam = StartOfRound.Instance.activeCamera.gameObject;
                    Vector3 forward = cam.transform.forward;
                    child.forward = forward;
                    SpriteRenderer spriteRenderer = child.GetComponent<SpriteRenderer>();
                    if (___hasEnteredChaseMode && spriteRenderer.sprite != LobotomyModBase.demonSprite)
                        spriteRenderer.sprite = LobotomyModBase.demonSprite;
                    else if (!___hasEnteredChaseMode && spriteRenderer.sprite != LobotomyModBase.normalSprite)
                        spriteRenderer.sprite = LobotomyModBase.normalSprite;
                    break;
                }
            }

            if (!foundBillboard)
            {
                foreach (SkinnedMeshRenderer skinnedMeshRenderer in obj.GetComponentsInChildren<SkinnedMeshRenderer>())
                {
                    skinnedMeshRenderer.enabled = false;
                    LobotomyModBase.mls.LogInfo("disabled meshrenderer");
                }

                GameObject billboard = CrawlerAI.Instantiate(LobotomyModBase.billboardPrefab);
                billboard.name = "billboard";
                billboard.transform.SetParent(transform, false);
                billboard.transform.localPosition = new Vector3(0, 1.0f, 0);
            }

            LobotomyModBase.nextDelays.RemoveNullKeys();
            LobotomyModBase.lastNormalSFX.RemoveNullKeys();

            Int64 previousTime = LobotomyModBase.lastNormalSFX[___creatureVoice.gameObject];
            Int64 currentTime = System.DateTimeOffset.Now.ToUnixTimeSeconds();
            if (currentTime - previousTime > LobotomyModBase.nextDelays[___creatureVoice.gameObject])
            {
                LobotomyModBase.nextDelays[___creatureVoice.gameObject] = UnityEngine.Random.Range(1, 4);
                LobotomyModBase.lastNormalSFX[___creatureVoice.gameObject] = currentTime;

                if (___hasEnteredChaseMode)
                {
                    ___creatureVoice.PlayOneShot(LobotomyModBase.demonSFX[0]);
                    LobotomyModBase.mls.LogInfo("Played Demon SFX");
                }
                else
                {
                    ___creatureVoice.PlayOneShot(LobotomyModBase.normalSFX);
                    LobotomyModBase.mls.LogInfo("Played Normal SFX");
                }
            }
        }
    }
}
