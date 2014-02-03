﻿var _gaq = _gaq || [];

var x = document.getElementById('userName');//.getAttribute('data-id')
_gaq.push(['_setAccount', 'UA-9850006-3'],
            ['_setDomainName', 'cloudents.com'],
            ['_setAllowLinker', true],
            ['_trackPageview', location.pathname + location.search + location.hash],
            ['_setSiteSpeedSampleRate', 15]);
// ['_setCustomVar', 5, 'visitorid', cd.userDetail().id, 1];

(function (cd) {
    function async_load() {
        var ga = document.createElement('script');
        ga.type = 'text/javascript';
        ga.async = true;
        ga.src = ('https:' === document.location.protocol ? 'https://ssl' : 'http://www') + '.google-analytics.com/ga.js';
        var s = document.getElementsByTagName('script')[document.getElementsByTagName('script').length - 1];
        s.parentNode.insertBefore(ga, s);

        _gaq.push(['_setCustomVar', 5, 'visitorid', cd.userDetail().id, 1]);
    }
    if (window.attachEvent) {
        window.attachEvent('onload', async_load);
    }
    else {

        window.addEventListener('load', async_load, false);
    }

    cd.analytics = {
        trackEvent: function (category, action, opt_label) {
            _gaq.push(['_trackEvent', category, action, opt_label]);
        },
        trackSocial: function (targetUrl, action) {
            _gaq.push(['_trackSocial', 'facebook', action, targetUrl]);
        },
        trackPage: function (targetUrl) {
            _gaq.push(['_trackPageview', targetUrl]);
        },
        setLibrary: function (universityName) {
            _gaq.push(['_setCustomVar',
                         1,                   // This custom var is set to slot #1.  Required parameter.
                         'Univeristy',     // The name acts as a kind of category for the user activity.  Required parameter.
                         universityName,    // This value of the custom variable.  Required parameter.
                         2                   // Sets the scope to session-level.  Optional parameter.
            ]);
        }
    };
})(cd = window.cd || {});
function TrackTiming(category, variable, opt_label) {
    this.category = category;
    this.variable = variable;
    this.label = opt_label ? opt_label : undefined;
    this.startTime;
    this.endTime;
    return this;
}

TrackTiming.prototype.startTime = function () {
    this.startTime = new Date().getTime();
    return this;
};

TrackTiming.prototype.endTime = function () {
    this.endTime = new Date().getTime();
    return this;
};

TrackTiming.prototype.send = function () {
    var timeSpent = this.endTime - this.startTime;
    try {
        window._gaq.push(['_trackTiming', this.category, this.variable, timeSpent, this.label]);
    } catch (e) {

    }
    return this;
};



(function (cd, $) {
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
    //var uvOptions = {};
    var isNeedRegistered = true;
    function registerFacebook() {
        if (isNeedRegistered) {
            isNeedRegistered = false;
            window.fbAsyncInit = function () {
                FB.init({
                    appId: '450314258355338',
                    status: true,
                    cookie: true,
                    xfbml: true,
                    oauth: true
                });
                cd.pubsub.publish('fbInit');
            };

            (function (d) {
                var js, id = 'facebook-jssdk';
                if (d.getElementById(id)) {
                    return;
                }
                js = d.createElement('script');
                js.id = id;
                js.async = true;
                js.src = "//connect.facebook.net/en_US/all.js";
                d.getElementsByTagName('head')[0].appendChild(js);
            }(document));
        }
    }

    function registerTwitter() {
        !function (d, s, id) {
            var js, fjs = d.getElementsByTagName(s)[0], p = /^http:/.test(d.location) ? 'http' : 'https'; if (!d.getElementById(id)) {
                js = d.createElement(s);
                js.id = id; js.src = p + "://platform.twitter.com/widgets.js";
                fjs.parentNode.insertBefore(js, fjs);
            }
        }
        (document, "script", "twitter-wjs");
    }

    var dropBoxisNeedRegistered = true;
    function registerDropBox() {
        if (dropBoxisNeedRegistered) {
            dropBoxisNeedRegistered = false;
            var js = document.createElement('script');
            js.id = "dropboxjs";
            js.setAttribute('data-app-key', 'gppwajedn90rv81');
            js.src = "https://www.dropbox.com/static/api/1/dropins.js";
            document.getElementsByTagName('head')[0].appendChild(js);
        }
    }

    function registerGoogleDrive() {
        var deffered = $.Deferred();
        var js = document.createElement('script');
        js.id = "googleDrive";
        js.src = "https://apis.google.com/js/api.js";
        document.getElementsByTagName('head')[0].appendChild(js);

        var interval = window.setInterval(function () {
            if (window.gapi !== undefined && window.gapi) {
                gapi.load('picker', {
                    'callback': function () {
                        deffered.resolve();
                    }
                });
                window.clearInterval(interval);
            }
        }, 50);
        return deffered;


    }
    function registerGoogleContact() {
        var deffered = $.Deferred();
        var js = document.createElement('script');
        js.id = "googleContact";
        js.src = " https://apis.google.com/js/client.js";
        document.getElementsByTagName('head')[0].appendChild(js);


        var clientId = '616796621727-o9vr11gtr5p9v2t18co7f7kjuu0plnum.apps.googleusercontent.com';
        apiKey = 'AIzaSyBqnR38dm9S2E-eQWRj-cTgup2kGA7lmlg',
        scopes = 'https://www.google.com/m8/feeds/contacts/default/full',
        //handleClientLoad : function() {
        //    gapi.client.setApiKey(this.apiKey);            
        //},
        checkAuth = function (isImmediate, callback) {
            gapi.auth.authorize({ client_id: clientId, scope: scopes, immediate: isImmediate }, callback);

        };

        var interval = window.setInterval(function () {
            if (window.gapi !== undefined && window.gapi.client !== undefined) {
                gapi.client.setApiKey(apiKey);
                deffered.resolve();

                //gapi.load('picker', {
                //    'callback': function () {
                //        deffered.resolve();
                //    }
                //});
                window.clearInterval(interval);
            }
        }, 50);
        return { deffred: deffered, checkAuth: checkAuth };

    }

    //function registerSkyDrive() {
    //    var df = new $.Deferred();
    //    if (document.getElementById('skyDrive')) {
    //        df.resolve();
    //        return df;
    //    }
    //    var js = document.createElement('script');
    //    js.id = "skyDrive";
    //    js.src = "//js.live.net/v5.0/wl.js";
    //    document.getElementsByTagName('head')[0].appendChild(js);
    //    var interval = window.setInterval(function () {
    //        if (window.WL !== undefined) {

    //            WL.init({
    //                client_id: '000000004C0FF9C4',
    //                redirect_uri: location.href,
    //                scope: "wl.basic",
    //                response_type: "code"
    //            });

    //            //WL.ui({
    //            //    name: "skydrivepicker",
    //            //    element: "up_Sky",
    //            //    mode: "open",
    //            //    select: "multi",
    //            //    onselected: function () { }
    //            //    //onerror: onDownloadFileError
    //            //});

    //            //WL.f
    //            window.clearInterval(interval);
    //            df.resolve();

    //        }
    //    }, 50);
    //    return df;
    //}  
 

    cd.loader = {
        async_load: async_load,
        registerFacebook: registerFacebook,
        // registerGoogle: registerGoogle,
        registerTwitter: registerTwitter,
        registerDropBox: registerDropBox,
        registerGoogleDrive: registerGoogleDrive,
        registerGoogleContact: registerGoogleContact
        //registerSkyDrive: registerSkyDrive
    };

})(cd = window.cd || {}, jQuery);