﻿module app {
    "use strict";

    angular.module("app").config(config);
    config.$inject = ["$controllerProvider",  "$provide",
        "$httpProvider", "$compileProvider", "$animateProvider",
        "$mdAriaProvider", "$mdIconProvider", "$sceDelegateProvider",
        "$mdThemingProvider", "$urlMatcherFactoryProvider"];
    // ReSharper disable once Class
    function config(
        $controllerProvider: angular.IControllerProvider,
        $provide,
        $httpProvider: angular.IHttpProvider,
        $compileProvider: angular.ICompileProvider,
        $animateProvider: angular.animate.IAnimateProvider,
        $mdAriaProvider,
        $mdIconProvider: angular.material.IIconProvider,
        $sceDelegateProvider: angular.ISCEDelegateProvider,
        $mdThemingProvider/*: angular.material.IThemingProvider*/,
        $urlMatcherFactoryProvider: angular.ui.IUrlMatcherFactory
    ) {

        $controllerProvider.allowGlobals();
        $httpProvider.defaults.headers.common["X-Requested-With"] = 'XMLHttpRequest';
        $provide.factory("requestinterceptor", [() => ({
            "request"(c) {
                return c;
            },
            // optional method
            "response"(response) {
                return response;
            },
            "responseError"(response: angular.IHttpPromiseCallbackArg<any>) {
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

                        window.open(`/error/membersonly/?returnUrl=${encodeURIComponent(location.href)}`, "_self");
                        break;
                    case 404:
                        window.open("/error/notfound/", "_self");
                        break;
                    case 500:
                        window.open("/error/", "_self");
                        break;
                    default:
                        // somehow firefox in incognito crash and transfer to error page
                        //   window.open('/error/', '_self');
                        break;

                }

                return response;
            }
        })]);
        $httpProvider.interceptors.push("requestinterceptor");
        $compileProvider.debugInfoEnabled(false);
        $compileProvider.aHrefSanitizationWhitelist(/^\s*(https?|whatsapp):/);
        $animateProvider.classNameFilter(/angular-animate/);
        $mdThemingProvider.disableTheming();
        //$provide.constant("$MD_THEME_CSS", "");

        $mdAriaProvider.disableWarnings();

        $sceDelegateProvider.resourceUrlWhitelist([
            // allow same origin resource loads.
            "self",
            // allow loading from our assets domain.  Notice the difference between * and **.
            `${window["cdnPath"]}/**`
        ]);

        $mdIconProvider
            .iconSet("t", append("/images/site/icons.svg"))
            .iconSet("i", append("/images/site/itemIcons.svg"))
            .iconSet("u", append("/images/site/uploadIcons.svg"))
            .iconSet("lc", append("/images/site/libChooseIcons.svg"))
            .iconSet("b", append("/images/site/box-icons.svg"))
            .iconSet("q", append("/images/site/quizIcons.svg"))
            .iconSet("p", append("/images/site/profileIcons.svg"))
            .iconSet("s", append("/images/site/social-icons.svg"));


        function append(str: string) {
            return (window["cdnPath"] || "") + str + "?" + window["version"];
        }

        $urlMatcherFactoryProvider.strictMode(true);
        $urlMatcherFactoryProvider.type('encodeStr', {
            encode: (item: string) => {
                if (!item) {
                    return item;
                }
                item = item.replace(/[<>*%&:\\/;?@=+$,{}|^[\]`"#'()]/g, "");
                item = item.replace(/[ _-]/g, "-");
                item = item.replace(/-{2,999}/g, "-");
                return item.toLowerCase();
            },
            decode: item => {
                // Look up the list item by index
                return item;//list[parseInt(item, 10)];
            },
            is: val => {
                return (val == null || typeof val === "string");
            },
            pattern: /[^/]*/
        });
        $urlMatcherFactoryProvider.type('encodeGuid', {
            encode: (item: string) => {
                var enc = Guid.toBase64(item);
                enc = enc.replace("/", "_").replace("+", "-");
                return enc.substring(0, 22);// list.indexOf(item);
            },
            decode: item => {
                var enc = item.replace("_", "/").replace("-", "+");
                return Guid.fromBase64(enc + "==");
                //return str;//list[parseInt(item, 10)];
            },
            is: val => {

                return val == null || (typeof val === "string");// && val.match(/-/g).length === 4);
            },
            pattern: /[^/]*/
        });

        //$stateProvider.state('list', {
        //    url: "/list/{item:listItem}",
        //    controller: function ($scope, $stateParams) {
        //        console.log($stateParams.item);
        //    }
        //});
    }
}





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

    function anylticsRun(analytics: IAnalytics, $document: angular.IDocumentService) {
        $document.ready(() => {
            analytics.createAnalyticsScriptTag();
        });
        //for run the app
    };

})();



(() => {
    angular.module("app").run(config);
    config.$inject = ["timeAgo"];

    function config(timeAgo) {
        if (document.documentElement.lang === "he") {
            timeAgo.settings.overrideLang = "he_IL";
        }
        const threeDays = 60 * 60 * 24 * 3;
        timeAgo.settings.fullDateAfterSeconds = threeDays;
        timeAgo.settings.refreshMillis = 15000;
    }
})();

(() => {
    angular.module("app").config(config);
    config.$inject = ['ScrollBarsProvider'];

    function config(ScrollBarsProvider) {
        // the following settings are defined for all scrollbars unless the
        // scrollbar has local scope configuration
        ScrollBarsProvider.defaults = {

            scrollInertia: 400, // adjust however you want
            scrollButtons: false,
            theme: "dark-thin",
            autoHideScrollbar: true
        };
    };
})();


