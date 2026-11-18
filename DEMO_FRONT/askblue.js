const sortBtn = document.getElementById("sortBtn");
const sortMenu = document.getElementById("sortMenu");
const arrowIcon = sortBtn.querySelector("i");

sortMenu.classList.add("hidden");

sortBtn.addEventListener("click", () => {
    sortMenu.classList.toggle("hidden");
    arrowIcon.classList.toggle("rot-180");
});