namespace Models;
/// <summary>
/// 文档信息模型
/// </summary>
public class DocElement
{
    public required string Name { get; set; }
    public bool IsHide { get; set; }
    public ICollection<DocElement>? Children { get; set; }
}
