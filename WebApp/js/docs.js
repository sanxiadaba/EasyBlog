class Docs {
  language = 'en-US';
  version = '1.0';

  constructor() {
    document.addEventListener('DOMContentLoaded', () => this.init());
  }
  init() {
    document.getElementById('languageSelect').addEventListener('change', function () {
      const selectedValue = this.value;
      if (selectedValue) {
        window.location.href = selectedValue;
      }
    });

    document.getElementById('versionSelect').addEventListener('change', function () {
      const selectedValue = this.value;
      if (selectedValue) {
        window.location.href = selectedValue;
      }
    });
  }
}
new Docs();
