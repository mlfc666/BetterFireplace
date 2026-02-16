namespace BetterFireplace.Models
{
    public abstract class BaseResourceModule
    {
        // 模块名称（用于日志区分）
        public virtual string ModuleName => "UnknownModule";

        // 数值越大，越早被注册 (越早抢占资源位)
        public virtual int Priority => 0;

        // 使用 virtual 允许子类重写整个字典
        // 主动替换：资源名 -> 文件路径
        public virtual Dictionary<string, string> Sprites => new();
        public virtual Dictionary<string, string> Textures => new();

        // 被动拦截：资源名 -> 处理函数 (输入原文本，返回新文本)
        public virtual Dictionary<string, Func<string, string>> TextAssetProcessors => new();
    }
}