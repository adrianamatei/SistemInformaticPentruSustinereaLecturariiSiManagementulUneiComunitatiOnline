// Automatizare pentru animație continuă
document.addEventListener("DOMContentLoaded", () => {
    const carouselInner = document.querySelector(".carousel-inner");
    const items = document.querySelectorAll(".carousel-item");

    // Clonează primele câteva imagini pentru efectul continuu
    items.forEach(item => {
        const clone = item.cloneNode(true);
        carouselInner.appendChild(clone);
    });
});