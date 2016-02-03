
(function (document) {
    //#region reviews slider
    $('.bxslider').bxSlider({
        auto: true,
        controls: false,

    });

    var options = {
        useEasing: true,
        useGrouping: true,
        separator: ',',
        decimal: '.',
        prefix: ''
    };
    var studentsCount = 180000;
    options.suffix = ' ' + $('#students').data('statistic');
    new CountUp("students", 0, studentsCount, 0, 2.5, options).start();;

    var documentsCount = 300000;
    options.suffix = ' ' + $('#documents').data('statistic');
    new CountUp("documents", 0, documentsCount, 0, 2.5, options).start();;

    var quizzesCount = 220000;
    options.suffix = ' ' + $('#quizzes').data('statistic');
    new CountUp("quizzes", 0, quizzesCount, 0, 2.5, options).start();;

    //#endregion


    //#region animations
    $('body').waypoint({
        offset: -200,
        handler: function (direction) {
            if (direction === 'down') {
                $('.go-top').css('bottom', '12px').css('opacity', '1');
            } else {
                $('.go-top').css('bottom', '-44px').css('opacity', '0');
            }
        }
    });

    // Init On scroll animations
    function onScrollInit(items, trigger) {
        items.each(function () {
            var osElement = $(this),
                osAnimationClass = osElement.attr('data-os-animation'),
                osAnimationDelay = osElement.attr('data-os-animation-delay');
            osElement.css({
                '-webkit-animation-delay': osAnimationDelay,
                '-moz-animation-delay': osAnimationDelay,
                'animation-delay': osAnimationDelay
            });

            var osTrigger = (trigger) ? trigger : osElement;

            osTrigger.waypoint(function () {
                osElement.addClass('animated').addClass(osAnimationClass);
            }, {
                triggerOnce: true,
                offset: '90%'
            });
        });
    }

    function initialisePageScrollAnimations() {
        onScrollInit($('.os-animation'));
    }

    initialisePageScrollAnimations();
    //#endregion

    //#region video shadowbox
    $(document.getElementById('close-video')).click(function () {
        $(document.getElementById('video')).empty();
    });

    $(document.getElementById('open-video')).click(function () {
        var videoHtml = '<iframe id="spitball-video" src="https://www.youtube.com/embed/69daYsNqNUA" frameborder="0" allowfullscreen></iframe>';
        $(document.getElementById('video')).html(videoHtml);
    });

    //#endregion

    Metronic.init(); // init metronic core components
    Layout.init(); // init current layout
    Login.init();

    var animatonSpeed = 300;

    $(document.getElementById('signin')).click(function () {
        toggleForm();
        $('#main-wrapper').css('min-height', 'calc(100vh - 150px)');
    });

    $('.close-form').click(function () {
        toggleForm();
        $('#main-wrapper').css('min-height', 0).height('auto');
    });

    function toggleForm() {
        $.each($('.home-page-body > *:not(.main)'),
            function () {
                $(this).slideToggle(animatonSpeed);
            });
        $('.login-wrapper, #main-wrapper h1').slideToggle(animatonSpeed);
        $('#main-wrapper .social-links, .statistics, #main-wrapper > .signin-options, header ul, .navbar-toggle').toggleClass('hidden');
    }

})(window.document);