using BetterFireplace.Tools;
using HarmonyLib;
using UnityEngine;

namespace BetterFireplace.Patchers
{
    // 文本替换 Patch
    [HarmonyPatch(typeof(TextAsset), nameof(TextAsset.text), MethodType.Getter)]
    public static class TextAssetPatch
    {
        public static void Postfix(TextAsset __instance, ref string __result)
        {
            // [性能敏感] 这个Getter每帧可能被调用多次
            // TextAssetManager 内部必须是 O(1) 的缓存查找，不要在这里做重逻辑
            if (__instance != null && TextAssetManager.TryGetModifiedContent(__instance.name, __result, out string? modified))
            {
                if (modified != null) __result = modified;
            }
        }
    }
}