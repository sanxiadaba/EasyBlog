namespace Models;
/// <summary>
/// 文档数据
/// </summary>
public class DocData
{
    /// <summary>
    /// 文档名称
    /// </summary>
    public required string Name { get; set; }

    public string Language { get; set; } = "en-us";

    public List<DocsVersion> DocVersions { get; set; } = [];
}

/// <summary>
/// 文档元素
/// </summary>
public class DocsVersion
{
    public required string Version { get; set; }
    public List<Catalog> Catalogs { get; set; } = [];
}
