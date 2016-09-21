var handleLanguage = (function () {
    var langCookie = 'l3';

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

    function getLangCookie() {
        return getCookie(langCookie);
    }

    function changeLanguage($langElem) {
        $langElem.click(function () {
            document.cookie = langCookie + "=" + $(this).data('cookie') + "; path=/;domain=spitball.co";
            location.reload();
        });
    }

    function updateLangOnDropDown() {
        var val = getCookie(langCookie).toLowerCase();
        var text = $('.language').find('button[data-cookie="' + val + '"]').html();
        $('nav li.language span:first').text(text);
    }

    return {
        getLangCookie: getLangCookie,
        changeLanguage: changeLanguage,
        updateLangOnDropDown: updateLangOnDropDown
    }
})();
