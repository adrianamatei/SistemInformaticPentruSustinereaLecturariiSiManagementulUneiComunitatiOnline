document.addEventListener("DOMContentLoaded", () => {
    const carouselInner = document.querySelector(".carousel-inner");
    const items = document.querySelectorAll(".carousel-item");
    const totalItems = items.length;

    // Clonează toate elementele pentru continuitate
    items.forEach((item) => {
        const clone = item.cloneNode(true);
        carouselInner.appendChild(clone);
    });

    // Calculează înălțimea animației
    const totalHeight = totalItems * items[0].offsetHeight;
    carouselInner.style.animation = `scrollUp ${totalItems * 10}s linear infinite`;
});
