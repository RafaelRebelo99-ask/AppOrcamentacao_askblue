const sortBtn = document.getElementById("sortBtn");
const sortMenu = document.getElementById("sortMenu");
const arrowIcon = sortBtn.querySelector("i");

sortMenu.classList.add("hidden");

sortBtn.addEventListener("click", () => {
    sortMenu.classList.toggle("hidden");
    arrowIcon.classList.toggle("rot-180");
});

const themeBtn = document.getElementById("themeBtn");
const icon = themeBtn.querySelector("i");
const logo = document.getElementById("logo")

themeBtn.addEventListener("click", () => {
    const root = document.documentElement;
    root.classList.add("disable-transitions");

    const isDark = root.classList.toggle("dark");

    logo.src = isDark ? 'askblue-logo-negativo-rgb.png' : 'askblue-logo-oficial-rgb.png';

    if (icon) {
        icon.classList.toggle("bi-sun", isDark);
        icon.classList.toggle("bi-moon", !isDark);
    }

    void root.offsetHeight;

    requestAnimationFrame(() => {
        root.classList.remove("disable-transitions");
    });
});