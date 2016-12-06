(function () {
    function setBackground() {
        var background = $('[background-cover]');
        var main = $('.main');
        //var header = $('.static-page-header');
        var backgroundUrl = 'https://az779114.vo.msecnd.net/universities/cover/' +
            encodeURIComponent(background.data('image')) +
            '?mode=crop&anchor=topcenter&quality=70&scale=both&width=' +
            main.outerWidth() +
            '&height=' +
            ($(window).outerHeight());
        background.css('background-image', 'url(' + backgroundUrl + ')');
        if ($('.welcome-text').length && $('.offset-bottom') && $('.static-page-header')) {
            var padding = $(window).height() + $('.welcome-text').offset().top - $('.offset-bottom').offset().top - $('.static-page-header').height();
            $('#main-wrapper').css('padding-top', padding);
        }
        //$('#main-wrapper').find(".promo-title").height(padding - 100);

    }
    setBackground();
    var timeout;
    $(window).resize(function () {
        clearTimeout(timeout);
        timeout = setTimeout(setBackground, 1000);
    });
    $('.close-form')
        .click(function () {
            setBackground();
        });
})();