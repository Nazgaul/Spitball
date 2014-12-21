/*global angular: true, appInsights: true */


(function () {
    "use strict";

    angular.module('angular-appinsights', [])

        .provider('insights', function () {

            var _appId,
                _appName;

            this.start = function (appId, appName) {

                _appId = appId;
                _appName = appName || '(Application Root)';

                //if (appInsights && appId) {
                //  //  appInsights.start(appId);
                //}

            };

            function Insights () {

                var _logEvent = function (event, properties, property) {

                    if (appInsights && _appId) {
                        appInsights.trackEvent(event, properties, property);
                    }

                },

                _logPageView = function (page) {

                    if (appInsights && _appId) {
                        appInsights.trackPageView(page);
                    }

                };

                return {
                    'logEvent': _logEvent,
                    'logPageView': _logPageView,
                    'appName': _appName
                };

            }

            this.$get = function() {
                return new Insights();
            };

        })

        .run(['$rootScope','$route','$location','insights',function($rootScope, $route, $location, insights) {
            $rootScope.$on('$routeChangeSuccess', function() {

                var pagePath;
                try {
                    pagePath = $location.path().substr(1);
                    pagePath =  insights.appName + '/' + pagePath;
                }
                finally {
                    insights.logPageView(pagePath);
                }
            });
        }]);

}());
