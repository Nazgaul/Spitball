handleLanguage.changeLanguage($('.language button'));

window.addEventListener("load", function load() {
    window.removeEventListener("load", load, false); //remove listener, no longer needed
    handleLanguage.updateLangOnDropDown();
}, false);


(function (document) {
   

    function handleScrollToTop() {
        if ($(this).scrollTop() > offset) {
            $('.scroll-to-top').fadeIn(duration);
        } else {
            $('.scroll-to-top').fadeOut(duration);
        }
    }

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

    function scrolledToElem($scrollTo) {
        var hT = $scrollTo.offset().top,
            wH = $(window).height(),
            wS = $(this).scrollTop();
        return wS > hT - wH;
    }

    if ($("body.homePage").length > 0) {
        var options = {
            useEasing: true,
            useGrouping: true,
            separator: ',',
            decimal: '.',
            prefix: '',
            suffix: ''
        };
        var studentsCount = ' ' + $('#students').data('value');
        new CountUp("students", 0, studentsCount, 0, 2.5, options).start();

        var documentsCount = ' ' + $('#documents').data('value');
        new CountUp("documents", 0, documentsCount, 0, 2.5, options).start();

        var quizzesCount = ' ' + $('#quizzes').data('value');
        new CountUp("quizzes", 0, quizzesCount, 0, 2.5, options).start();

        //#endregion

        var hasBoxes = false, hasReviews = false;

        //#region scroll to top
        var offset = 300;
        var duration = 500;

        if (navigator.userAgent.match(/iPhone|iPad|iPod/i)) {  // ios supported
            $(window).bind("touchend touchcancel touchleave", function () {
                handleScrollToTop();
            });
        } else {  // general 
            $(window).scroll(function () {
                handleScrollToTop();
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
        Login.init();

        $(window).scroll(function () {
            if (!hasBoxes && scrolledToElem($('section.check-us'))) {
                hasBoxes = true;
                $.get("/home/boxes", function (data) {
                    var boxes = data.payload;
                    if (!boxes || !boxes.length) {
                        scrollTo.addClass('no-boxes');
                        return;
                    }
                    var boxElement = $('#box-template-flex').html();

                    for (var box in boxes) {
                        var currBox = boxes[box];
                        var boxClass = "color" + currBox.name.length % 11;
                        var mapObj = {
                            '{boxUrl}': currBox.url,
                            '{boxName}': currBox.name,
                            '{boxLetters}': currBox.name.substr(0, 2).toLowerCase(),
                            '{boxProfessor}': currBox.professor,
                            '{boxCourseCode}': currBox.courseCode,
                            '{boxClass}': boxClass,
                            '{boxItemCount}': currBox.itemCount
                        };

                        box = boxElement.replace(new RegExp(Object.keys(mapObj).join("|"), 'g'), function (matched) {
                            return mapObj[matched] || '';
                        });

                        $('.boxes').append(box);
                    }


                });
            }


            if (!hasReviews && scrolledToElem($('section.reviews'))) {
                hasReviews = true;
                $('.reviews .bxslider').show();
                //#region reviews slider
                $('.bxslider').bxSlider({
                    auto: true,
                    controls: false,
                    pause: 15000,
                    pager: false,
                    touchEnabled: false

                });


                $('footer .app-links').show();
            }
        });
    }

})(window.document);