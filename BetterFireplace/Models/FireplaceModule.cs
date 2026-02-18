using System.Text.RegularExpressions;
using AssetRedux.Models;

namespace BetterFireplace.Models;

public class FireplaceModule : BaseResourceModule
{
    public override string ModuleName => "壁炉范围增强模块";

    public override Dictionary<string, Func<string, string>> TextAssetProcessors => new()
    {
        { "build", ModifyBuildRange }
    };
    
    private string ModifyBuildRange(string originalJson)
    {
        // 逻辑说明：
        // 先匹配包含 "id": 104014 的整个 JSON 对象块
        // 在这个块内寻找 "effect_range": 216 并修改
        
        // 这个正则匹配包含 ID 104014 的对象
        // 使用 RegexOptions.Singleline 确保 . 匹配换行符
        string objectPattern = @"\{[^{}]*""id""\s*:\s*104014[^{}]*\}";

        return Regex.Replace(originalJson, objectPattern, blockMatch =>
        {
            string blockContent = blockMatch.Value;
            
            // 在 ID 104014 的块内查找 effect_range
            string rangePattern = @"(""effect_range""\s*:\s*)216(\b)";
            
            // 替换：216 * 10 = 2160
            return Regex.Replace(blockContent, rangePattern, rangeMatch =>
            {
                string prefix = rangeMatch.Groups[1].Value;
                string boundary = rangeMatch.Groups[2].Value;
                return $"{prefix}2160{boundary}";
            });
        }, RegexOptions.Singleline);
    }
}