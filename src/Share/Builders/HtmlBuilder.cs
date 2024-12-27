using System.IO.Compression;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Text.Unicode;
using Markdig;
using Share.Builders;
using Share.MarkdownExtension;

namespace Share;

public class HtmlBuilder : BaseBuilder
{
    public string ContentPath { get; init; }
    public string Output { get; init; }
    public string DataPath { get; init; }

    /// <summary>
    /// 博客列表
    /// </summary>
    public List<Doc> Blogs { get; set; } = [];
    /// <summary>
    /// 文档列表
    /// </summary>
    public List<Doc> Docs { get; set; } = [];

    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
        Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
        WriteIndented = true
    };

    public HtmlBuilder(string input, string output, WebInfo webinfo) : base(webinfo)
    {
        Output = output;
        ContentPath = input.EndsWith(Path.DirectorySeparatorChar) ? input[0..^1] : input;
        DataPath = Path.Combine(Output, BlogConst.DataPath);
        WebInfo = webinfo;
    }

    public void BuildWebSite()
    {
        EnableBaseUrl();
        if (ExtractWebAssets())
        {
            BuildData();
            BuildHtmls("blogs");
            BuildHtmls("docs");
            BuildIndexHtml();
            BuildAboutMe();
            BuildBlogHtml();
            BuildDocHtml();
        }
        else
        {
            Command.LogError("缺少基础模板文件!");
        }
    }

    /// <summary>
    /// 解压基础资源
    /// </summary>
    public bool ExtractWebAssets()
    {
#if DEBUG
        return true;
#endif
        var stream = TemplateHelper.GetZipFileStream("web.zip");
        if (stream == null)
        {
            return false;
        }
        using (ZipArchive archive = new(stream, ZipArchiveMode.Read))
        {
            archive.ExtractToDirectory(Output, true);
        }
        return true;
    }

    /// <summary>
    ///  html file
    /// </summary>
    private void BuildHtmls(string dirName)
    {
        var dirPath = Path.Combine(ContentPath, dirName);
        Command.LogInfo($"search files in {dirPath}");
        // 配置markdown管道
        MarkdownPipeline pipeline = new MarkdownPipelineBuilder()
            .UseAlertBlocks()
            .UseFigures()
            .UseCitations()
            .UseFigures()
            .UseEmphasisExtras()
            .UseMathematics()
            .UseMediaLinks()
            .UseListExtras()
            .UseTaskLists()
            .UseDiagrams()
            .UseAutoLinks()
            .UseAutoIdentifiers(Markdig.Extensions.AutoIdentifiers.AutoIdentifierOptions.GitHub)
            .UsePipeTables()
            .UseBetterCodeBlock()
            .Build();

        // 如果是文件存在
        if (File.Exists(dirPath))
        {
            string markdown = File.ReadAllText(dirPath);
            string html = Markdown.ToHtml(markdown, pipeline);
            string relativePath = dirPath.Replace(dirPath, Path.Combine(Output, dirName)).Replace(".md", ".html");

            var title = GetTitleFromMarkdown(markdown);
            var toc = GetContentTOC(markdown) ?? "";
            html = AddHtmlTags(html, title, toc);
            string? dir = Path.GetDirectoryName(relativePath);

            File.WriteAllText(relativePath, html, Encoding.UTF8);
            Command.LogSuccess($"generate html:{relativePath}");
            return;
        }

        if (Directory.Exists(dirPath))
        {
            // 读取所有要处理的md文件
            List<string> files = Directory.EnumerateFiles(dirPath, "*.md", SearchOption.AllDirectories)
                .ToList();
            // 复制其他非md文件
            List<string> otherFiles = Directory.EnumerateFiles(dirPath, "*", SearchOption.AllDirectories)
                .Where(f => !f.EndsWith(".md"))
                .ToList();

            foreach (var file in files)
            {
                try
                {
                    string markdown = File.ReadAllText(file);
                    string html = Markdown.ToHtml(markdown, pipeline);
                    string relativePath = file.Replace(dirPath, Path.Combine(Output, dirName)).Replace(".md", ".html");

                    var title = GetTitleFromMarkdown(markdown);
                    var toc = GetContentTOC(markdown) ?? "";
                    html = AddHtmlTags(html, title, toc);
                    string? dir = Path.GetDirectoryName(relativePath);

                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir!);
                    }

                    File.WriteAllText(relativePath, html, Encoding.UTF8);
                    Command.LogSuccess($"generate html:{relativePath}");
                }
                catch (Exception e)
                {
                    Command.LogError($"parse markdown error: {file}" + e.Message + e.StackTrace);
                }
            }
            Command.LogSuccess("generate  html!");
            string[] extensions = [".jpg", ".png", ".jpeg", ".gif", ".svg"];
            foreach (var file in otherFiles)
            {
                var extension = Path.GetExtension(file);
                if (!extensions.Contains(extension)) { continue; }

                string relativePath = file.Replace(ContentPath, Path.Combine(Output, dirName));
                string? dir = Path.GetDirectoryName(relativePath);

                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir!);
                }

                File.Copy(file, relativePath, true);
            }
            Command.LogSuccess("copy other files!");
        }
    }

    /// <summary>
    /// json 数据文件
    /// </summary>
    public void BuildData()
    {
        if (!Directory.Exists(DataPath))
        {
            Directory.CreateDirectory(DataPath);
        }
        var webInfoContent = JsonSerializer.Serialize(WebInfo, _jsonSerializerOptions);
        File.WriteAllText(Path.Combine(DataPath, Command.WebConfigFileName), webInfoContent, Encoding.UTF8);

        // 获取git历史信息
        ProcessHelper.RunCommand("git", "fetch --unshallow", out string _);
        BuildBlogs();
        BuildDocs();
    }

    public void BuildBlogs()
    {
        // create blogs.json
        var rootCatalog = new Catalog { Name = "Root" };
        TraverseDirectory(Path.Combine(ContentPath, "blogs"), rootCatalog);
        Blogs = rootCatalog.GetAllDocs();
        string json = JsonSerializer.Serialize(rootCatalog, _jsonSerializerOptions);

        string blogDataPath = Path.Combine(DataPath, "blogs.json");
        File.WriteAllText(blogDataPath, json, Encoding.UTF8);
        Command.LogSuccess("update blogs.json!");
        // create sitemap.xml
        var blogs = rootCatalog.GetAllDocs();
        BuildSitemap(blogs);
    }

    public void BuildDocs()
    {
        var docInfos = WebInfo.DocInfos;
        var docRootPath = Path.Combine(ContentPath, "docs");

        foreach (var docInfo in docInfos)
        {
            var docPath = Path.Combine(docRootPath, docInfo.Name);
            if (!Directory.Exists(docPath))
            {
                Command.LogWarning($"{docPath} not exist! skip it.");
            }
            // 匹配语言
            var languageDirs = Directory.GetDirectories(docPath).Select(d => Path.GetFileName(d));
            var showLanguages = docInfo.Languages;
            var matchLanguages = languageDirs.Where(d => showLanguages.Contains(Path.GetFileName(d))).ToList();
            foreach (var language in matchLanguages)
            {
                var languagePath = Path.Combine(docPath, language);
                // 匹配版本
                var versionDirs = Directory.GetDirectories(languagePath).Select(d => Path.GetFileName(d));
                var showVersions = docInfo.Versions;
                var matchVersions = versionDirs.Where(d => showVersions.Contains(Path.GetFileName(d))).ToList();

                // 以{docInfo.Name}/{language}-{version}.json 生成对应语言版本的内容
                foreach (var version in matchVersions)
                {
                    var versionPath = Path.Combine(languagePath, version);
                    var versionCatalog = new Catalog { Name = $"{docInfo.Name}" };
                    TraverseDirectory(versionPath, versionCatalog);
                    string json = JsonSerializer.Serialize(versionCatalog, _jsonSerializerOptions);
                    string versionDataPath = Path.Combine(DataPath, docInfo.Name);
                    if (!Directory.Exists(versionDataPath))
                    {
                        Directory.CreateDirectory(versionDataPath);
                    }
                    var docFilePath = Path.Combine(versionDataPath, $"{language}-{version}.json");
                    File.WriteAllText(docFilePath, json, Encoding.UTF8);
                    Command.LogSuccess($"update {docInfo.Name}-{language}-{version}.json!");
                }
            }
        }
    }

    /// <summary>
    /// 生成aboutme
    /// </summary>
    public void BuildAboutMe()
    {
        BuildHtmls("about.md");
        Command.LogSuccess("update about.html");

    }

    /// <summary>
    /// 构建 index.html
    /// </summary>
    public void BuildIndexHtml()
    {
        var indexPath = Path.Combine(Output, "index.html");
        var indexHtml = TemplateHelper.GetTplContent("index.html");
        var blogData = Path.Combine(DataPath, "blogs.json");
        var blogContent = File.ReadAllText(blogData);
        var rootCatalog = JsonSerializer.Deserialize<Catalog>(blogContent);
        if (rootCatalog != null && WebInfo != null)
        {
            var navigations = BuildNavigations();
            var blogHtml = GenBlogListHtml(rootCatalog, WebInfo);
            // TODO: 生成最新的博客列表以及 文档列表(如果有)
            var latestBlogs = Blogs.Take(4).ToList();
            var blogSb = new StringBuilder();
            if (latestBlogs.Count > 0)
            {
                blogSb.AppendLine("""
                    <div class="text-lg my-2 font-medium">
                      最新博客
                    </div>
                    <div class="mt-2 flex gap-4 flex-wrap">
                    """);
                foreach (var blog in latestBlogs)
                {
                    var date = blog.UpdatedTime.Value.ToLocalTime().ToString("yyyy-MM-dd HH:mm");
                    blogSb.AppendLine($"""
                    <div class="blog-card">
                      <p class="title">{blog.Title}</p>
                      <p class="sub-title">👨‍💻 {WebInfo.AuthorName} &nbsp;&nbsp;📅 {date}</p>
                    </div>
                    """);
                }
                blogSb.Append("</div>");
            }


            indexHtml = indexHtml.Replace("@{Name}", WebInfo.Name)
                .Replace("@{navigations}", navigations)
                .Replace("@{blogs}", blogSb.ToString())
                .Replace("@{BaseUrl}", BaseUrl);

            File.WriteAllText(indexPath, indexHtml, Encoding.UTF8);
            Command.LogSuccess("update index.html");
        }
    }

    /// <summary>
    /// 构建blogs.html
    /// </summary>
    public void BuildBlogHtml()
    {
        var indexPath = Path.Combine(Output, "blogs.html");
        var indexHtml = TemplateHelper.GetTplContent("blogs.html");
        var blogData = Path.Combine(DataPath, "blogs.json");
        var blogContent = File.ReadAllText(blogData);
        var rootCatalog = JsonSerializer.Deserialize<Catalog>(blogContent);
        if (rootCatalog != null && WebInfo != null)
        {
            var navigations = BuildNavigations();
            var blogHtml = GenBlogListHtml(rootCatalog, WebInfo);
            var siderBarHtml = GenSiderBar(rootCatalog);

            indexHtml = indexHtml.Replace("@{Name}", WebInfo.Name)
                .Replace("@{navigations}", navigations)
                .Replace("@{BaseUrl}", BaseUrl)
                .Replace("@{blogList}", blogHtml)
                .Replace("@{siderbar}", siderBarHtml);

            File.WriteAllText(indexPath, indexHtml, Encoding.UTF8);
            Command.LogSuccess("update blogs.html");
        }
    }

    /// <summary>
    /// TODO:构建docs.html
    /// </summary>
    public void BuildDocHtml()
    {
        var indexPath = Path.Combine(Output, "docs.html");
        var indexHtml = TemplateHelper.GetTplContent("docs.html");

        var blogData = Path.Combine(DataPath, "blogs.json");

        var blogContent = File.ReadAllText(blogData);
        var rootCatalog = JsonSerializer.Deserialize<Catalog>(blogContent);
        if (rootCatalog != null && WebInfo != null)
        {
            var navigations = BuildNavigations();
            var docTocs = GenBlogListHtml(rootCatalog, WebInfo);

            // TODO:生成文档列表
            indexHtml = indexHtml.Replace("@{Name}", WebInfo.Name)
                .Replace("@{navigations}", navigations)
                .Replace("@{BaseUrl}", BaseUrl);

            File.WriteAllText(indexPath, indexHtml, Encoding.UTF8);
            Command.LogSuccess("update docs.html");
        }
    }

    /// <summary>
    /// 菜单导航
    /// </summary>
    /// <returns></returns>
    private string BuildNavigations()
    {
        var hasDocs = WebInfo.DocInfos.Count > 0;
        var hasBlog = Directory.Exists(Path.Combine(ContentPath, "blogs"));
        var hasAbout = File.Exists(Path.Combine(ContentPath, "about.md"));
        var navigations = new StringBuilder();
        if (hasBlog)
        {
            navigations.AppendLine(@"<a href=""/blogs.html"" class=""block py-2 text text-lg"">Blogs</a>");
        }
        if (hasDocs)
        {
            var docLinkHtml = "";
            foreach (var docName in WebInfo.DocInfos.Select(d => d.Name))
            {
                docLinkHtml += $@"<a href=""/docs/{docName}.html"" class=""block px-4 py-2 text"">{docName}</a>" + Environment.NewLine;
            }
            var docsMenuHtml = $$"""
                <div class="relative dropdown">
                  <div>
                    <button type="button" class="flex items-center gap-x-1 text text-lg">
                      Docs
                      <svg class="-mr-1 h-5 w-5 text-gray-400" viewBox="0 0 20 20" fill="currentColor" aria-hidden="true"
                        data-slot="icon">
                        <path fill-rule="evenodd"
                          d="M5.22 8.22a.75.75 0 0 1 1.06 0L10 11.94l3.72-3.72a.75.75 0 1 1 1.06 1.06l-4.25 4.25a.75.75 0 0 1-1.06 0L5.22 9.28a.75.75 0 0 1 0-1.06Z"
                          clip-rule="evenodd" />
                      </svg>
                    </button>
                  </div>
                  <div class="absolute z-10 mt-2 w-56 origin-top-right rounded-md bg-card dropdown-content hidden" tabindex="-1">
                    <div class="py-1" role="none">
                      {{docLinkHtml}}
                    </div>
                  </div>
                </div>
                """;
            navigations.AppendLine(docsMenuHtml);
        }
        if (hasAbout)
        {
            navigations.AppendLine("<a href=\"/about.html\" target=\"_blank\" class=\"block py-2 text text-lg \">About</a>");
        }
        return navigations.ToString();
    }

    private static string GetFullPath(Catalog catalog)
    {
        var path = Uri.EscapeDataString(catalog.Name);
        if (catalog.Parent != null)
        {
            path = GetFullPath(catalog.Parent) + "/" + path;
        }
        return path.Replace("Root", "");
    }

    public void EnableBaseUrl()
    {
        BaseUrl = WebInfo?.BaseHref ?? "/";
    }

    /// <summary>
    /// 创建sitemap.xml
    /// </summary>
    private void BuildSitemap(List<Doc> blogs)
    {
        if (!string.IsNullOrWhiteSpace(WebInfo.Domain) && blogs.Count > 0)
        {
            var sitemaps = new List<Sitemap>();
            var domain = WebInfo.Domain.EndsWith('/') ? WebInfo.Domain[..^1] : WebInfo.Domain;
            foreach (var blog in blogs)
            {
                var sitemap = new Sitemap
                {
                    Loc = domain + BuildBlogPath(blog.Path),
                    Lastmod = blog.PublishTime.ToString("yyyy-MM-dd")
                };
                sitemaps.Add(sitemap);
            }

            var sitemapXml = Sitemap.GetSitemaps(sitemaps);
            var sitemapPath = Path.Combine(Output, "sitemap.xml");
            File.WriteAllText(sitemapPath, sitemapXml, Encoding.UTF8);
            Command.LogSuccess("update sitemap.xml");
        }
    }

    /// <summary>
    /// blog list html
    /// </summary>
    /// <returns></returns>
    private string GenBlogListHtml(Catalog rootCatalog, WebInfo webInfo)
    {
        var sb = new StringBuilder();
        if (rootCatalog == null)
        {
            return string.Empty;
        }

        var blogs = rootCatalog.GetAllDocs().OrderByDescending(b => b.PublishTime).ToList() ?? [];

        foreach (var blog in blogs)
        {
            var html = $"""
                   <div class="w-100 rounded overflow-hidden shadow-lg dark:bg-neutral-800 my-2">
                       <div class="px-6 py-3">
                           <div class="font-bold text-xl mb-2">
                               <a href = "{BuildBlogPath(blog.Path)}" target="_blank" class="block text-lg py-2 text-neutral-600 hover:text-neutral-800 dark:text-neutral-300 dark:hover:text-neutral-100">📑 {blog.Title}</a>
                           </div>
                           <p class="text-neutral-700 text-base dark:text-neutral-300">
                               👨‍💻 {webInfo?.AuthorName}
                               &nbsp;&nbsp;
                               ⏱️ <span class="publish-time" data-time="{blog.PublishTime:yyyy-MM-ddTHH:mm:sszzz}"></span> 
                           </p>
                       </div>
                   </div>
                   """;
            sb.AppendLine(html);
        }
        return sb.ToString();
    }

    /// <summary>
    /// catalog and date
    /// </summary>
    /// <returns></returns>
    private string GenSiderBar(Catalog data)
    {
        var sb = new StringBuilder();
        var catalogs = data?.Children.ToList() ?? [];
        var allBlogs = data?.GetAllDocs().OrderByDescending(b => b.PublishTime).ToList() ?? [];
        var dates = allBlogs!.Select(b => b.PublishTime)
            .OrderByDescending(b => b)
            .DistinctBy(b => b.ToString("yyyy-MM"))
            .ToList();

        sb.AppendLine("<div id=\"catalog-list\" class=\"rounded-lg shadow-md p-4 dark:bg-neutral-800\">");
        sb.AppendLine("<div class=\"text-xl font-semibold dark:text-neutral-300\">分类</div>");
        sb.AppendLine($"""
            <span data-catalog="all" class="filter-item text-lg block py-2 text-neutral-600 hover:text-neutral-800 dark:text-neutral-300 dark:hover:text-neutral-100">
                全部 [{allBlogs.Count}]
            </span>
            """);
        foreach (var catalog in catalogs)
        {
            var html = $"""
                <span data-catalog="{catalog.Name}" class="filter-item text-lg block py-2 text-neutral-600 hover:text-neutral-800 dark:text-neutral-300 dark:hover:text-neutral-100">
                    {catalog.Name} [{catalog.Docs.Count}]
                </span>
                """;

            sb.AppendLine(html);
        }
        sb.AppendLine("</div>");

        sb.AppendLine("<div id=\"date-list\" class=\"rounded-lg shadow-md p-4 dark:bg-neutral-800 mt-2\">");
        sb.AppendLine("<div class=\"text-xl font-semibold dark:text-neutral-300\">存档</div>");
        sb.AppendLine($"""
            <span data-date="all" class="filter-item text-lg block py-2 text-neutral-600 hover:text-neutral-800 dark:text-neutral-300 dark:hover:text-neutral-100">
                全部 [{allBlogs.Count}]
            </span>
            """);
        foreach (var date in dates)
        {
            var count = allBlogs.Count(b => b.PublishTime.Year == date.Year && b.PublishTime.Month == date.Month);
            var html = $"""
                <span data-date="{date:yyyy-MM}" class="filter-item text-lg block py-2 text-neutral-600 hover:text-neutral-800 dark:text-neutral-300 dark:hover:text-neutral-100">
                    {date:yyyy-MM} [{count}]
                </span>
                """;
            sb.AppendLine(html);
        }
        sb.AppendLine("</div>");

        return sb.ToString();
    }

    private string BuildBlogPath(string path)
    {
        return BaseUrl + "blogs" + path;
    }
}
