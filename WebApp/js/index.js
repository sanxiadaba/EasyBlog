const BaseUrl = baseUrl;
class Index {
    blogs = [];
    allBlogs = [];
    catalogs = [];
    webInfo;
    constructor() {
        document.addEventListener('DOMContentLoaded', () => this.init());
    }
    init() {
        this.getData();
        this.addEvent();
    }
    getData() {
        fetch(BaseUrl + 'data/blogs.json')
            .then(res => res.json()).then((data) => {
                this.allBlogs = this.getAllBlogs(data).sort((a, b) => {
                    return new Date(b.PublishTime).getTime() - new Date(a.PublishTime).getTime();
                });
                this.blogs = this.allBlogs.slice(0, 50);
                this.catalogs = data.Children;
            });
        fetch(BaseUrl + 'data/webinfo.json')
            .then(res => res.json()).then((data) => {
                this.webInfo = data;
            });
    }
    addEvent() {
        const self = this;
        // const menuButton = document.getElementById('menu-button');
        // const menu = document.getElementById('menu');

        // menuButton.addEventListener('click', function () {
        //     const isExpanded = menuButton.getAttribute('aria-expanded') === 'true';
        //     menuButton.setAttribute('aria-expanded', !isExpanded);
        //     menu.classList.toggle('hidden');
        // });

        // document.addEventListener('click', function (event) {
        //     if (!menuButton.contains(event.target) && !menu.contains(event.target)) {
        //         menuButton.setAttribute('aria-expanded', 'false');
        //         menu.classList.add('hidden');
        //     }
        // });
    }
    getAllBlogs(rootCatalog) {
        let blogs = [];
        blogs.push(...rootCatalog.Docs);
        if (rootCatalog.Children && rootCatalog.Children.length > 0) {
            rootCatalog.Children.forEach(catalog => {
                blogs.push(...this.getAllBlogs(catalog));
            });
        }
        return blogs;
    }
    timeAgo(date) {
        const seconds = Math.floor((new Date().getTime() - date.getTime()) / 1000);
        const intervals = {
            '年': 31536000,
            '月': 2592000,
            '天': 86400,
            '小时': 3600,
            '分钟': 60,
            '秒': 1
        };
        let counter;
        let values = [];
        for (const [unit, secondsPerUnit] of Object.entries(intervals)) {
            counter = Math.floor(seconds / secondsPerUnit);
            values.push([counter, unit]);
        }
        for (let i = 0; i < values.length; i++) {
            const [counter, unit] = values[i];
            if (counter > 0) {
                if (unit === '年') {
                    let month = Math.floor((seconds - counter * intervals[unit]) / intervals['月']);
                    let str = month > 0 ? month + '月' : '';
                    return `${counter}${unit}${str}前`;
                }
                if (unit === '月') {
                    let day = Math.floor((seconds - counter * intervals[unit]) / intervals['天']);
                    let str = day > 0 ? day + '天' : '';
                    return `${counter}${unit}${str}前`;
                }
                if (unit === '天') {
                    let hour = Math.floor((seconds - counter * intervals[unit]) / intervals['小时']);
                    let str = hour > 0 ? hour + '小时' : '';
                    return `${counter}${unit}${str}前`;
                }
                return `${counter}${unit}前`;
            }
        }
        return '刚刚';
    }
}
new Index();
