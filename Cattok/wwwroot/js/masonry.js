function masonryLayout() {
    var elem = document.querySelector('.masonry');
    imagesLoaded(elem, function () {
        var msnry = new Masonry(elem, {
            itemSelector: '.masonry-item',
            columnWidth: '.masonry-item',
            percentPosition: true
        });
    });
}
