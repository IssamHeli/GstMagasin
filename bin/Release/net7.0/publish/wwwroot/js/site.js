function isInViewport(element) {
    var rect = element.getBoundingClientRect();
    return (
        rect.top <= (window.innerHeight || document.documentElement.clientHeight) &&
        rect.left >= 0 &&
        rect.bottom >= 0 &&
        rect.right <= (window.innerWidth || document.documentElement.clientWidth)
    );
}
function handleScrollAnimations() {
    var elements = document.querySelectorAll('.animate-scroll');

    elements.forEach(function (element) {
        if (isInViewport(element)) {
            element.classList.add('show');
        }
    });

}

window.addEventListener('scroll', handleScrollAnimations);

// Initial check on page load
window.addEventListener('load', handleScrollAnimations);
