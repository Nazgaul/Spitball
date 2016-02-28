﻿(function () {
    var cdnPath = '/cdn/gzip/',
       // scriptsPath = '/Scripts/',
        cdnCdPath = '.vo.msecnd.net';
    //cdnGooglePath = 'ajax.googleapis.com';

    window.addEventListener("load", function load() {
    //window.onload = function () {
        cssFailCallback();
        javascriptFailCallback();
    });

    function cssFailCallback() {
        var cssLoaded = document.getElementById('cssCheck').offsetTop < 0, files;

        if (!cssLoaded) {
            files = getCdnStylesheets();
            for (var i = 0, l = files.length; i < l; i++) {
                loadStylesheet(cdnPath + files[i]);
            }
        }
    }

    function javascriptFailCallback() {
        var scripts = [], isBootstrap = false;//, count, interval;

        if (!window.jQuery) {
            scripts = getCdnScripts(cdnCdPath);
            load(cdnPath);
            isBootstrap = true;
            //scripts = getCdnScripts(cdnGooglePath);
            //if (scripts.length) {
            //    load(scriptsPath);
            //    isBootstrap = true;
            //}
        }

        if (!window.Login) {
            scripts = getCdnScripts(cdnCdPath);
            load(cdnPath);
            isBootstrap = true;
        }

        if (!isBootstrap) {
            return;
        }

        //count = 0;
        //interval = setInterval(function () {
        //    count++;
        //    if (!window.angular) {
        //        if (count == 500) {
        //            clearInterval(interval);
        //            window.location.reload();
        //            return;
        //        }
        //        return;
        //    }

        //    clearInterval(interval);


        //    if (angular.element('app').injector()) {
        //        return;
        //    }

        //    window.angular.bootstrap(document, ['app']);
        //}, 20);

        function load(path) {
            for (var i = 0, l = scripts.length; i < l; i++) {
                loadScript(path + getFilename(scripts[i]));
            }
        }

    }

    function getCdnStylesheets() {
        var link, files = [], cssLinks = document.getElementsByTagName('link');
        for (var i = 0, l = cssLinks.length; i < l; i++) {
            link = cssLinks[i].href;
            if (link.indexOf(cdnCdPath) > -1) {
                files.push(getFilename(link));
            }
        }
        return files;
    }

    function getCdnScripts(path) {
        var link, scripts = [];
        for (var i = 0, l = document.scripts.length ; i < l; i++) {
            link = document.scripts[i].getAttribute('src');
            if (link) {
                if (link.indexOf(path) > -1) {
                    scripts.push(link);
                }
            }
        }
        return scripts;
    }

    function loadStylesheet(url) {
        var style = document.createElement('link');
        style.setAttribute('rel', 'stylesheet');
        style.setAttribute('type', 'text/css');
        style.setAttribute('href', url);
        document.getElementsByTagName('head')[0].appendChild(style);
    }

    function loadScript(url) { //we do it like this so we load the scripts synchronously
        var xhr = new XMLHttpRequest();
        xhr.open('GET', url, false);
        xhr.send(null);
        var script = document.createElement('script');
        script.type = 'text/javascript';
        script.async = false;
        script.text = xhr.responseText;
        document.getElementsByTagName('head')[0].appendChild(script);
    }

    function getFilename(url) {
        if (!url)
            return '';
        return url.substring(url.lastIndexOf('/') + 1);
    }
}());