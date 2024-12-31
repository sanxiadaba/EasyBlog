using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models;
/// <summary>
/// 生成文件
/// </summary>
public class GenFile
{
    public required string Name { get; set; }
    public required string Path { get; set; }

    public string? Content { get; set; }
}
