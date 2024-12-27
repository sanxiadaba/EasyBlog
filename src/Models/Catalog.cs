namespace Models;


/// <summary>
/// 分类
/// </summary>
public class Catalog
{
    /// <summary>
    /// 名称
    /// </summary>
    public required string Name { get; set; }

    public ICollection<Catalog> Children { get; set; } = [];

    public ICollection<Doc> Docs { get; set; } = [];

    public Catalog? Parent { get; set; }

    public List<Doc> GetAllDocs()
    {
        var docs = new List<Doc>();

        docs.AddRange(Docs);
        foreach (var catalog in Children)
        {
            docs.AddRange(catalog.GetAllDocs());
        }
        return docs;
    }
}