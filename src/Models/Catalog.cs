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
        var blogs = new List<Doc>();

        blogs.AddRange(Docs);
        foreach (var catalog in Children)
        {
            blogs.AddRange(catalog.GetAllDocs());
        }
        return blogs;
    }
}