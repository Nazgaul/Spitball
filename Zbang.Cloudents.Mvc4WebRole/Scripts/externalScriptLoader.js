﻿var _gaq = _gaq || [];
//.getAttribute('data-id')

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
    ga('send', 'pageview');

    cd.analytics = {
        trackEvent: function (category, action, opt_label) {
            //_gaq.push(['_trackEvent', category, action, opt_label]);
            window.ga('send', 'event', category, action, opt_label);  // value is a number.
        },
        trackSocial: function (targetUrl, action) {
            //_gaq.push(['_trackSocial', 'facebook', action, targetUrl]);
            window.ga('send', 'social', 'facebook', action, targetUrl);
        },
        trackPage: function (targetUrl) {
           // _gaq.push(['_trackPageview', targetUrl]);
            window.ga('send', 'pageview', targetUrl);
        },
        setLibrary: function (universityName) {
            ga('set', 'dimension1', universityName);
            //_gaq.push(['_setCustomVar',
            //             1,                   // This custom var is set to slot #1.  Required parameter.
            //             'University',     // The name acts as a kind of category for the user activity.  Required parameter.
            //             universityName,    // This value of the custom variable.  Required parameter.
            //             2                   // Sets the scope to session-level.  Optional parameter.
            //]);
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
       // window._gaq.push(['_trackTiming', this.category, this.variable, timeSpent, this.label]);
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

//"WeakMap" in this || (function (context) { //**********For mutation observer***********//

//    // (C) WebReflection - Mit Style License

//    // WeakMap(void):WeakMap
//    function WeakMap() {

//        // private references holders
//        var
//          keys = [],
//          values = []
//        ;

//        // WeakMap#delete(key:void*):void
//        function del(key) {
//            if (has(key)) {
//                keys.splice(i, 1);
//                values.splice(i, 1);
//            }
//            return -1 < i;
//        }

//        // WeakMap#get(key:void*[, d3fault:void*]):void*
//        function get(key, d3fault) {
//            return has(key) ? values[i] : d3fault;
//        }

//        // WeakMap#has(key:void*):boolean
//        function has(key) {
//            if (key !== Object(key))
//                throw new TypeError("not a non-null object")
//            ;
//            i = indexOf.call(keys, key);
//            return -1 < i;
//        }

//        // WeakMap#has(key:void*, value:void*):void
//        function set(key, value) {
//            has(key) ?
//              values[i] = value
//              :
//              values[keys.push(key) - 1] = value
//            ;
//        }

//        // returns freshly new created
//        // instanceof WeakMap in any case
//        return create(WeakMapPrototype, {
//            "delete": { value: del },
//            get: { value: get },
//            has: { value: has },
//            set: { value: set }
//        });

//    }

//    // need for an empty constructor ...
//    function WeakMapInstance() { }  // GC'ed if !!Object.create
//    // ... so that new WeakMapInstance and new WeakMap
//    // produces both an instanceof WeakMap

//    var
//      // shortcuts and ...
//      Object = context.Object,
//      WeakMapPrototype = WeakMap.prototype,

//      // partial polyfill for this aim only
//      create = Object.create || function create(proto, descriptor) {
//          // partial ad-hoc Object.create shim if not available
//          var object = new WeakMapInstance;
//          object["delete"] = descriptor["delete"].value;
//          object.get = descriptor.get.value;
//          object.has = descriptor.has.value;
//          object.set = descriptor.set.value;
//          return object;
//      },

//      indexOf = [].indexOf || function indexOf(value) {
//          // partial fast Array#indexOf polyfill if not available
//          for (i = this.length; i-- && this[i] !== value;);
//          return i;
//      },

//      i // recycle ALL the variables !
//    ;

//    // used to follow FF behavior where WeakMap.prototype is a WeakMap itself
//    WeakMap.prototype = WeakMapInstance.prototype = WeakMapPrototype = WeakMap();

//    // assign it to the global context
//    "defineProperty" in Object ?
//      Object.defineProperty(context, "WeakMap", { value: WeakMap })
//      :
//      context.WeakMap = WeakMap
//    ;

//    // that's pretty much it in less than 400bytes minzipped

//}(this));