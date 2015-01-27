/**
 * @license Angulartics v0.17.2
 * (c) 2013 Luis Farzati http://luisfarzati.github.io/angulartics
 * Universal Analytics update contributed by http://github.com/willmcclellan
 * License: MIT
 */
(function (angular) {
    'use strict';

    /**
     * @ngdoc overview
     * @name angulartics.google.analytics
     * Enables analytics support for Google Analytics (http://google.com/analytics)
     */
    angular.module('angulartics.google.analytics', ['angulartics'])
    .config(['$analyticsProvider', function ($analyticsProvider) {

        // GA already supports buffered invocations so we don't need
        // to wrap these inside angulartics.waitForVendorApi

        $analyticsProvider.settings.trackRelativePath = true;

        // Set the default settings for this module
        $analyticsProvider.settings.ga = {
            // array of additional account names (only works for analyticsjs)
            additionalAccountNames: undefined
        };

        $analyticsProvider.registerPageTrack(function (path) {
            setTimeout(function () {
                if (window._gaq) _gaq.push(['_trackPageview', path]);
                if (window.ga) {
                    ga('send', 'pageview', path);
                    angular.forEach($analyticsProvider.settings.ga.additionalAccountNames, function (accountName) {
                        ga(accountName + '.send', 'pageview', path);
                    });
                }

            }, 200);
        });

        /**
         * Track Event in GA
         * @name eventTrack
         *
         * @param {string} action Required 'action' (string) associated with the event
         * @param {object} properties Comprised of the mandatory field 'category' (string) and optional  fields 'label' (string), 'value' (integer) and 'noninteraction' (boolean)
         *
         * @link https://developers.google.com/analytics/devguides/collection/gajs/eventTrackerGuide#SettingUpEventTracking
         *
         * @link https://developers.google.com/analytics/devguides/collection/analyticsjs/events
         */
        $analyticsProvider.registerEventTrack(function (action, properties) {

            // do nothing if there is no category (it's required by GA)
            if (!properties || !properties.category) {
                return;
            }
            // GA requires that eventValue be an integer, see:
            // https://developers.google.com/analytics/devguides/collection/analyticsjs/field-reference#eventValue
            // https://github.com/luisfarzati/angulartics/issues/81
            if (properties.value) {
                var parsed = parseInt(properties.value, 10);
                properties.value = isNaN(parsed) ? 0 : parsed;
            }

            if (window.ga) {

                var eventOptions = {
                    eventCategory: properties.category || null,
                    eventAction: action || null,
                    eventLabel: properties.label || null,
                    eventValue: properties.value || null,
                    nonInteraction: properties.noninteraction || null
                };

                // add custom dimensions and metrics
                for (var idx = 1; idx <= 20; idx++) {
                    if (properties['dimension' + idx.toString()]) {
                        eventOptions['dimension' + idx.toString()] = properties['dimension' + idx.toString()];
                    }
                    if (properties['metric' + idx.toString()]) {
                        eventOptions['metric' + idx.toString()] = properties['metric' + idx.toString()];
                    }
                }
                ga('send', 'event', eventOptions);
                angular.forEach($analyticsProvider.settings.ga.additionalAccountNames, function (accountName) {
                    ga(accountName + '.send', 'event', eventOptions);
                });
            }

            else if (window._gaq) {
                _gaq.push(['_trackEvent', properties.category, action, properties.label, properties.value, properties.noninteraction]);
            }

        });

        $analyticsProvider.registerSocialTrack(function (action, properties) {
            if (properties.value) {
                var parsed = parseInt(properties.value, 10);
                properties.value = isNaN(parsed) ? 0 : parsed;
            }
            else if (window.ga) {
                if (properties.noninteraction) {
                    ga('send', 'social', properties.category, action, properties.label, properties.value, { nonInteraction: 1 });
                } else {
                    ga('send', 'social', properties.category, action, properties.label, properties.value);
                }
            }
        });

        $analyticsProvider.registerTimingTrack(function (properties) {
            properties.label = properties.label || undefined;
            if (window.ga) {
                window.ga('send', 'timing', properties.category, properties.variable, properties.timeSpent, properties.label);
            }
        });

        $analyticsProvider.registerSetVariable(function (variable, value) {
            ga('set', variable, value);
        });

        $analyticsProvider.registerSearchTrack(function (path, term, category) {
            if (path.indexOf('.') > -1) {
                path = path.split('.');
            }
            ga('send', 'pageview', '/' + path[1] + '/?q=' + encodeURIComponent(term) + '&t=' + category);
        });

    }]);
})(angular);




///**
// * @license Angulartics v0.16.3
// * (c) 2013 Luis Farzati http://luisfarzati.github.io/angulartics
// * Universal Analytics update contributed by http://github.com/willmcclellan
// * License: MIT
// */
//(function (angular) {
//    'use strict';

//    /**
//     * @ngdoc overview
//     * @name angulartics.google.analytics
//     * Enables analytics support for Google Analytics (http://google.com/analytics)
//     */
//    angular.module('angulartics.google.analytics', ['angulartics'])
//    .config(['$analyticsProvider', function ($analyticsProvider) {

//        // GA already supports buffered invocations so we don't need
//        // to wrap these inside angulartics.waitForVendorApi

//        $analyticsProvider.settings.trackRelativePath = true;

//        $analyticsProvider.registerPageTrack(function (path) {
//            if (window._gaq) _gaq.push(['_trackPageview', path]);
//            if (window.ga) ga('send', 'pageview', path);
//        });

//        /**
//         * Track Event in GA
//         * @name eventTrack
//         *
//         * @param {string} action Required 'action' (string) associated with the event
//         * @param {object} properties Comprised of the mandatory field 'category' (string) and optional  fields 'label' (string), 'value' (integer) and 'noninteraction' (boolean)
//         *
//         * @link https://developers.google.com/analytics/devguides/collection/gajs/eventTrackerGuide#SettingUpEventTracking
//         *
//         * @link https://developers.google.com/analytics/devguides/collection/analyticsjs/events
//         */
//        $analyticsProvider.registerEventTrack(function (action, properties) {
//            // GA requires that eventValue be an integer, see:
//            // https://developers.google.com/analytics/devguides/collection/analyticsjs/field-reference#eventValue
//            // https://github.com/luisfarzati/angulartics/issues/81
//            if (properties.value) {
//                var parsed = parseInt(properties.value, 10);
//                properties.value = isNaN(parsed) ? 0 : parsed;
//            }

//            if (window._gaq) {
//                _gaq.push(['_trackEvent', properties.category, action, properties.label, properties.value, properties.noninteraction]);
//            }
//            else if (window.ga) {
//                if (properties.noninteraction) {
//                    ga('send', 'event', properties.category, action, properties.label, properties.value, { nonInteraction: 1 });
//                } else {
//                    ga('send', 'event', properties.category, action, properties.label, properties.value);
//                }
//            }
//        });
     

//    }]);
//})(angular);
