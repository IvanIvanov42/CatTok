function masonryLayout() {
    var elems = document.querySelectorAll('.masonry');
    elems.forEach(function (elem) {
        imagesLoaded(elem, function () {
            new Masonry(elem, {
                itemSelector: '.masonry-item',
                percentPosition: true
            });
        });
    });
}