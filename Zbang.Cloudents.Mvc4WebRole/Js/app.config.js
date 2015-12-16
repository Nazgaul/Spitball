﻿(function () {
    "use strict";

    angular.module('app').config(config);

    config.$inject = ['$controllerProvider', '$locationProvider', '$angularCacheFactoryProvider', '$provide', '$httpProvider'];

    function config($controllerProvider, $locationProvider, $angularCacheFactoryProvider, $provide, $httpProvider) {
        //$locationProvider.html5Mode(true).hashPrefix('!');
        $controllerProvider.allowGlobals();
        $angularCacheFactoryProvider.setCacheDefaults({
            maxAge: 45000, //45 seconds
            deleteOnExpire: 'aggressive',
            recycleFreq: 45000,
            cacheFlushInterval: 45000,
            storageMode: 'sessionStorage'
        });
        $httpProvider.defaults.headers.common["X-Requested-With"] = 'XMLHttpRequest';
        //deep-purple
        //$mdThemingProvider.setDefaultTheme('deep-purple');
        //$mdBiDirectionalProvider.rtlMode(true);
        $provide.factory('requestinterceptor', [function () {

            return {
                'request': function (c) {
                    return c;
                },
                // optional method
                'response': function (response) {
                    // do something on success
                    //switch (response.status) {
                    //    case 200:
                    //        return response;
                    //        break;
                    //}

                    return response;
                },
                'responseError': function (response) {
                    switch (response.status) {

                        case 400:
                        case 412:
                            alert('Spitball has updated, refreshing page');
                            window.location.reload(true);
                            break;
                        case 401:
                        case 403:
                            window.open('/', '_self');
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
            };
        }]);
        $httpProvider.interceptors.push('requestinterceptor');



    }

})();


(function () {
    angular.module('textAngular').config(config);
    config.$inject = ['$provide'];
    function config($provide) {
        $provide.decorator('taOptions', ['taRegisterTool', '$delegate', '$q', '$stateParams',
               function (taRegisterTool, taOptions, $q, $stateParams) {
                   var buttons;

                   buttons = [['fontUp', 'fontDown'],
                       ['bold', 'italics', 'underline'],
                       ['justifyLeft', 'justifyCenter', 'justifyRight'],
                       ['ol', 'ul'],
                       ['insertImage'],
                       ['redo', 'undo']];

                   if (Modernizr.inputtypes.color) {
                       buttons[1].push('color');
                   }
                   taOptions.toolbar = buttons;


                   taOptions.defaultFileDropHandler = function (file, insertAction) {
                       var dfd = $q.defer();
                       var client = new XMLHttpRequest();
                       client.onreadystatechange = function () {
                           if (client.readyState == 4 && client.status == 200) {
                               var response = JSON.parse(client.response);
                               if (!response.success) {
                                   alert('Error');
                                   return;
                               }
                               insertAction('insertImage', response.payload, true);
                               dfd.resolve();
                           }
                       }

                       var formData = new FormData();
                       formData.append(file.name, file);
                       formData.append("boxId", $stateParams.boxId);
                       client.open("POST", "/upload/quizimage/", true);
                       client.send(formData);

                       return dfd.promise;
                   };
                   return taOptions;
               }]);

        //function createSvgDisplay(svgNamge) {
        //    return '<button type="button" class="btn btn-default"> \
        //<svg><use xmlns:xlink="http://www.w3.org/1999/xlink" xlink:href="/images/textEditor.svg?26.0.0#' + svgNamge + '"></use></svg> \
        //</button>';
        //}

        //$provide.decorator('taTools', ['$delegate', function (taTools) {
        //    delete taTools.bold.iconclass;
        //    taTools.bold.display = createSvgDisplay('bold');
        //    return taTools;
        //}]);

    }
})();


(function () {
    angular.module('app').config(config);
    config.$inject = ['AnalyticsProvider'];
    function config(analyticsProvider) {

        var analyticsObj = {
            'siteSpeedSampleRate': 70,
            'cookieDomain': 'spitball.co',
            'alwaysSendReferrer': true
        }
        if (window.id && window.id > 0) {
            analyticsObj.userId = window.id;
        }
        analyticsProvider.setAccount({
            tracker: 'UA-9850006-3',
            fields: analyticsObj,
            //set: {
            //    'dimension1': data.universityName || null,
            //    'dimension2': data.universityCountry || null,
            //    'dimension3': data.id || null
            //}
        });
        analyticsProvider.setPageEvent('$stateChangeSuccess');
        //AnalyticsProvider.setDomainName('XXX');
    }

    angular.module('app').run(anylticsRun);
    anylticsRun.$inject = ['Analytics'];
    function anylticsRun(analytics) {
        analytics.createAnalyticsScriptTag();
        //for run the app
    };

})()