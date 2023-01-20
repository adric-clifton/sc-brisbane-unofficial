const btn = document.querySelector(".btn-toggle");
const prefersDarkScheme = window.matchMedia("(prefers-color-scheme: dark)");


// get theme preference from local storage
const currentTheme = localStorage.getItem("theme");
var d = document;
if (currentTheme == "dark") {
    document.firstChild.classList.toggle("dark-mode");
} else if (currentTheme == "light") {
    document.firstChild.classList.toggle("light-mode");
}

// listen for clicks
btn.addEventListener("click", function () {
    var doc = document;
    if (prefersDarkScheme.matches) {
        document.firstChild.classList.toggle("light-mode");
        var theme = document.firstChild.classList.contains("light-mode") ? "light" : "dark";
    } else {
        document.firstChild.classList.toggle("dark-mode");
        var theme = document.firstChild.classList.contains("dark-mode") ? "dark" : "light";
    }
    // save preference to localStorage
    localStorage.setItem("theme", theme);
});