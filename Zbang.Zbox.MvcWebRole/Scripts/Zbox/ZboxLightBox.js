$(window).ready(function () {
    ZeroClipboard.setMoviePath('/Scripts/ZeroClipboard/ZeroClipboard.swf');
    var $myCarousel = $('#mycarousel');
    var $myCarouselImages = $myCarousel.find('img');
    $('#lb-comment').watermark('Comment', { className: 'style5', useNative: false });

    // close 
    $('#btnLbClose').click(function (event) {
        Zbox.LightBox.CloseLB();
        return false;
    });
    $myCarousel.jcarousel({
        scroll: 1,
        initCallback: Zbox.LightBox.mycarousel_initCallback,
        // This tells jCarousel NOT to autobuild prev/next buttons
        buttonNextHTML: null,
        buttonPrevHTML: null
    });

    $myCarousel.click(function (e) {
        var target = e.target;
        if (e.target.nodeName === 'IMG' && e.target.parentNode.nodeName === 'LI') {
            target = e.target.parentNode;
        }
        if (target.nodeName === 'LI') {
            $e = $(target);

            $myCarousel.find('li.lbActiveThumb').removeClass('lbActiveThumb');
            //$('#mycarousel ul li.lbActiveThumb').removeClass('lbActiveThumb');
            $e.addClass('lbActiveThumb');


            var img = $e.children('img');
            var itemToShowHRef = img.attr('data-href');
            var $LightBoxMainPic = $('#LightBoxMainPic');
            $LightBoxMainPic.empty();
            if (Zbox.LightBox.isDocument(img.attr('data-name'))) {
                var fileExtension = /[^.]+$/.exec(img.attr('data-name'));                
                var iFrameToLoad = $('<iframe/>').attr({
                    id: 'iFramedoc',
                    width: 710,
                    height: 546,
                    onload: function () {
                        GetZohoUrl(this, fileExtension);
                    }
                });
                $LightBoxMainPic.append(iFrameToLoad);

            }
            else if (Zbox.LightBox.isLink(itemToShowHRef)) {
                var iFrameToLoad = $('<iframe/>').attr({
                    src: itemToShowHRef,
                    width: 710,
                    height: 546
                });
                $LightBoxMainPic.append(iFrameToLoad);
            }
//            else if (Zbox.LightBox.isAudio(img.attr('data-name'))) {
//                var audio = $('<audio></audio>').attr({
//                    src : itemToShowHRef,
//                    controls : 'controls'
//                });
//                $LightBoxMainPic.append(audio);

//            }
            else {
                //if (isImage(img.attr('data-name'))) {
                var imgToLoad = $('<img/>').attr('src', itemToShowHRef);
                $LightBoxMainPic.append(imgToLoad);
            }
            if (img.attr('data-fileSize') != 0)
                $('#LightBoxfileSize').text(img.attr('data-fileSize') || '');

            var re = /-?\d+/;
            var m = re.exec(img.attr('data-fileUploaded'));
            var d = new Date(parseInt(m[0]));

            $('#LightBoxFileCreated').text(d.toDateString());
            $('#LightBoxItemName').text(img.attr('data-Name'));

            $('#LightBoxUploader').text(img.attr('data-uploaderName'));

            Zbox.LightBox.GetComments(img);
        }
    });


    $('#LightBoxMoveRight').click(function () {
        var currentImg = getCarouselCurrentImage();
        currentImg.parent().next().click();

        Zbox.LazyLoadOfImages($myCarousel, $myCarouselImages);

    });

    $('#LightBoxMoveLeft').click(function () {
        var currentImg = getCarouselCurrentImage();
        currentImg.parent().prev().click();
        Zbox.LazyLoadOfImages($myCarousel, $myCarouselImages);

    });
    $('#LightBoxFadeMe').click(function () {
        Zbox.LightBox.CloseLB();
    });
    $('#LightBoxDownload').click(function () {
        window.open(Zbox.LightBox.GetDownloadLink());
    });

    $('#LightBoxDelete').click(function () {
        var itemId = getCurrentItemId();
        $("#removeItem_" + itemId).trigger('click');
        Zbox.LightBox.CloseLB();
    });

    function GetZohoUrl(iframe, fileExtension) {
        var itemId = getCurrentItemId();
        var GetLink = new ZboxAjaxRequest({
            beforeSend: function () {
            },
            url: '/AStorage/BlobSharedAccessUrl',
            data: { fileId: itemId },
            success: function (data) {
                iframe.src = 'https://viewer.zoho.com/docs/urlview.do?url=' + encodeURIComponent(data + "&." + fileExtension) + '&embed=true';
            },
            error: function (error) {
                iframe.src = '';
            },
            complete: function () {
            }

        });

        GetLink.Post();
    }
    function getCurrentItemId() {
        var currentImg = getCarouselCurrentImage();
        var itemId = currentImg.attr('data-itemId');
        return itemId;
    }
    function getCarouselCurrentImage() {
        return $('#mycarousel').find('li.lbActiveThumb').find('img');
    }

    $('#btnShareLighbxComment').click(function () {
        // var self = this;
        //if (e.which == jQuery.ui.keyCode.ENTER) {

        var commentText = $.trim($('#lb-comment').val());
        if (commentText == '') {
            $(this).focusout();
            return;
        }
        var itemId = getCurrentItemId();
        var boxId = Zbox.Box.boxes.GetCurrentBox().BoxId;

        var postItemComment = new ZboxAjaxRequest({
            beforeSend: function () {
            },
            url: '/Comment/AddItemComment',
            data: { commentText: escape(commentText), itemId: itemId, boxId: boxId },
            success: function (comment) {

                if (comment.itemId == itemId) {
                    var commentItem = $('#LightBoxItemComment').clone();
                    var htmlComment = Zbox.changeTemplateText(commentItem.html(), comment.comment);

                    $('#LightHouseCommentSection').prepend(htmlComment);
                }

                Zbox.UpdateScreenTime();
            },
            error: function (error) {
            },
            complete: function () {
                $('#lb-comment').val('');
            }

        });

        postItemComment.Post();

    });
});

Zbox.LightBox = {};

Zbox.LightBox.Cache = [];
/**
* We use the initCallback callback
* to assign functionality to the controls
*/
//var carouselVariable = null;

Zbox.LightBox.mycarousel_initCallback = function (carousel) {
    //carouselVariable = carousel;
    //        jQuery('.jcarousel-control a').bind('click', function () {
    //            carousel.scroll(jQuery.jcarousel.intval(jQuery(this).text()));
    //            return false;
    //        });
    var $myCarousel = $('#mycarousel');
    var $myCarouselImages = $('#mycarousel').find('img');
    jQuery('#mycarousel-next').bind('click', function () {
        carousel.next();
        Zbox.LazyLoadOfImages($myCarousel, $myCarouselImages);
        return false;
    });
    jQuery('#mycarousel-prev').bind('click', function () {
        carousel.prev();
        Zbox.LazyLoadOfImages($myCarousel, $myCarouselImages);
        return false;
    });
};

// Ride the carousel...


Zbox.LightBox.CloseLB = function () {
    for (var i = 0; i < ZeroClipboard.clients.length; i++) {
        i.destroy();
    }
    $('#LightBoxFadeMe').hide();
    $('#LightBox').hide();
    this.ClearLightBox();
}

Zbox.LightBox.ClearLightBox = function () {
    $('#LightBoxBoxName').text('');
    $('#LightBoxItemName').text('');
    $('#LightBoxfileSize').text('');
    $('#LightBoxFileCreated').text('');
    $('#LightBoxUploader').text('');
    $('#LightBoxMainPic').empty();

}

Zbox.LightBox.ShowLightBox = function (itemClicked) {
    var self = this;
    $('#LightBoxFadeMe').show();
    self.initLightBox(itemClicked);
    $('#LightBox').show(400, function () {

        var clip = new ZeroClipboard.Client();
        clip.setText(' '); // will be set later on mouseDown
        clip.setHandCursor(true);
        clip.addEventListener('mouseDown', function (client) {
            clip.setText(self.GetDownloadLink());
        });
        clip.addEventListener('onComplete', function (client, text) {
            Zbox.toaster('Url copy to clipboard');
        });
        clip.glue('d_clip_button', 'd_clip_container');
    });
}


Zbox.LightBox.GetDownloadLink = function () {
    var $itemOn = $('#mycarousel').find('li.lbActiveThumb').find('img');
    if (Zbox.LightBox.isLink($itemOn.attr('data-name'))) {
        return $itemOn.attr('data-href');
    }
    var urltext = $itemOn.attr('data-href');

    var i = urltext.lastIndexOf('/');

    var urltext = urltext.slice(0, i + 1);
    var host = this.GetHost();
    return host + urltext + 'download';
}

Zbox.LightBox.GetHost = function () {
    var j = window.location.href.lastIndexOf('/');
    return window.location.href.slice(0, j);
}
//Zbox.LightBox.isImage = function(url) {
//    return /\.(gif|png|jpg|jpeg|bmp)(?:\?([^#]*))?(?:#(\.*))?$/i.test(url);
//}
Zbox.LightBox.isDocument = function (url) {
    return /\.(doc|docx|xls|xlsx|ppt|pptx|pps|odt|ods|odp|sxw|sxc|sxi|wpd|pdf|rtf|html|txt|csv|tsv)(?:\?([^#]*))?(?:#(\.*))?$/i.test(url);
}
Zbox.LightBox.isAudio = function (url) {
    var val = /\.(m4a|mp3|ogg|wav)(?:\?([^#]*))?(?:#(\.*))?$/i.test(url);
    return val && Modernizr.audio;
}

Zbox.LightBox.isLink = function (url) {
    var regEx = new RegExp('^(http|https|ftp)\://([a-zA-Z0-9\.\-]+(\:[a-zA-Z0-9\.&amp;%\$\-]+)*@)*((25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9])\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[0-9])|localhost|([a-zA-Z0-9\-]+\.)*[a-zA-Z0-9\-]+\.(com|edu|gov|int|mil|net|org|biz|arpa|info|name|pro|aero|coop|museum|[a-zA-Z]{2}))(\:[0-9]+)*(/($|[a-zA-Z0-9\(\)\.\,\?\'\\\+&amp;%\$#\=~_\-]+))*$');
    return regEx.test(url);
}
Zbox.LightBox.initLightBox = function (itemClicked) {


    var $myCarousel = $('#mycarousel');
    var currentBox = Zbox.Box.boxes.GetCurrentBox();
    $('#LightBoxBoxName').text(currentBox.BoxName);
    $('#LightBoxItemName').text($(itemClicked).attr('data-name'));

    var itemPictures = $('#divFiles').find('.boxItem');

    //reset and reinitialize the carousel
    $myCarousel.jcarousel('reset');
    $.each(itemPictures, function (index, itemPic) {
        var img = $(itemPic).find('.item-thumb').find('img');
        //$(img).attr('src', $(img).attr('original'));
        var liItem = $('<li></li>').html(img.clone());
        $myCarousel.jcarousel('add', index, liItem);
        $myCarousel.jcarousel('size', index);

    });

    //finding the item in the carousel and click on it
    var itemRequested = $(itemClicked).attr('data-href');
    $myCarousel.find('img[data-href="' + itemRequested + '"]').parent().click();
    Zbox.LazyLoadOfImages($myCarousel, $myCarousel.find('img'));
}


Zbox.LightBox.GetComments = function (img) {
    var self = this;
    var boxId = Zbox.Box.boxes.GetCurrentBox().BoxId;
    var itemId = img.attr('data-itemid');
    var $LightHouseCommentSection = $('#LightHouseCommentSection');
    var getBoxItemComment = new ZboxAjaxRequest({
        beforeSend: function () {
            $LightHouseCommentSection.empty();
            $LightHouseCommentSection.text('loading...');
        },
        url: '/Comment/GetItemComments',
        data: { boxId: boxId, itemId: itemId },
        success: function (comments) {
            if (comments.boxId == boxId) {
                if (comments.itemId == itemId) {
                    $LightHouseCommentSection.empty();
                    Zbox.LightBox.renderComments(comments.comments);
                    //self.cache[boxId + '-' + itemId] = comments.comments;
                }
            }
            Zbox.UpdateScreenTime();
        },
        error: function (error) {
        },
        complete: function () {
        }
    });

    getBoxItemComment.Get();
}
Zbox.LightBox.renderComments = function (commentsData) {
    var comments = [];
    var $LightBoxItemComment = $('#LightBoxItemComment').clone();
    $.each(commentsData, function (i, comment) {
        var commentItem = $LightBoxItemComment;
        var htmlComment = Zbox.changeTemplateText(commentItem.html(), comment);
        comments.push(htmlComment);

    });
    $('#LightHouseCommentSection').prepend(comments.join(''));
}