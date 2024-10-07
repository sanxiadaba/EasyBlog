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
    <script src="@{BaseUrl}js/docs.js"></script>
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
        <input id="searchText" placeholder="搜索文档"
          class="px-4 py-2 border border-gray-600 rounded-lg dark:bg-neutral-800 text-black dark:text-white focus:outline-none" />
        <button id="searchBtn" class="ml-2 bg-blue-500 hover:bg-blue-600 text-white px-4 py-2 rounded-lg">
          搜索
        </button>
      </div>
    </div>
    </div>

    <div class="container mx-auto mt-2">
    <div class="flex">
        <div class="w-1/4 mt-1 hidden sm:block">
            @{docToc}
        </div>
        <div class="sm:w-3/4 sm:pr-4 w-full">
            <iframe frameborder="0" id="myIframe" width="100%" height="1000px"></iframe>
            <script>
              var iframe = document.getElementById('myIframe');
              iframe.onload = function () {
                iframe.style.height = iframe.contentWindow.document.body.scrollHeight + 20 + 'px';
              };
            </script>
        </div>
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