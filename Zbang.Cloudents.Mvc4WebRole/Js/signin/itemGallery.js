﻿
function openImage(thumbImage, thumbnails) {
    $('.gallery .arrow').removeClass('disabled');

    var currThumbIndex = thumbnails.index(thumbImage);
    if (currThumbIndex == 0) {
        $('.gallery .prev').addClass('disabled').unbind("click");
    }
    if (currThumbIndex == thumbnails.length - 1) {
        $('.gallery .next').addClass('disabled').unbind("click");
    }
    var nextImage = $(thumbnails[currThumbIndex + 1]);
    var prevImage = $(thumbnails[currThumbIndex - 1]);
    var largeImage = '<img src="' + $(thumbImage).data("image") + '" alt="Class note">';
    var largeImageUrl = $(thumbImage).data("image");

    var imageContainer = $('.gallery .image');
    var image = $('.gallery .image img');
    if (image.length) {
        image.fadeOut(300, function () {
            image.attr('src', largeImageUrl);
            image.fadeIn(300);
        });
    } else {
        imageContainer.append(largeImage);
    }

    $('.gallery .next:not(.disabled)').unbind("click").click(function () {
        openImage(nextImage, thumbnails);
    });

    $('.gallery .prev:not(.disabled)').unbind("click").click(function () {
        openImage(prevImage, thumbnails);
    });

    $(document).keydown(function (e) {
        if (e.keyCode == 37) { // left
            $('.gallery .prev:not(.disabled)').trigger("click");
        }
        else if (e.keyCode == 39) { // right
            $('.gallery .next:not(.disabled)').trigger("click");
        }
    });
}



function createGallery(imageContainer) {
    var galleryContainer = ('<div class="gallery">' +
        '<div class="content">' +
        '<div class="close-button"></div>' +
        '<div class="arrow prev"></div>' +
        '<div class="image">' +
        '</div>' +
        '<div class="arrow next"></div>' +
        '</div></div>');
    $('body').append(galleryContainer);
    var thumbnails = $(imageContainer + ' img');

    thumbnails.click(function () {
        openImage($(this), thumbnails);
        $('body').addClass('gallery-open');
        $('.gallery').show();
        $('.close-button').click(function () {
            $('.gallery').hide().find('.image').empty();
            $('body').removeClass('gallery-open');
        });
    });

}