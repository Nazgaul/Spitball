handleLanguage.changeLanguage($('.language button'));

window.addEventListener("load", function load() {
    window.removeEventListener("load", load, false); //remove listener, no longer needed
    handleLanguage.updateLangOnDropDown();

    (function (i, s, o, g, r, a, m) {
        i['GoogleAnalyticsObject'] = r; i[r] = i[r] || function () {
            (i[r].q = i[r].q || []).push(arguments)
        }, i[r].l = 1 * new Date(); a = s.createElement(o),
        m = s.getElementsByTagName(o)[0]; a.async = 1; a.src = g; m.parentNode.insertBefore(a, m)
    })(window, document, 'script', '//www.google-analytics.com/analytics.js', 'ga');

    ga('create', 'UA-9850006-3', 'auto');
    ga('send', 'pageview');


    $('.mobileLink').click(function (e) {
        ga('send', 'event', 'Mobile Link', e.target.id);
    });

}, false);


(function (document) {
    //#region reviews slider
    $('.bxslider').bxSlider({
        auto: true,
        controls: false,
        pause: 15000,
        pager: false,
        touchEnabled: false

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

    var hasBoxes = false;

    //#region scroll to top
    var offset = 300;
    var duration = 500;

    if (navigator.userAgent.match(/iPhone|iPad|iPod/i)) {  // ios supported
        $(window).bind("touchend touchcancel touchleave", function () {
            if ($(this).scrollTop() > offset) {
                $('.scroll-to-top').fadeIn(duration);
            } else {
                $('.scroll-to-top').fadeOut(duration);
            }
        });
    } else {  // general 
        $(window).scroll(function () {
            if ($(this).scrollTop() > offset) {
                $('.scroll-to-top').fadeIn(duration);
            } else {
                $('.scroll-to-top').fadeOut(duration);
            }
        });
    }

    $('.scroll-to-top').on('click', function () {
        $('html, body').animate({ scrollTop: 0 }, duration);
    });
    //#endregion

    //#region animations
    $('body').waypoint({
        offset: -200,
        handler: function (direction) {
            var goTop = $('.go-top');
            if (direction === 'down') {
                goTop.css('bottom', '12px').css('opacity', '1');
            } else {
                goTop.css('bottom', '-44px').css('opacity', '0');
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

    $('#showVideo').change(function () {
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

    //Metronic.init(); // init metronic core components
    //Layout.init(); // init current layout
    Login.init();


    $(window).scroll(function () {
        if (!hasBoxes) {
            var scrollTo = $('section.check-us');
            var hT = scrollTo.offset().top,
                hH = scrollTo.outerHeight(),
                wH = $(window).height(),
                wS = $(this).scrollTop();
            if (wS > hT - wH) {
                hasBoxes = true;
                $.get("/home/boxes", function (data) {
                    var boxes = data.payload;
                    var boxElement = $('#box-template').html();

                    for (box in boxes) {
                        var currBox = boxes[box];
                        var boxClass = "color" + currBox.name.length % 11;
                        var mapObj = {
                            '{boxUrl}'          : currBox.url ? currBox.url : '',
                            '{boxName}'         : currBox.name ? currBox.name : '',
                            '{boxProfessor}'    : currBox.professor ? currBox.professor : '',
                            '{boxCourseCode}'   : currBox.courseCode ? currBox.courseCode : '',
                            '{boxClass}'        : boxClass ? boxClass : '',
                            '{boxMembersCount}'     : currBox.membersCount ? currBox.membersCount : '',
                            '{boxItemCount}'    : currBox.itemCount ? currBox.itemCount : ''
                        };

                        box = boxElement.replace(new RegExp(Object.keys(mapObj).join("|"), 'g'), function (matched) {
                            return mapObj[matched];
                        });

                        $('.boxes').append(box);
                    }

                      
                });
            }
        }
    });

})(window.document);