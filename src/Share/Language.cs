using System.Globalization;

namespace Share;
public class Language
{
    public static Dictionary<string, string> CN { get; set; } = new Dictionary<string, string>
    {
        {"Command","命令" },
        {"init",$"初始化配置文件{Command.WebConfigFileName};[path]文件路径."},
        {"build","生成静态网站;[configPath] 为配置文件路径"},
        {"doc","生成文档类静态网站;[configPath]为配置文件(json)"},
        {"buildRequired","参数[configPath]是必需的." },
        {"initSuccess",$"初始化配置文件[{Command.WebConfigFileName}] 成功!"},
        {"notExistWebInfo",$"未找到配置文件,将使用默认配置." }
    };
    public static Dictionary<string, string> EN { get; set; } = new Dictionary<string, string>
    {
        {"Command","Commands" },
        {"init",$"init config file {Command.WebConfigFileName};[path] is path."},
        {"build","generate static website;[configPah] is config file path"},
        {"doc","generate doc site;[configPath] is json config file "},
        {"buildRequired","params [configPath] is Required!" },
        {"initSuccess",$"Init config file:[{Command.WebConfigFileName}] success!"},
        {"notExistWebInfo",$"config file not found, will use default config!" }
    };

    public static string Get(string key)
    {
        var isCn = CultureInfo.CurrentCulture.Name == "zh-CN";

        return isCn ? CN[key] : EN[key];
    }
}
