using System.Text;
using ToolGood.Words;

namespace Utility.Helper
{
    /// <summary>
    /// 非法（敏感）词帮助类
    /// </summary>
    public class IllegalWordHelper
    {
        private static readonly string pathBase = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
        // 敏感词库
        private const string KeywordsPath = "_Illegal/IllegalKeywords.txt";
        // 敏感url
        private const string UrlsPath = "_Illegal/IllegalUrls.txt";

        // 更多敏感词汇
        // https://github.com/fighting41love/funNLP/tree/master/data/%E6%95%8F%E6%84%9F%E8%AF%8D%E5%BA%93

        // 保存文件信息（最后更新时间），根据时间比对是否重新加载词库
        private const string InfoPath = "_Illegal/IllegalInfo.txt";
        // 词库缓存文件
        private const string BitPath = "_Illegal/IllegalBit.iws";

        // 过滤非法词（敏感词）专用类
        private static IllegalWordsSearch _search;

        public IllegalWordHelper()
        {
            if (!File.Exists(GetFullPath(UrlsPath)) || !File.Exists(GetFullPath(KeywordsPath)))
            {
                throw new ArgumentNullException(nameof(IllegalWordHelper), "未找到非法词库");
            }
            if (_search == null)
            {
                string ipath = GetFullPath(InfoPath);
                if (!File.Exists(ipath))
                {
                    _search = CreateIllegalWordsSearch();
                }
                else
                {
                    var texts = File.ReadAllText(ipath).Split('|');
                    if (GetLastWriteTime(KeywordsPath) != texts[0] || GetLastWriteTime(UrlsPath) != texts[1])
                    {
                        _search = CreateIllegalWordsSearch();
                    }
                    else
                    {
                        var s = new IllegalWordsSearch();
                        s.Load(GetFullPath(BitPath));
                        _search = s;
                    }
                }
            }
        }
        /// <summary>
        /// 获取已加载敏感词IllegalWordsSearch类（本地敏感库,文件修改后，重新创建缓存Bit）
        /// </summary>
        /// <returns></returns>
        public static IllegalWordsSearch GetIllegalWordsSearch()
        {
            if (_search == null)
            {
                string ipath = GetFullPath(InfoPath);
                if (!File.Exists(ipath))
                {
                    _search = CreateIllegalWordsSearch();
                }
                else
                {
                    var texts = File.ReadAllText(ipath).Split('|');
                    if (GetLastWriteTime(KeywordsPath) != texts[0] || GetLastWriteTime(UrlsPath) != texts[1])
                    {
                        _search = CreateIllegalWordsSearch();
                    }
                    else
                    {
                        var s = new IllegalWordsSearch();
                        s.Load(GetFullPath(BitPath));
                        _search = s;
                    }
                }
            }
            return _search;
        }
        /// <summary>
        /// 创建IllegalWordsSearch
        /// </summary>
        /// <returns></returns>
        private static IllegalWordsSearch CreateIllegalWordsSearch()
        {
            string[] words1 = File.ReadAllLines(GetFullPath(KeywordsPath), Encoding.UTF8);
            string[] words2 = File.ReadAllLines(GetFullPath(UrlsPath), Encoding.UTF8);
            var words = new List<string>();
            foreach (var item in words1)
            {
                words.Add(item.Trim());
            }
            foreach (var item in words2)
            {
                words.Add(item.Trim());
            }

            var search = new IllegalWordsSearch();
            search.SetKeywords(words);

            search.Save(GetFullPath(BitPath));

            var text = GetLastWriteTime(KeywordsPath) + "|" + GetLastWriteTime(UrlsPath);
            File.WriteAllText(GetFullPath(InfoPath), text);

            return search;
        }
        /// <summary>
        /// 获取文件最后更新时间
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static string GetLastWriteTime(string path)
        {
            return new FileInfo(GetFullPath(path)).LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss");
        }
        /// <summary>
        /// 获取文件全路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static string GetFullPath(string path)
        {
            return $"{pathBase}{path}";
        }
    }
}
