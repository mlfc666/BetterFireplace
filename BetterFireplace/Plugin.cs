using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;

namespace BetterFireplace;

[BepInPlugin(MyPluginInfo.PluginGuid, MyPluginInfo.PluginName, MyPluginInfo.PluginVersion)]
public class Plugin : BasePlugin
{
    public new static ManualLogSource? Log;

    public override void Load()
    {
    }
}

public static class MyPluginInfo
{
    public const string PluginGuid = "moe.mlfc.BetterFireplace";
    public const string PluginName = "MlfcBetterFireplace";
    public const string PluginVersion = "1.0.20260218.18";
}

/// <summary>
/// 该类由 AssetRedux 核心框架通过反射读取，用于版本校验。
/// </summary>
public static class AssetReduxConfig
{
    /// <summary>
    /// 设定该 Mod 针对哪个版本的 AssetRedux 插件进行开发。
    /// </summary>
    public const string TargetVersion = MyPluginInfo.PluginVersion;
}