var app;
(function (angular) {
    "use strict";
    var lang, version;
    app = angular.module('app', ['ngRoute', 'ngSanitize', 'infinite-scroll', 'custom_scrollbar',
        'monospaced.elastic', 'ngDragDrop', 'displayTime', 'textDirection', 'jmdobry.angular-cache',
       'ui.bootstrap', 'ngMessages', 'ngAnimate', 'mAccount', 'mDashboard', 'textAngular',
       'mBox', 'mItem', 'mLibrary', 'mQuiz', 'mUser', 'mSearch', 'debounce', 'angulartics',
       'angulartics.google.analytics']).
   config([
       '$routeProvider',
       '$locationProvider',
       '$httpProvider',
       '$tooltipProvider',
       '$provide',
       '$angularCacheFactoryProvider',
       '$compileProvider',
       function ($routeProvider, $locationProvider, $httpProvider, $tooltipProvider, $provide,
           $angularCacheFactoryProvider, $compileProvider) {
           $compileProvider.debugInfoEnabled(false);
           $httpProvider.defaults.headers.common["X-Requested-With"] = 'XMLHttpRequest';

           $angularCacheFactoryProvider.setCacheDefaults({
               maxAge: 45000, //45 seconds
               deleteOnExpire: 'aggressive',
               recycleFreq: 45000,
               cacheFlushInterval: 45000,
               storageMode: 'sessionStorage'
           });


           $provide.decorator('taOptions', ['taRegisterTool', '$delegate', '$q', '$routeParams', function (taRegisterTool, taOptions, $q, $routeParams) {
               var buttons;

               buttons = [['fontUp', 'fontDown'], ['bold', 'italics', 'underline'], ['justifyLeft', 'justifyCenter', 'justifyRight'], ['ol', 'ul'], ['insertImage'], ['redo', 'undo']];

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
                   formData.append("boxId", $routeParams.boxId);
                   client.open("POST", "/upload/quizimage/", true);
                   client.send(formData);

                   return dfd.promise;
               };
               //taOptions.classes = {
               //    focussed: 'focused',
               //    toolbar: 'btn-toolbar',
               //    toolbarGroup: 'btn-group',
               //    toolbarButton: 'btn btn-default',
               //    toolbarButtonActive: 'active',
               //    disabled: 'disabled',
               //    textEditor: 'form-control',
               //    htmlEditor: 'form-control'
               //};
               return taOptions;
           }]);

           $provide.factory('requestinterceptor', [function () {

               return {
                   'request': function (config) {
                       return config;
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
                               window.open('/account/', '_self');
                               break;
                           case 404:
                               window.open('/error/', '_self');
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

           $tooltipProvider.options({
               placement: 'bottom',
               animation: true,
               popupDelay: 500
               //appendToBody: true
           });

           $locationProvider.html5Mode(true).hashPrefix('!');


           var originalWhen = $routeProvider.when;


           $routeProvider.when = function (path, route) {
               route.resolve = {
                   currentUser: ['$q', 'sUserDetails', 'sNewUpdates', function ($q, sUserDetails, sNewUpdates) {
                       return sUserDetails.initDetails().then(sNewUpdates.loadUpdates).finally(function () {
                           lang = sUserDetails.getDetails().culture;
                       });
                   }]
               };

               return originalWhen.call($routeProvider, path, route);
           };
           //#region routes

           //#region cloudents
           var routes = [
                {
                    paths: [
                        { url: '/box/:uniName/:boxId/:boxName/', isPrivate: true },
                        { url: '/box/:uniName/:boxId/:boxName/#:tab?', isPrivate: true },
                        { url: '/course/:uniName/:boxId/:boxName/#:tab?', isPrivate: false },
                        { url: '/course/:uniName/:boxId/:boxName/', isPrivate: false }
                    ],
                    iterator: function (route) {
                        var obj = createRoute('box', '/box/indexpartial/', 'BoxCtrl', false);
                        obj.params.isPrivate = route.isPrivate;
                        obj.reloadOnSearch = false;
                        $routeProvider.when(route.url, obj);
                    }
                },
                {
                    paths: [
                        { url: '/account/settings' }
                    ],
                    iterator: function (route) {
                        var obj = createRoute('accountSettings', '/account/settingpartial/', 'AccountSettingsCtrl');
                        $routeProvider.when(route.url, obj);
                    }
                },
                {
                    paths: [
                        { url: '/account/' },
                        { url: '/account/:lang/' }
                    ],
                    iterator: function (route) {
                        var obj = createRoute('account', '/account/indexpartial/', 'AccountCtrl');
                        $routeProvider.when(route.url, obj);
                    }
                },
                {
                    paths: [
                        { url: '/dashboard/' }                      
                    ],
                    iterator: function (route) {
                        var obj = createRoute('dashboard', '/dashboard/indexpartial/', 'DashboardCtrl');
                        obj.reloadOnSearch = false;
                        $routeProvider.when(route.url, obj);
                    }
                },
                {
                    paths: [
                        { url: '/item/:uniName/:boxId/:boxName/:itemId/:itemName/' },
                        { url: '/item/:uniName/:boxId/:boxName/:itemId/:itemName/#fullscreen' }
                    ],
                    iterator: function (route) {
                        var obj = createRoute('item', '/item/indexpartial/', 'ItemCtrl', false);
                        $routeProvider.when(route.url, obj);
                    }
                },
                {
                    paths: [
                            { url: '/library/', type: 'library' },
                            { url: '/library/:libraryId/:libraryName/', type: 'department' }
                    ],
                    iterator: function (route) {
                        var obj = createRoute(route.type, '/library/indexpartial/', 'LibraryCtrl');
                        obj.reloadOnSearch = false;
                        $routeProvider.when(route.url, obj);
                    }
                },
                {
                    paths: [
                        { url: '/user/:userId/:userName/' }
                    ],
                    iterator: function (route) {
                        var obj = createRoute('user', '/user/indexpartial/', 'UserCtrl');
                        obj.reloadOnSearch = false;
                        $routeProvider.when(route.url, obj);
                    }
                },
                {
                    paths: [
                        { url: '/search/' }
                    ],
                    iterator: function (route) {
                        var obj = createRoute('search', '/search/indexpartial/', 'SearchCtrl', false);
                        $routeProvider.when(route.url, obj);
                    }
                },
                {
                    paths: [
                    { url: '/library/choose/' }
                    ],
                    iterator: function (route) {
                        var obj = createRoute('libraryChoose', '/library/choosepartial/', 'LibChooseCtrl');
                        $routeProvider.when(route.url, obj);
                    }
                },
                {
                    paths: [
                        { url: '/quiz/:uniName/:boxId/:boxName/:quizId/:quizName/' }
                    ],
                    iterator: function (route) {
                        var obj = createRoute('quiz', '/quiz/indexpartial/', 'QuizCtrl');
                        $routeProvider.when(route.url, obj);
                    }
                },
                {
                    paths: [
                        { url: '/box/:uniName/:boxId/:boxName/quizcreate/' },
                        { url: '/course/:uniName/:boxId/:boxName/quizcreate/' }
                    ],
                    iterator: function (route) {
                        var obj = createRoute('quizCreate', '/quiz/createpartial/', 'QuizCreateCtrl');
                        $routeProvider.when(route.url, obj);
                    }
                }
           ];

           angular.forEach(routes, function (route) {
               angular.forEach(route.paths, route.iterator);
           });

           function createRoute(type, templateUrl, controller, reloadOnSearch) {
               var obj = {
                   params: {
                       type: type
                   },
                   templateUrl: function () {
                       return templateUrl + '?lang=' + getCookie('l2') + '&version=' + version;
                   },
                   controller: controller
               };

               if (angular.isDefined(reloadOnSearch)) {
                   obj.reloadOnSearch = reloadOnSearch;
               }

               return obj;
           }
           //#endregion
           $routeProvider.

          
           otherwise({ redirectTo: '/dashboard/' });


           //function buildQueryString(url, params) {
           //    var first = true;
           //    for (var key in params) {
           //        if (first) {
           //            url += '?' + key.toLowerCase() + '=' + params[key];
           //            first = false;
           //            continue;
           //        }
           //        url += '&' + key.toLowerCase() + '=' + params[key];

           //    }
           //    return url;
           //}

           //#endregion


           //#region log js errors 
           $provide.decorator('$exceptionHandler', ['$delegate', '$log', 'stackTraceService', function ($delegate, $log, stackTraceService) {
               return function (exception, cause) {
                   $delegate(exception, cause);

                   try {
                       var errorMessage = exception.toString(),
                           stackTrace = stackTraceService.print({ e: exception });

                       $.ajax({
                           type: 'POST',
                           url: '/error/jslog/',
                           contentType: 'application/json',
                           data: angular.toJson({
                               errorUrl: window.location.href,
                               errorMessage: errorMessage,
                               stackTrace: stackTrace,
                               cause: cause || ''
                           })
                       });

                       ga('send', 'exception', {
                           exDescription: errorMessage,
                           exFatal: false,
                           version: document.querySelector('[data-version]').getAttribute('data-version')
                       });

                   }
                   catch (loggingError) {
                       $log.warn('Error logging failed');
                       $log.log(loggingError);
                   }
               };
           }]);

           //#endregion
       }
   ]);

    app.run(['$rootScope', '$window', '$location', 'sUserDetails', 'sNewUpdates', 'sVerChecker', /*'htmlCache',*/
        function ($rootScope, $window, $location, sUserDetails, sNewUpdates, sVerChecker/*, htmlCache*/) {

            //analytics
            (function (i, s, o, g, r, a, m) {
                i['GoogleAnalyticsObject'] = r; i[r] = i[r] || function () {
                    (i[r].q = i[r].q || []).push(arguments)
                }, i[r].l = 1 * new Date(); a = s.createElement(o),
                m = s.getElementsByTagName(o)[0]; a.async = 1; a.src = g; m.parentNode.insertBefore(a, m)
            })(window, document, 'script', '//www.google-analytics.com/analytics.js', 'ga');


            $location.search('search', null);
            $location.search('Search', null);

            sVerChecker.checkVersion();
            version = sVerChecker.currentVersion();

            $rootScope.$on('$routeChangeStart', function (event) {
                $window.scrollTo(0, 0);
                if ($location.hash().indexOf('search') > -1) {
                    event.preventDefault();
                }
            });

            $rootScope.$on('$routeChangeSuccess', function (event, current, previous) {
                //htmlCache.checkState();
               
                try {
                    if (sUserDetails.isAuthenticated() && !sUserDetails.getDetails().university.id) {
                        event.preventDefault();
                        $location.path('/library/choose/');
                        return;
                    }
                    if (sUserDetails.isAuthenticated() && current.$$route.params.type === 'account') {
                        $location.path('/dashboard/');
                        return;
                    }
                }
                catch (ex) {

                }

                //title 
                if (!previous) { //no previous firsttime load        
                    try {
                        if (current.$$route.params.type === 'box') {
                            if (sUserDetails.isAuthenticated()) {
                                sNewUpdates.removeUpdates(current.params.boxId);
                            }

                            setBackDashboard();
                        }
                    }
                    catch (ex) {

                    }
                    return;
                }
                document.title = 'Spitball';
                try {
                    if (current.$$route.params.type === 'box') {
                        if (sUserDetails.isAuthenticated()) {
                            sNewUpdates.removeUpdates(current.params.boxId);
                        }

                        switch (previous.$$route.params.type) {
                            case 'user':
                                $rootScope.back.title = previous.pathParams.userName;
                                $rootScope.back.url = '/user/' + previous.params.userId + '/' + previous.params.userName + '/';
                                break;
                            case 'department':
                                $rootScope.back.title = previous.params.libraryName;
                                $rootScope.back.url = '/library/' + previous.params.libraryId + '/' + previous.params.libraryName + '/';
                                break;
                            case 'search':
                                $rootScope.back.title = 'Search "' + previous.params.q + '"';
                                $rootScope.back.url = previous.loadedTemplateUrl;
                                break;
                            default:
                                setBackDashboard();
                                break;
                        }
                    }

                    if (current.$$route.params.type === 'department' && previous.$$route.params.type === 'department') {
                        $rootScope.back.title = previous.params.libraryName;
                        $rootScope.back.url = '/library/' + previous.params.libraryId + '/' + previous.params.libraryName + '/';

                    }
                }
                catch (ex) {

                }
            });

            function setBackDashboard() {
                $rootScope.back.url = '/dashboard/';
                $rootScope.back.title = 'Dashboard';
            }

        }]);
}(window.angular));


function getCookie(cname) {
    var name = cname + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') c = c.substring(1);
        if (c.indexOf(name) == 0) return c.substring(name.length, c.length);
    }
    return "";
}