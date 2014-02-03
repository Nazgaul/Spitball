var _gaq = _gaq || [];
_gaq.push(['_setAccount', 'UA-9850006-3']);
_gaq.push(['_setDomainName', 'multimicloud.com']);
_gaq.push(['_setAllowLinker', true]);
_gaq.push(['_trackPageview']);

(function () {
    function async_load() {
        var ga = document.createElement('script');
        ga.type = 'text/javascript';
        ga.async = true;
        ga.src = ('https:' == document.location.protocol ? 'https://ssl' : 'http://www') + '.google-analytics.com/ga.js';
        var s = document.getElementsByTagName('script')[document.getElementsByTagName('script').length - 1];
        s.parentNode.insertBefore(ga, s);
    }
    if (window.attachEvent)
        window.attachEvent('onload', async_load);
    else
        window.addEventListener('load', async_load, false);
})();
//google analytics


function async_load(src, shouldAsync) {
    try {
        var uv = document.createElement('script');
        uv.type = 'text/javascript';
        uv.async = shouldAsync === undefined ? true : shouldAsync;
        uv.src = src;
        var s = document.getElementsByTagName('script')[0];
        s.parentNode.insertBefore(uv, s);
    }
    catch (e) {

    }
}
//widget of feedback
var uvOptions = {};
//(function () {
//    var url = ('https:' == document.location.protocol ? 'https://' : 'http://') + 'widget.uservoice.com/UkUMA0g4Tp2dVp21PCpCRA.js';
//    $(window).bind('onload', async_load(url));
//    //if (window.attachEvent)
//    //    window.attachEvent('onload', async_load(url));
//    //else
//    //    window.addEventListener('load', async_load(url), false);
//})();
//cloudsponge
//function loadCloudSponge() {
//    var url = 'https://api.cloudsponge.com/address_books.js';
//    $(window).bind('onload', async_load(url));
//    //if (window.attachEvent)
//    //    window.attachEvent('onload', async_load(url));
//    //else
//    //    window.addEventListener('load', async_load(url), false);
//}
//ad this
function loadAddThis() {
    var url = 'https://s7.addthis.com/js/250/addthis_widget.js#pubid=ra-4fb0edec62fc27e9';
    async_load(url);
    //if (window.attachEvent)
    //    window.attachEvent('onload', async_load(url));
    //else
    //    window.addEventListener('load', async_load(url), false);
}
//function loadJson() {
//    var uv = document.createElement('script');
//    uv.type = 'text/javascript';
//   // uv.async = true;
//    uv.src = ('https:' == document.location.protocol ? 'https://' : 'http://') + window.location.host + "/Scripts/json2.min.js";
//    var s = document.getElementsByTagName('script')[0];
//    s.parentNode.insertBefore(uv, s);
//}



