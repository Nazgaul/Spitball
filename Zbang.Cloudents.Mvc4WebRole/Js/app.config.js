(function () {
    "use strict";

    angular.module('app').config(config);

    config.$inject = ['$controllerProvider', '$locationProvider', '$provide', '$httpProvider', 'CacheFactoryProvider'];

    function config($controllerProvider, $locationProvider, $provide, $httpProvider, cacheFactoryProvider) {
        //$locationProvider.html5Mode(true).hashPrefix('!');
        $controllerProvider.allowGlobals();
        angular.extend(cacheFactoryProvider.defaults, {
            deleteOnExpire: 'aggressive',
            maxAge: 45000, //45 seconds
            recycleFreq: 15000, // 15 seconds
            storageMode: 'sessionStorage',
            storagePrefix: 'sb.c.'
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
                            //case 403:
                            window.open('/', '_self');
                            break;
                        case 403:

                            window.open('/error/membersonly/?returnUrl=' + encodeURIComponent(location.href), '_self');
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
    config.$inject = ['AnalyticsProvider', 'DoubleClickProvider'];
    function config(analyticsProvider, doubleClickProvider) {

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
        analyticsProvider.trackUrlParams(true);
        analyticsProvider.setPageEvent('$stateChangeSuccess');
        //AnalyticsProvider.setDomainName('XXX');

        var matanTest2Code = 'div-gpt-ad-1459256517867-0';      //top of dashboard
        var testWithAsafCode = 'div-gpt-ad-1459255888335-0';    // for menu ad (outside box)
        var box250X250AtfCode = 'div-gpt-ad-1459261777271-0';    // for menu ad (in box)
        var search728X90AtfCode = 'div-gpt-ad-1459258380236-0'; //top of search
        var item728X90AtfCode = 'div-gpt-ad-1459258538511-0';   //top of item
        var item160X300SideCode = 'div-gpt-ad-1459259151004-0'; //side of item
        var item728X250Utf = 'div-gpt-ad-1459259529611-0';      //after first item's page

        doubleClickProvider.defineSlot('/107474526/Matan_Test2', [[300, 75], [468, 60], [728, 90]], matanTest2Code)
                           .defineSlot('/107474526/Test_With_Asaf', [[250, 250], [234, 60]], testWithAsafCode)
                           .defineSlot('/107474526/Box_250x250_ATF', [[250, 250], [234, 60]], box250X250AtfCode)
                           .defineSlot('/107474526/search_728x90_ATF', [[300, 75], [468, 60], [728, 90]], search728X90AtfCode)
                           .defineSlot('/107474526/Item_728x90_ATF', [[300, 75], [468, 60], [728, 90]], item728X90AtfCode)
                           .defineSlot('/107474526/Item_728x250_UTF', [[300, 75], [468, 60], [728, 90]], item728X250Utf)
                           .defineSlot('/107474526/Item_160x300_Side', [[160, 600]], item160X300SideCode);

        doubleClickProvider.defineSizeMapping(matanTest2Code)
            .addSize([1050, 768], [728, 90])
            .addSize([640, 480], [468, 60])
           // .addSize([500, 350], [480, 320])
            //// Fits browsers of any size smaller than 640 x 480
            .addSize([0, 0], [300, 75]);

        doubleClickProvider.defineSizeMapping(testWithAsafCode)
            .addSize([1050, 850], [250, 250])
            .addSize([0, 0], [234, 60]);

        doubleClickProvider.defineSizeMapping(box250X250AtfCode)
            .addSize([1050, 850], [250, 250])
            .addSize([0, 0], [234, 60]);

        doubleClickProvider.defineSizeMapping(search728X90AtfCode)
            .addSize([1050, 768], [728, 90])
            .addSize([640, 480], [468, 60])
            .addSize([0, 0], [300, 75]);

        doubleClickProvider.defineSizeMapping(item728X90AtfCode)
            .addSize([1050, 768], [728, 90])
            .addSize([640, 480], [468, 60])
            .addSize([0, 0], [300, 75]);

        doubleClickProvider.defineSizeMapping(item728X250Utf)
            .addSize([1050, 768], [728, 90])
            .addSize([640, 480], [468, 60])
            .addSize([0, 0], [300, 75]);

        doubleClickProvider.defineSizeMapping(item160X300SideCode)
            .addSize([0, 0], [160, 600]);

        doubleClickProvider.setRefreshInterval(30000);


    }

    angular.module('app').run(anylticsRun);
    anylticsRun.$inject = ['Analytics'];
    function anylticsRun(analytics) {
        analytics.createAnalyticsScriptTag();
        //for run the app
    };

})()