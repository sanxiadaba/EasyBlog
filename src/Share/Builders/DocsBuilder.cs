using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColorCode.Core.Compilation.Languages;
using Markdig;
using Share.MarkdownExtension;

namespace Share.Builders;
/// <summary>
/// 内容构建
/// </summary>
public class DocsBuilder(WebInfo webInfo, string input, string output) : BaseBuilder(webInfo)
{
    public List<DocInfo> DocInfos { get; set; } = webInfo.DocInfos;
    public string ContentPath { get; init; } = input.EndsWith(Path.DirectorySeparatorChar) ? input[0..^1] : input;

    public string Output { get; init; } = output;

    public Guid Id { get; set; }

    /// <summary>
    /// 构建文档
    /// </summary>
    /// <returns></returns>
    public void BuildDocs()
    {
        if (DocInfos == null || DocInfos.Count == 0)
        {
            return;
        }
        var docRootPath = Path.Combine(ContentPath, "docs");
        var outputDocPath = Path.Combine(Output, "docs");

        var docsCatalog = new Catalog { Name = "Root", Path = docRootPath };
        TraverseDirectory(docRootPath, docsCatalog);

        var tplContent = TemplateHelper.GetTplContent("docs.html");
        var navMenuTmp = BuildNavigations(ContentPath);
        tplContent = tplContent.Replace("@{NavMenus}", navMenuTmp)
            .Replace("@{Name}", WebInfo.Name);

        foreach (var docInfo in DocInfos)
        {
            var docPath = Path.Combine(docRootPath, docInfo.Name);

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

                var versionSelect = BuildVersionSelect(matchVersions);

                foreach (var version in matchVersions)
                {
                    var versionPath = Path.Combine(languagePath, version);
                    var versionCatalog = docsCatalog.FindCatalog(versionPath);
                    var docTree = BuildTree(versionCatalog);

                    var docs = versionCatalog.GetAllDocs();

                    foreach (var doc in docs)
                    {
                        string markdown = File.ReadAllText(doc.Path);

                        var leftNav = versionSelect + docTree;
                        var docContent = BuildDocContent(doc);
                        var title = GetTitleFromMarkdown(markdown);
                        var toc = GetContentTOC(markdown) ?? "";
                        var extensionScript = GetExtensionScript(docContent);

                        var htmlContent = tplContent.Replace("@{BaseUrl}", BaseUrl)
                          .Replace("@{ExtensionHead}", extensionScript)
                          .Replace("@{Title}", title)
                          .Replace("@{LeftNav}", leftNav)
                          .Replace("@{TOC}", toc)
                          .Replace("@{DocContent}", docContent)
                          .Replace("@{DocName}", docInfo.Name)
                          .Replace("@{Language}", language)
                          .Replace("@{Version}", version);

                        var outputFilePath = Path.Combine(outputDocPath, doc.HtmlPath);

                        var dirPath = Path.GetDirectoryName(outputFilePath);
                        if (dirPath != null && !Directory.Exists(dirPath))
                        {
                            Directory.CreateDirectory(dirPath);
                        }
                        File.WriteAllText(outputFilePath, htmlContent);
                        Command.LogInfo($"generate {outputFilePath}");
                    }
                }
            }
        }
    }

    public string BuildDocContent(Doc doc)
    {
        var pipeline = new MarkdownPipelineBuilder()
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

        string markdown = File.ReadAllText(doc.Path);
        string html = Markdig.Markdown.ToHtml(markdown, pipeline);
        //string relativePath = dirPath.Replace(dirPath, Path.Combine(Output, dirName)).Replace(".md", ".html");
        return html;
    }



    /// <summary>
    /// 版本选择控件
    /// </summary>
    /// <param name="docInfo"></param>
    /// <returns></returns>
    public string BuildVersionSelect(List<string> versions)
    {
        var sb = new StringBuilder();
        // version select

        sb.Append("""
            <select id="versionSelect" class="border border-gray-300 rounded-md p-2 bg-block w-full">

            """);
        foreach (var version in versions)
        {
            sb.Append($"<option value='{version}'>{version}</option>");
        }

        sb.Append("</select>");
        return sb.ToString();
    }

    /// <summary>
    /// 树型导航控件
    /// </summary>
    /// <returns></returns>
    public string BuildTree(Catalog rootCatalog)
    {
        if (rootCatalog == null)
            throw new ArgumentNullException(nameof(rootCatalog));

        var sb = new StringBuilder();
        sb.Append("<ul>");
        GenerateCatalogHtml(rootCatalog, sb);
        sb.Append("</ul>");
        return sb.ToString();
    }
    private void GenerateCatalogHtml(Catalog catalog, StringBuilder sb)
    {
        sb.Append("<li>");
        sb.Append($"<span>{catalog.Name}</span>");

        // Generate documents list if present
        if (catalog.Docs != null && catalog.Docs.Count > 0)
        {
            sb.Append("<ul>");
            foreach (var doc in catalog.Docs)
            {
                sb.Append($"<li><span>{doc.FileName}</span></li>");
            }
            sb.Append("</ul>");
        }

        // Generate children if present
        if (catalog.Children != null && catalog.Children.Count > 0)
        {
            sb.Append("<ul>");
            foreach (var child in catalog.Children)
            {
                GenerateCatalogHtml(child, sb);
            }
            sb.Append("</ul>");
        }

        sb.Append("</li>");
    }
}
