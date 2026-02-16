namespace BetterFireplace.Tools
{
    public static class TextAssetManager
    {
        // 存储 List 以支持多个处理器流水线执行 (一个资源名 -> 多个处理函数)
        private static readonly Dictionary<string, List<Func<string, string>>> Processors = new();

        // 缓存字典：资源名 -> 最终处理完的内容
        private static readonly Dictionary<string, string> Cache = new();

        // 注册方法：追加模式
        public static void RegisterProcessor(string assetName, Func<string, string> processor)
        {
            if (!Processors.ContainsKey(assetName))
            {
                Processors[assetName] = new List<Func<string, string>>();
            }
            Processors[assetName].Add(processor);

            // 一旦注册了新的处理器，之前的缓存结果就“过期”了
            // 必须移除它，确保下一次读取时，能把这个新处理器的逻辑也跑一遍
            Cache.Remove(assetName);
        }

        // 获取结果：流水线执行
        public static bool TryGetModifiedContent(string assetName, string originalContent, out string? finalContent)
        {
            // 先查缓存，只要名字对，就认为内容对（忽略 originalContent 的比对，追求最高性能）
            if (Cache.TryGetValue(assetName, out var cachedResult))
            {
                finalContent = cachedResult;
                return true;
            }

            // 检查是否有处理器
            if (!Processors.TryGetValue(assetName, out var processorList) || processorList.Count == 0)
            {
                finalContent = null;
                return false;
            }

            string currentContent = originalContent;
            
            // 执行流水线 (A -> B -> C ...)
            foreach (var processFunc in processorList)
            {
                try
                {
                    // 将上一次的结果作为下一次的输入
                    string tempResult = processFunc(currentContent);
                    currentContent = tempResult;
                }
                catch (Exception)
                {
                    // 忽略单个处理器的错误，继续传递旧数据
                }
            }
            
            finalContent = currentContent;

            // 【将最终跑完流水线的结果存入缓存
            Cache[assetName] = finalContent;

            return true;
        }
    }
}