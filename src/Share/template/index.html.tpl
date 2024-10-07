<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>@{Name}</title>
    <meta name="description" content="@{Description}" />
    <base href="/" />
    <link rel="stylesheet" href="@{BaseUrl}css/app.css" />
    <link rel="stylesheet" href="@{BaseUrl}css/markdown.css" />
    <link rel="icon" type="image/png" href="@{BaseUrl}favicon.ico" />
    <script>const baseUrl = '@{BaseUrl}';</script>
    <script src="@{BaseUrl}js/index.js"></script>
</head>

<body class="dark:bg-neutral-900">
    <div class="text-white py-2 bg-gray-600 dark:bg-neutral-800">
    <div class="container mx-auto flex items-center space-x-4">
      <div class="flex-none">
        <a href="/" class="text-2xl font-semibold hidden sm:block">EasyBlog</a>
      </div>
      <div class="flex-grow text-left flex space-x-4">
         @{navigations}
      </div>
      <div class="flex-none flex items-center">
      </div>
    </div>
    </div>

    <div class="container mx-auto mt-2" id="index-content">

    </div>
    </div>

    <div class="dark:bg-neutral-800 py-4 fixed bottom-0 w-full bg-gray-200">
    <div class="container mx-auto text-center">
        <p class="text-neutral-600 dark:text-neutral-300">
        @{Name}
        <a class="text-blue-600" target="_blank" href="https://github.com/AterDev/EasyBlog">Powered by Ater Blog</a>
        </p>
    </div>
    </div>
</body>
</html>