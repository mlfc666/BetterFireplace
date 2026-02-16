using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using BetterFireplace.Components;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace BetterFireplace;

[BepInPlugin(MyPluginInfo.PluginGuid, MyPluginInfo.PluginName, MyPluginInfo.PluginVersion)]
public class Plugin : BasePlugin
{
    public new static ManualLogSource Log;

    // 将版本号公开，以便 Controller 读取
    public const string TargetVersion = "0.20251031.13";

    public override void Load()
    {
        // 强制控制台 UTF-8 (解决中文乱码)
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Log = base.Log;

        // 注册 IL2CPP 类型 
        ClassInjector.RegisterTypeInIl2Cpp<ModController>();

        // 创建控制器
        var go = new GameObject("MlfcModController");
        Object.DontDestroyOnLoad(go);
        go.hideFlags = HideFlags.HideAndDontSave; // 防止被游戏内的清理脚本误删

        // 挂载脚本
        go.AddComponent<ModController>();

        // 应用 Harmony 补丁
        // PatchAll 比较耗时，放在最后，确保前面关键组件已就绪
        new Harmony(MyPluginInfo.PluginGuid).PatchAll();
    }
}

public static class MyPluginInfo
{
    public const string PluginGuid = "MlfcBetterFireplace";
    public const string PluginName = "MlfcBetterFireplace";
    public const string PluginVersion = "0.0.1";
}