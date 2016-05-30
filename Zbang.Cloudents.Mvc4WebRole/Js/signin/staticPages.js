var Login = true; //we need this for cdn fallback.
(function () {
    $(document).on('click', '.panel-title a', function (e) {
        var $panel = $(this).parents('.panel').first();
        $panel.toggleClass('expanded');
        $panel.find('.panel.expanded a').click();
        $panel.siblings().removeClass('expanded');
    });


    function getCookie(cname) {
        var name = cname + "=";
        var ca = document.cookie.split(';');
        for (var i = 0; i < ca.length; i++) {
            var c = ca[i];
            while (c.charAt(0) == ' ') c = c.substring(1);
            if (c.indexOf(name) == 0) return c.substring(name.length, c.length);
        }
        return "";
    }

    $('.item').each(function () {
        var $this = $(this);
        var image = $this.find('img');
        image.attr('src', 'https://az779114.vo.msecnd.net/preview/' + encodeURIComponent(image.data('source')) + '.jpg');
        var $name = $this.find('.item-name');
        $name.html($name.html().replace(/\.[^/.]+$/, ""));
    });


    changeLanguage($('.language button'));

    $(function () {
        var slider = $('.items-container').bxSlider({
            infiniteLoop: false,
            slideWidth: 130,
            maxSlides: 7,
            pager: false,
            nextSelector: $('.nav-next'),
            prevSelector: $('.nav-prev'),
            hideControlOnEnd: true,
            mode: 'horizontal',
        });

        var currUrl = window.location.href.split("/")[3];
        var active = $('.nav-bar .items-container a[href="/' + currUrl + '/"], .static-page-header li a[href="/' + currUrl + '/"]').addClass('active');
        if (slider.length && window.innerWidth < $('.nav-bar').width() + 40) {
            slider.goToSlide($('.items-container a').index(active));
        }

        var val = getCookie('l3').toLowerCase();
        var text = $('.language').find('button[data-cookie="' + val + '"]').html();
        $('nav li.language span:first').text(text);

        window.Intercom('boot', {
            app_id: "njmpgayv",
        });

        (function (i, s, o, g, r, a, m) {
            i['GoogleAnalyticsObject'] = r;
            i[r] = i[r] || function () {
                (i[r].q = i[r].q || []).push(arguments)
            }, i[r].l = 1 * new Date();
            a = s.createElement(o),
                m = s.getElementsByTagName(o)[0];
            a.async = 1;
            a.src = g;
            m.parentNode.insertBefore(a, m)
        })(window, document, 'script', '//www.google-analytics.com/analytics.js', 'ga');

        ga('create', 'UA-9850006-3', 'auto');
        ga('send', 'pageview');


        $('.mobileLink').click(function (e) {
            ga('send', 'event', 'Mobile Link', e.target.id);
        });

        createGallery('.gallery-images');
    });
})()