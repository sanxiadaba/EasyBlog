class Docs {
  docName = '';
  language = 'en-US';
  version = '1.0';

  constructor() {
    document.addEventListener('DOMContentLoaded', () => this.init());
  }

  init() {
    const self = this;
    const docData = document.getElementById('docData');
    if (docData) {
      this.docName = docData.getAttribute('data-docName');
      this.language = docData.getAttribute('data-language');
      this.version = docData.getAttribute('data-version');
    }

    // set selected value for version select
    const versionSelect = document.getElementById('versionSelect');
    if (versionSelect) {
      const options = versionSelect.querySelectorAll('option');
      options.forEach(option => {
        if (option.value === this.version) {
          option.selected = true;
        }
      });
    }

    document.getElementById('languageSelect')?.addEventListener('change', function () {
      const selectedValue = this.value;
      if (selectedValue) {
        window.location.href = selectedValue;
      }
    });

    document.getElementById('versionSelect')?.addEventListener('change', function () {
      const selectedValue = this.value;
      if (selectedValue != this.version) {
        self.redirectToVersion(selectedValue);
      }
    });

    var toggler = document.getElementsByClassName("caret");
    for (var i = 0; i < toggler.length; i++) {
      toggler[i].addEventListener("click", function () {
        this.parentElement.querySelector(".nested").classList.toggle("active");
        this.classList.toggle("caret-down");
      });
    }
  }

  redirectToVersion(version) {
    var url = new URL(window.location.href);
    var path = url.pathname;
    var htmlName = path.split('/').pop();
    url.pathname = `/docs/${this.docName}/${this.language}/${version}/${htmlName}`;
    window.location.href = url.href;
  }

}
new Docs();


// interface UrlInfo {
//   url: string;
//   title: string;
// }
