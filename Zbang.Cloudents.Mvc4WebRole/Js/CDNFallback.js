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
    var cdnPath = '/cdn/', scriptsPath = '/Scripts/',
    cdnCdPath = 'vo.msecnd.net/',
    cdnMsPath = '//ajax.aspnetcdn.com/';

    window.onload = function () {        
        cssFailCallback();
        javascriptFailCallback();
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
        var jqueryActive = false, resourcesActive = false, files;
        if (window.jQuery) {
            jqueryActive = true;    //jQuery loaded
        }
        if (window.cd && window.ko) {
            resourcesActive = true;  //Resources loaded
        }

        if (!jqueryActive) {
            files = getCDNScripts(true); //Both failed 
            for (var i = 0, l = files.length; i < l; i++) {
                if (files[i].indexOf(cdnMsPath) > -1) {
                    loadScript(scriptsPath + getFilename(files[i]));
                } else if (files[i].indexOf(cdnCdPath) > -1) {
                    loadScript(cdnPath + getFilename(files[i]));
                }
            }
        } else if (!resourcesActive) { //Cloudents CDN failed
            files = getCDNScripts(false);
            for (var i = 0, l = files.length; i < l; i++) {
                if (files[i].indexOf(cdnCdPath) > -1) {
                    loadScript(cdnPath + getFilename(files[i]));
                }
            }
        } else {
            //All good
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

    function getCDNScripts(totalCDNFail) {
        var link, files = [];
        for (var i = 0, l = document.scripts.length ; i < l; i++) {
            link = document.scripts[i].getAttribute('src');
            if (link) {
                if (totalCDNFail) {
                    if (link.indexOf(cdnMsPath) > -1 || link.indexOf(cdnPath) > -1) {
                        files.push(link);
                    }
                } else {
                    if (link.indexOf(cdnCdPath) > -1) {
                        files.push(link);
                    }
                }
            }
        }
        return files;
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