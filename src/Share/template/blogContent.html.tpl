<!DOCTYPE html>
<html>
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <link rel="stylesheet" href="@{BaseUrl}css/app.css">
    <link rel="stylesheet" href="@{BaseUrl}css/markdown.css">
    <link rel="icon" type="image/png" href="@{BaseUrl}favicon.ico" />
    <script src="@{BaseUrl}js/markdown.js"></script>
    <title>@{Title}</title>
    @{ExtensionHead}
</head>
<body class="px-4 sm:px-6 lg:px-8 dark:bg-neutral-900 pb-4">
    <div class="container mx-auto flex mt-2" style="margin-bottom: 48px;">
        <div class="sm:w-3/4 sm:pr-4 w-full markdown-content">
        @{content}
        </div>
        <div class="w-1/4 mt-1 hidden sm:flex">
            @{toc}
        </div>
    </div>
    <div class="footer py-2 bottom-0 w-full fixed">
      <div class="container mx-auto text-center">
        <p class="text-neutral-600 dark:text-neutral-300 mb-0">
        @{Name}
        <a class="text-blue-600" target="_blank" href="https://github.com/AterDev/EasyBlog">Powered by Ater Blog</a>
        </p>
      </div>
    </div>
</body>
</html>