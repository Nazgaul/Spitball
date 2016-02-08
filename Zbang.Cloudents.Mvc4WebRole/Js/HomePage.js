
(function (document) {
    //#region reviews slider
    $('.bxslider').bxSlider({
        auto: true,
        controls: false,
        pause: 15000,
    });

    var options = {
        useEasing: true,
        useGrouping: true,
        separator: ',',
        decimal: '.',
        prefix: ''
    };

    options.suffix = ' ' + $('#students').data('statistic');
    var studentsCount = ' ' + $('#students').data('value');
    new CountUp("students", 0, studentsCount, 0, 2.5, options).start();;

    options.suffix = ' ' + $('#documents').data('statistic');
    var documentsCount = ' ' + $('#documents').data('value');
    new CountUp("documents", 0, documentsCount, 0, 2.5, options).start();;

    options.suffix = ' ' + $('#quizzes').data('statistic');
    var quizzesCount = ' ' + $('#quizzes').data('value');
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

    //$('#signin)' +

    $('.login-option.signup').click(function () {
        toggleForm('register-form');
        $('#main-wrapper').css('min-height', 'calc(100vh - 150px)');
    });
    $('#signin').click(function () {
        toggleForm('login-form');
        $('#main-wrapper').css('min-height', 'calc(100vh - 150px)');
    });

    $('.close-form').click(function () {
        toggleForm($(this).parent().attr('class'));
        $('#main-wrapper').css('min-height', 0).height('auto');
        window.history.replaceState(null, "Home", "/");
    });

    function toggleForm(form) {
        $.each($('.home-page-body > *:not(.main)'),
            function () {
                $(this).slideToggle(animatonSpeed);
            });
        $('.login-wrapper, #main-wrapper h1').slideToggle(animatonSpeed);
        $('.login-wrapper .content > form:not(.' + form + ')').each(function() {
            $(this).addClass('hidden');
        });
        $('.login-wrapper .content > form.' + form).removeClass('hidden').show();
        $('#main-wrapper .social-links, .statistics, #main-wrapper > .signin-options, header ul, .navbar-toggle').toggleClass('hidden');
    }

})(window.document);