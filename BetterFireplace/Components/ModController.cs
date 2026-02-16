using System.Reflection;
using BetterFireplace.Models;
using BetterFireplace.Tools;
using Il2CppInterop.Runtime.Attributes;
using UnityEngine;

namespace BetterFireplace.Components
{
    public class ModController: MonoBehaviour
    {
        public ModController(IntPtr ptr) : base(ptr)
        {
        }

        public void Awake()
        {
            // 模块注册：解析路径、注册 TextAsset 流水线
            // 这一步只存路径字符串，速度极快
            AutoRegisterModules();
        }

        public void Start()
        {
            Plugin.Log.LogInfo($"ModController控制器启动成功！");
            CheckGameVersion();
        }

        [HideFromIl2Cpp]
        private void ProcessBatch<TValue>(Dictionary<string, TValue>? map, Action<string, TValue> action, string label)
        {
            if (map == null || map.Count == 0) return;
            foreach (var kvp in map)
            {
                try { action(kvp.Key, kvp.Value); }
                catch (Exception e) { Plugin.Log.LogError($"[{label}] 注册失败: {kvp.Key} -> {e.Message}"); }
            }
        }

        private void AutoRegisterModules()
        {
            Plugin.Log.LogInfo("开始注册所有模块资源...");
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                var moduleTypes = assembly.GetTypes().Where(t =>
                    typeof(BaseResourceModule).IsAssignableFrom(t) && t is { IsInterface: false, IsAbstract: false });

                List<BaseResourceModule> loadedModules = new();
                foreach (var type in moduleTypes)
                {
                    if (Activator.CreateInstance(type) is BaseResourceModule module)
                        loadedModules.Add(module);
                }

                // 排序：100 -> 0 (降序)
                loadedModules.Sort((a, b) => b.Priority.CompareTo(a.Priority));

                foreach (var module in loadedModules)
                {
                    // 这里的注册只记录路径字符串，不消耗 IO 和 内存
                    ProcessBatch(module.TextAssetProcessors, TextAssetManager.RegisterProcessor, "TextAsset");

                    Plugin.Log.LogInfo($"模块加载: {module.ModuleName} [Priority: {module.Priority}]");
                }
            }
            catch (Exception e)
            {
                Plugin.Log.LogError($"自动注册模块崩溃: {e.Message}");
            }
        }

        private void CheckGameVersion()
        {
            try
            {
                var versionAsset = Resources.Load<TextAsset>("version");
                if (versionAsset == null)
                {
                    Plugin.Log.LogWarning("[VersionCheck] 未能读取到 version 文件");
                    return;
                }
                string gameVersion = versionAsset.text.Trim();
                if (gameVersion != Plugin.TargetVersion)
                {
                    Plugin.Log.LogWarning($"[VersionCheck] 版本不匹配: 插件[{Plugin.TargetVersion}] vs 游戏[{gameVersion}]");
                }
                else
                {
                    Plugin.Log.LogInfo($"[VersionCheck] 游戏版本校验通过: {gameVersion}");
                }
            }
            catch (Exception e)
            {
                Plugin.Log.LogError($"[VersionCheck] 异常: {e.Message}");
            }
        }
    }
}