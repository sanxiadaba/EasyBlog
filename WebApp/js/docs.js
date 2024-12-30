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
}
new Docs();
