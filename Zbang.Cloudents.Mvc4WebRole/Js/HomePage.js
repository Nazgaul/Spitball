
(function (document) {
    //#region reviews slider
    $('.bxslider').bxSlider({
        auto: true,
        controls: false,
        pause: 15000,
        pager: false,
        touchEnabled:false

    });

    var options = {
        useEasing: true,
        useGrouping: true,
        separator: ',',
        decimal: '.',
        prefix: '',
        suffix: ''
    };

    var studentsCount = ' ' + $('#students').data('value');
    new CountUp("students", 0, studentsCount, 0, 2.5, options).start();;

    var documentsCount = ' ' + $('#documents').data('value');
    new CountUp("documents", 0, documentsCount, 0, 2.5, options).start();;

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

    $('#showVideo').change(function() {
        if (this.checked) {
            var videoHtml = '<iframe id="spitball-video" src="https://www.youtube.com/embed/69daYsNqNUA" frameborder="0" allowfullscreen></iframe>';
            $(document.getElementById('video')).html(videoHtml);
        } else {
            $(document.getElementById('video')).empty();
        }
    });
    ////#region video shadowbox
    //$(document.getElementById('close-video')).click(function () {
    //    $(document.getElementById('video')).empty();
    //});

    //$(document.getElementById('open-video')).click(function () {
    //    var videoHtml = '<iframe id="spitball-video" src="https://www.youtube.com/embed/69daYsNqNUA" frameborder="0" allowfullscreen></iframe>';
    //    $(document.getElementById('video')).html(videoHtml);
    //});

    //#endregion

    Metronic.init(); // init metronic core components
    Layout.init(); // init current layout
    Login.init();

    var animatonSpeed = 300;

    $('.login-wrapper input').focus(function () {
        $('.alert-danger').slideUp(animatonSpeed);
    });

    $('.login-option.signup').click(function (e) {
        e.preventDefault();
        toggleForm('register-form');
        $('#main-wrapper').css('min-height', 'calc(100vh - 150px)');
    });
    $('.signin-btn').click(function (e) {
        e.preventDefault();
        toggleForm('login-form');
        $('#main-wrapper').css('min-height', 'calc(100vh - 150px)');
        $(this).addClass('hidden');
    });

    $('.close-form').click(function () {
        $.each($('.home-page-body > *:not(.main)'),
            function () {
                $(this).slideDown(animatonSpeed);
            });
        $('.login-wrapper, #main-wrapper .welcome-text').slideDown(animatonSpeed).removeClass('hidden');
        $('.login-wrapper').slideUp(animatonSpeed);
        $('.toggle').removeClass('hidden');
        $('#main-wrapper').css('min-height', 0).height('auto');
        window.history.replaceState(null, "Home", "/");

    });

    function toggleForm(form) {
        document.body.scrollTop = document.documentElement.scrollTop = 0;
        $.each($('.home-page-body > *:not(.main)'),
            function () {
                $(this).slideToggle(animatonSpeed);
            });
        $('.login-wrapper, #main-wrapper .welcome-text').slideToggle(animatonSpeed);
        $('.login-wrapper .content > form:not(.' + form + ')').each(function () {
            $(this).addClass('hidden');
        });
        $('.login-wrapper .content > form.' + form).removeClass('hidden').show();
        $('.toggle').toggleClass('hidden');
    }

})(window.document);