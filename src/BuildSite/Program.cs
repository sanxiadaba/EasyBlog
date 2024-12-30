using Share;

ShowLogo();
#if DEBUG
Test();
#endif

string? command = args.FirstOrDefault();

switch (command)
{
    case "init":
        var path = args.Skip(1).FirstOrDefault() ?? Directory.GetCurrentDirectory();
        Command.Init(path);
        break;
    case "build":
        var contentPath = args.Skip(1).FirstOrDefault();
        var outputPath = args.Skip(2).FirstOrDefault();

        if (!string.IsNullOrWhiteSpace(contentPath) && !string.IsNullOrWhiteSpace(outputPath))
        {
            Command.Build(contentPath, outputPath);
        }
        else
        {
            Command.LogError(Language.Get("buildRequired"));
        }
        break;
    default:
        ShowHelp();
        break;
}

static void ShowHelp()
{
    var helpContent = """
    {0}:
    init [path]
        {1}

    build [contentPath] [outputPath]
        {2}

    doc [configPath]
    """;
    Console.Write(helpContent,
        Language.Get("Command"),
        Language.Get("init"),
        Language.Get("build"),
        Language.Get("doc")
        );
}
static void ShowLogo()
{
    var logo = """
            EasyBlog : The Static Web Builder!
               —→ for freedom 🗽 ←—

            """;

    Console.WriteLine(logo);
}

static void Test()
{
    Console.WriteLine("Test");
    var contentPath = Path.Combine(Directory.GetCurrentDirectory(), "Content");
    var outputPath = Path.Combine(Directory.GetCurrentDirectory(), "WebApp");
    Command.Build(contentPath, outputPath);
}
