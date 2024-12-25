using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Share.Builders;
/// <summary>
/// 内容构建
/// </summary>
public class DocsBuilder(WebInfo webInfo) : BaseBuilder(webInfo)
{
    private readonly StringBuilder _stringBuilder = new();

    public Guid Id { get; set; }

    /// <summary>
    /// 构建文档主页
    /// </summary>
    /// <returns></returns>
    public string BuildIndex()
    {
        return "Hello World";
    }

    public List<string> BuildDocs()
    {
        foreach (var docInfo in WebInfo.DocInfos)
        {

        }

        return [];
    }

    /// <summary>
    /// 侧边导航列表
    /// </summary>
    /// <returns></returns>
    public string BuildNavigations()
    {
        foreach (var docInfo in WebInfo.DocInfos)
        {

        }

        return "";
    }

}
