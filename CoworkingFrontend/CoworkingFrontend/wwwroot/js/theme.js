window.cwTheme = {
  getTheme: function () {
    try {
      return localStorage.getItem('cw_theme');
    } catch (e) { return null; }
  },
  setTheme: function (theme) {
    try {
      if (theme === 'dark') {
        document.documentElement.classList.add('dark-theme');
        localStorage.setItem('cw_theme', 'dark');
      } else {
        document.documentElement.classList.remove('dark-theme');
        localStorage.setItem('cw_theme', 'light');
      }
      return theme;
    } catch (e) { return null; }
  },
  applyStoredTheme: function () {
    try {
      var t = localStorage.getItem('cw_theme');
      if (t === 'dark') document.documentElement.classList.add('dark-theme');
      else document.documentElement.classList.remove('dark-theme');
      return t || 'light';
    } catch (e) { return null; }
  }
};