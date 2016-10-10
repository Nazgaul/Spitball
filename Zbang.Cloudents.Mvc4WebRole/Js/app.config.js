var app;
(function (app) {
    "use strict";
    angular.module("app").config(config);
    config.$inject = ["$controllerProvider", "$locationProvider", "$provide",
        "$httpProvider", "$compileProvider", "$animateProvider",
        "$mdAriaProvider", "$mdIconProvider", "$sceDelegateProvider", "$mdThemingProvider"];
    function config($controllerProvider, $locationProvider, $provide, $httpProvider, $compileProvider, $animateProvider, $mdAriaProvider, $mdIconProvider, $sceDelegateProvider, $mdThemingProvider) {
        $controllerProvider.allowGlobals();
        $httpProvider.defaults.headers.common["X-Requested-With"] = 'XMLHttpRequest';
        $provide.factory("requestinterceptor", [function () { return ({
                "request": function (c) {
                    return c;
                },
                "response": function (response) {
                    return response;
                },
                "responseError": function (response) {
                    switch (response.status) {
                        case 400:
                        case 412:
                            alert("Spitball has updated, refreshing page");
                            window.location.reload(true);
                            break;
                        case 401:
                            window.open("/", "_self");
                            break;
                        case 403:
                            window.open("/error/membersonly/?returnUrl=" + encodeURIComponent(location.href), "_self");
                            break;
                        case 404:
                            window.open("/error/notfound/", "_self");
                            break;
                        case 500:
                            window.open("/error/", "_self");
                            break;
                        default:
                            break;
                    }
                    return response;
                }
            }); }]);
        $httpProvider.interceptors.push("requestinterceptor");
        $compileProvider.debugInfoEnabled(false);
        $animateProvider.classNameFilter(/angular-animate/);
        $mdThemingProvider.disableTheming();
        $mdAriaProvider.disableWarnings();
        $sceDelegateProvider.resourceUrlWhitelist([
            "self",
            (window["cdnPath"] + "/**")
        ]);
        $mdIconProvider
            .iconSet("t", append("/images/site/icons.svg"))
            .iconSet("i", append("/images/site/itemIcons.svg"))
            .iconSet("u", append("/images/site/uploadIcons.svg"))
            .iconSet("lc", append("/images/site/libChooseIcons.svg"))
            .iconSet("b", append("/images/site/box-icons.svg"))
            .iconSet("q", append("/images/site/quizIcons.svg"))
            .iconSet("p", append("/images/site/profileIcons.svg"));
        function append(str) {
            return (window["cdnPath"] || "") + str + "?" + window["version"];
        }
    }
})(app || (app = {}));
(function () {
    angular.module("app").config(config);
    config.$inject = ['AnalyticsProvider'];
    function config(analyticsProvider) {
        var analyticsObj = {
            'cookieDomain': 'spitball.co',
            'alwaysSendReferrer': true
        };
        if (window['id'] && window['id'] > 0) {
            analyticsObj['userId'] = window['id'];
        }
        analyticsProvider.setAccount({
            tracker: 'UA-9850006-3',
            fields: analyticsObj
        });
        analyticsProvider.trackUrlParams(true);
        analyticsProvider.setPageEvent("$stateChangeSuccess");
        analyticsProvider.delayScriptTag(true);
    }
    angular.module("app").run(anylticsRun);
    anylticsRun.$inject = ["Analytics", "$document"];
    function anylticsRun(analytics, $document) {
        $document.ready(function () {
            analytics.createAnalyticsScriptTag();
        });
    }
    ;
})();
(function () {
    angular.module("app").run(config);
    config.$inject = ["timeAgo"];
    function config(timeAgo) {
        if (document.documentElement.lang === "he") {
            timeAgo.settings.overrideLang = "he_IL";
        }
        var threeDays = 60 * 60 * 24 * 3;
        timeAgo.settings.fullDateAfterSeconds = threeDays;
        timeAgo.settings.refreshMillis = 15000;
    }
})();
(function () {
    angular.module("app").config(config);
    config.$inject = ['ScrollBarsProvider'];
    function config(ScrollBarsProvider) {
        ScrollBarsProvider.defaults = {
            scrollInertia: 400,
            scrollButtons: false,
            theme: "dark-thin",
            autoHideScrollbar: true
        };
    }
    ;
})();
