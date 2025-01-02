<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>@{Title}</title>
    <meta name="description" content="@{Description}" />
    <base href="/" />
    <link rel="stylesheet" href="@{BaseUrl}css/app.css" />
    <link rel="stylesheet" href="@{BaseUrl}css/markdown.css" />
    <link rel="icon" type="image/png" href="@{BaseUrl}favicon.ico" />
    <script>const baseUrl = '@{BaseUrl}';</script>
    <script src="@{BaseUrl}js/docs.js"></script>
    <script src="@{BaseUrl}js/markdown.js"></script>
    @{ExtensionHead}
  <style>
   .dropdown:focus-within .dropdown-content {
        display: block;
    }

    .tree ul li {
      list-style-type: none;
      margin-left: 1.5rem;
    }

    .root-list>li {
      margin-left: 0 !important;
    }

    .tree .caret {
      cursor: pointer;
      user-select: none;
    }

    .tree .nested {
      display: none;
    }

    .tree .active {
      display: block;
    }

    .tree .caret::before {
      content: "▶️";
      display: inline-block;
      margin-right: 4px;
    }

    .tree .space::before {
      content: "\2003";
      display: inline-block;
      margin-right: 10px;
    }

    .tree .caret-down::before {
      transform: rotate(90deg);
    }
  </style>

</head>

<body class="dark:bg-neutral-900">
    <div class="text-white py-2 bg-block">
    <div class="container mx-auto flex items-center space-x-4">
      <div class="flex-none">
        <a href="/" class="text-2xl font-semibold hidden sm:block">@{Name}</a>
      </div>
      <div class="flex-grow text-left flex space-x-4 items-center">
         @{NavMenus}
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

<div class="container mx-auto">
  <div id="docData" class="hidden" data-docName="@{DocName}" data-language="@{Language}" data-version="@{Version}"></div>
  <div class="grid grid-cols-1 md:grid-cols-3 lg:grid-cols-4 gap-3 mt-2 pt-2">
    <div class="hidden md:block lg:col-span-1 sticky pe-4">
    <div>
      <strong>Version</strong>
    </div>
    @{LeftNav}
    </div>
    <div class="col-span-1 md:col-span-2 lg:col-span-2">
    @{DocContent}
    </div>
    <div class="hidden lg:block lg:col-span-1">
    @{TOC}
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