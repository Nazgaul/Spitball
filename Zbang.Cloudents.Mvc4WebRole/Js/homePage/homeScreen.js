var oxyThemeData = {
    navbarHeight: 100,
    navbarScrolled: 90,
    navbarScrolledPoint: 30,
    menuClose: 'off',
    scrollFinishedMessage: 'No more items to load.',
    hoverMenu:
    {
        hoverActive: false,
        hoverDelay: 200,
        hoverFadeDelay: 200
    },
    siteLoader: 'on'
};

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

window.addEventListener("load", function load(event) {
    window.removeEventListener("load", load, false); //remove listener, no longer needed
    var removeCloudentsBanner = 'removeCloudentsBanner';
    $('.alert').on('closed.bs.alert', function () {
        localStorage.setItem(removeCloudentsBanner, '1');
    });

    $('.mobileLink').on('click', function (e) {
        ga('send', 'event', 'Mobile Link', e.target.id);
    });

    var willShow = localStorage.getItem(removeCloudentsBanner);
    if (willShow != null) {
        $('.alert').alert('close');
    }
    var val = getCookie('l2').toLowerCase();
    var text = document.querySelector('[href$="' + val + '/"]').text;
    document.querySelector('[data-toggle=dropdown] span').innerText = text;

    (function (i, s, o, g, r, a, m) {
        i['GoogleAnalyticsObject'] = r; i[r] = i[r] || function () {
            (i[r].q = i[r].q || []).push(arguments)
        }, i[r].l = 1 * new Date(); a = s.createElement(o),
        m = s.getElementsByTagName(o)[0]; a.async = 1; a.src = g; m.parentNode.insertBefore(a, m)
    })(window, document, 'script', '//www.google-analytics.com/analytics.js', 'ga');

    ga('create', 'UA-9850006-3', 'auto');
    ga('send', 'pageview');
}, false);




