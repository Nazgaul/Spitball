
(function (cd) {
    (function (i, s, o, g, r, a, m) {
        i['GoogleAnalyticsObject'] = r; i[r] = i[r] || function () {
            (i[r].q = i[r].q || []).push(arguments)
        }, i[r].l = 1 * new Date(); a = s.createElement(o),
        m = s.getElementsByTagName(o)[0]; a.async = 1; a.src = g; m.parentNode.insertBefore(a, m)
    })(window, document, 'script', '//www.google-analytics.com/analytics.js', 'ga');

    ga('create', 'UA-9850006-3', {
        'userId': cd.userDetail().id,
        'siteSpeedSampleRate': 70,
        'cookieDomain': 'cloudents.com'
    });

    
    ga('require', 'displayfeatures');
   // ga('send', 'pageview');

    //Obselete
    cd.analytics = {
        //done
        trackEvent: function (category, action, opt_label) {
            window.ga('send', 'event', category, action, opt_label);  // value is a number.
        },
        //we need this - we need directive
        trackSocial: function (targetUrl, action) {
            window.ga('send', 'social', 'facebook', action, targetUrl);
        },
        trackPage: function (targetUrl) {
            window.ga('send', 'pageview', targetUrl);
        },
        //we need this - we need in controller
        setLibrary: function (universityName) {
            ga('set', 'dimension1', universityName);
            
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
        window.ga('send', 'timing', this.category, this.variable, timeSpent, this.label);
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
