var scriptLoaded = {
    scripts: [],
    isLoaded: function (script) {
        if (this.scripts.indexOf(script) > -1) {
            return true;
        }

        this.scripts.push(script);
    }
};

(function () {
    var cdnPath = '/cdn/gzip/', scriptsPath = '/Scripts/',
    cdnCdPath = 'vo.msecnd.net/';
    cdnGooglePath = 'ajax.googleapis.com';

    window.onload = function () {
        cssFailCallback();
       // javascriptFailCallback();
    };

    function cssFailCallback() {
        var cssLoaded = false, files;

        cssLoaded = document.getElementById('cssCheck').offsetLeft < 0;

        if (!cssLoaded) {
            files = getCDNStylesheets();
            for (var i = 0, l = files.length; i < l; i++) {
                loadStylesheet(cdnPath + files[i]);
            }
        }
    }

    function javascriptFailCallback() {
        if (window.cd && window.jQuery) {
            return;
        }

        files = getCDNScripts();

        for (var i = 0, l = files.google.length; i < l; i++) {
            loadScript(scriptsPath + getFilename(files.google[i]));
        }

        for (var i = 0, l = files.cloudents.length; i < l; i++) {
            loadScript(cdnPath + getFilename(files.cloudents[i]));
        }
     
    }

    function getCDNStylesheets() {
        var link, files = [], cssLinks = document.getElementsByTagName('link');
        for (var i = 0, l = cssLinks.length; i < l; i++) {
            link = cssLinks[i].href;
            if (link.indexOf(cdnCdPath) > -1) {
                files.push(getFilename(link));
            }
        }
        return files;
    }

    function getCDNScripts() {
        var link, cloudents = [], google= [];
        for (var i = 0, l = document.scripts.length ; i < l; i++) {
            link = document.scripts[i].getAttribute('src');
            if (link) {
                if (link.indexOf(cdnCdPath) > -1) {
                    cloudents.push(link);
                } else if (link.indexOf(cdnGooglePath) > -1) {
                    google.push(link);
                }
            }
        }

        return { cloudents: cloudents, google: google };
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