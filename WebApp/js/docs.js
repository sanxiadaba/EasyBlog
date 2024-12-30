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

    const toggles = document.querySelectorAll('[data-toggle]');
    toggles.forEach(toggle => {
      toggle.addEventListener('click', () => {
        const targetId = toggle.getAttribute('data-toggle');
        const targetElement = document.getElementById(targetId);
        if (targetElement) {
          targetElement.classList.toggle('hidden');
        }
      });
    });
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
