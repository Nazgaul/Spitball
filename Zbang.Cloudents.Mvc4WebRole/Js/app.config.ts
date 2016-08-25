declare var Modernizr: any;
module app {
    "use strict";

    angular.module('app').config(config);
    config.$inject = ["$controllerProvider", "$locationProvider", "$provide",
        "$httpProvider", "$compileProvider", "$animateProvider"];
    // ReSharper disable once Class
    function config(
        $controllerProvider: angular.IControllerProvider,
        $locationProvider: angular.ILocationProvider,
        $provide,
        $httpProvider: angular.IHttpProvider,
        $compileProvider: angular.ICompileProvider,
        $animateProvider: angular.animate.IAnimateProvider) {

        $controllerProvider.allowGlobals();
        $httpProvider.defaults.headers.common["X-Requested-With"] = 'XMLHttpRequest';
        $provide.factory('requestinterceptor', [() => ({
            'request'(c) {
                return c;
            },
            // optional method
            'response'(response) {
                return response;
            },
            'responseError'(response) {
                switch (response.status) {

                    case 400:
                    case 412:
                        alert('Spitball has updated, refreshing page');
                        window.location.reload(true);
                        break;
                    case 401:
                        //case 403:
                        window.open('/', '_self');
                        break;
                    case 403:

                        window.open(`/error/membersonly/?returnUrl=${encodeURIComponent(location.href)}`, '_self');
                        break;
                    case 404:
                        window.open('/error/notfound/', '_self');
                        break;
                    case 500:
                        window.open('/error/', '_self');
                        break;
                    default:
                        // somehow firefox in incognito crash and transfer to error page
                        //   window.open('/error/', '_self');
                        break;

                }

                return response;
            }
        })]);
        $httpProvider.interceptors.push('requestinterceptor');
        $compileProvider.debugInfoEnabled(false);
        $animateProvider.classNameFilter(/angular-animate/);
        $provide.constant('$MD_THEME_CSS', '');
    }
}





(function () {
    angular.module('app').config(config);
    config.$inject = ['AnalyticsProvider'];

    function config(analyticsProvider) {

        var analyticsObj = {
            'siteSpeedSampleRate': 70,
            'cookieDomain': 'spitball.co',
            'alwaysSendReferrer': true
        }
        if (window['id'] && window['id'] > 0) {
            analyticsObj['userId'] = window['id'];
        }
        analyticsProvider.setAccount({
            tracker: 'UA-9850006-3',
            fields: analyticsObj
            //set: {
            //    'dimension1': data.universityName || null,
            //    'dimension2': data.universityCountry || null,
            //    'dimension3': data.id || null
            //}
        });
        analyticsProvider.trackUrlParams(true);
        analyticsProvider.setPageEvent('$stateChangeSuccess');
        analyticsProvider.delayScriptTag(true);
        //AnalyticsProvider.setDomainName('XXX');


    }

    angular.module('app').run(anylticsRun);
    anylticsRun.$inject = ['Analytics', '$document'];

    function anylticsRun(analytics, $document) {
        $document.ready(function () {
            analytics.createAnalyticsScriptTag();
        });
        //for run the app
    };

})();



(function () {
    angular.module('app').run(config);
    config.$inject = ['timeAgo'];

    function config(timeAgo) {
        if (document.documentElement.lang === 'he') {
            timeAgo.settings.overrideLang = 'he_IL';
        }
        var threeDays = 60 * 60 * 24 * 3;
        timeAgo.settings.fullDateAfterSeconds = threeDays;
        timeAgo.settings.refreshMillis = 15000;
    }
})();

(function () {
    angular.module('app').config(config);
    config.$inject = ['ScrollBarsProvider'];

    function config(ScrollBarsProvider) {
        // the following settings are defined for all scrollbars unless the
        // scrollbar has local scope configuration
        ScrollBarsProvider.defaults = {
            //setHeight:500,
            //scrollButtons: {
            //    scrollAmount: 'auto', // scroll amount when button pressed
            //    enable: true // enable scrolling buttons by default
            //},
            scrollInertia: 400, // adjust however you want
            //axis: 'yx', // enable 2 axis scrollbars by default,
            scrollButtons: false,
            theme: 'dark-thin',
            autoHideScrollbar: true
        };
    };
})();


