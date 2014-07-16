var app = angular.module('app', ['ngRoute', 'ngSanitize', 'infinite-scroll', 'custom_scrollbar', 'monospaced.elastic',
    'pasvaz.bindonce', 'ui.bootstrap', 'ngAnimate', 'mDashboard', 'mBox', 'mItem', 'mLibrary', 'mQuiz', 'mUser', 'debounce']);

app.config([
    '$routeProvider',
    '$locationProvider',
    '$controllerProvider',
    '$compileProvider',
    '$filterProvider',
    '$httpProvider',
    '$tooltipProvider',
    '$provide',

    function ($routeProvider, $locationProvider, $controllerProvider, $compileProvider, $filterProvider, $httpProvider, $tooltipProvider, $provide) {
        $httpProvider.defaults.headers.common["X-Requested-With"] = 'XMLHttpRequest';

        $provide.factory('requestinterceptor', ['$location', '$q', function ($location, $q) {
            return {
                // optional method
                'response': function (response) {
                    // do something on success
                    switch (response.status) {
                        case 200:
                            return response;
                        case 401:
                        case 403:
                            window.open('/account', '_self');
                            break;
                        case 500:
                            window.open('/error', '_self');
                        default:
                            window.open('/error', '_self');
                            break;

                    }
                }
            };
        }]);



        $httpProvider.interceptors.push('requestinterceptor');

        $tooltipProvider.options({
            placement: 'bottom',
            animation: true,
            popupDelay: 500,
            //appendToBody: true
        });

        $locationProvider.html5Mode(true).hashPrefix('!');

        //#region routes
        $routeProvider.
        when('/dashboard/', {
            params: {
                type: 'dashboard'
            },
            templateUrl: '/dashboard/'
        }).
        when('/box/my/:boxId/:boxName/', {
            params: {
                type: 'box'
            },
            templateUrl: function (params) { return '/box/my/' + params.boxId + '/' + encodeURIComponent(params.boxName) + '/'; }
        }).
        when('/course/:uniName/:boxId/:boxName/', {
            params: {
                type: 'box'
            },
            templateUrl: function (params) { return '/course/' + encodeURIComponent(params.uniName) + '/' + params.boxId + '/' + encodeURIComponent(params.boxName) + '/'; }
        }).
        when('/course/:uniName/:boxId/:boxName/', {
            params: {
                type: 'box'
            },
            templateUrl: function (params) { return '/course/' + encodeURIComponent(params.uniName) + '/' + params.boxId + '/' + encodeURIComponent(params.boxName) + '/'; }
        }).
        when('/item/:uniName/:boxId/:boxName/:itemId/:itemName/', {
            params: {
                type: 'item'
            },
            templateUrl: function (params) {
                //return '/item/' + encodeURIComponent(params.uniName) + '/' + params.boxId + '/'
                //    + encodeURIComponent(params.boxName) + '/' + params.itemId + '/' + encodeURIComponent(params.itemName) + '/';
                return '/item/?boxUid=' + params.boxId + '&itemId=' + params.itemId;
            }
        }).
        when('/quiz/:uniName/:boxId/:boxName/:quizId/:quizName/', {
            params: {
                type: 'quiz'
            },
            templateUrl: function (params) {
                return '/quiz/' + encodeURIComponent(params.uniName) + '/' + params.boxId + '/'
                    + encodeURIComponent(params.boxName) + '/' + params.quizId + '/' + encodeURIComponent(params.quizName) + '/';
            }
        }).
        when('/library/:libraryId/:libraryName/', {
            params: {
                type: 'library'
            },
            templateUrl: function (params) { return '/library/' + params.libraryId + '/' + encodeURIComponent(params.libraryName) + '/'; }
        }).
        when('/library/', {
            params: {
                type: 'library'
            },
            templateUrl: '/library/'
        }).
        when('/user/:userId/:userName/', {
            params: {
                type: 'user'
            },
            templateUrl: function (params) { return '/user/' + params.userId + '/' + encodeURIComponent(params.userName) + '/'; }
        }).
        otherwise({ redirectTo: '/dashboard/' });

        //#endregion


        //#region log js errors 
        $provide.decorator('$exceptionHandler', ['$delegate','$log', 'stackTraceService', function ($delegate,$log, stackTraceService) {
            return function (exception, cause) {
                $delegate(exception, cause);

                try {
                    var errorMessage = exception.toString(),
                        stackTrace = stackTraceService.print({ e: exception });

                    $.ajax({
                        type: 'POST',
                        url: '/Error/JsLog',
                        contentType: 'application/json',
                        data: angular.toJson({
                            errorUrl: window.location.href,
                            errorMessage: errorMessage,
                            stackTrace: stackTrace,
                            cause: cause || ''
                        })
                    })

                }
                catch (loggingError) {
                    $log.warn('Error logging failed');
                    $log.log(loggingError);
                };
            };
        }]);

        //#endregion
    }
]);

app.run(['$rootScope', '$window', 'sUserDetails', 'sNewUpdates', function ($rootScope, $window, sUserDetails, sNewUpdates) {

    $rootScope.$on('$routeChangeSuccess', function (event, current, previous) {

        //title 
        if (!previous || (!previous.$$route && !previous.$$route.params)) {
            return;
        }

        switch (previous.$$route.params.type) {
            case 'box':
                sNewUpdates.removeUpdates(previous.params.boxId);
                break;
            case 'library':
                cd.pubsub.publish('libraryclear');
                break;
            case 'user':
                cd.pubsub.publish('userclear');
                break;
            case 'item':
                cd.pubsub.publish('itemclear');
                break;
            case 'quiz':
                cd.pubsub.publish('quizclear');
                break;
        };
        if (!current || (!current.$$route && !current.$$route.params)) {
            return;
        }
        if (current.$$route.params.type === 'box') {
            switch (previous.$$route.params.type) {
                case 'library':
                    $rootScope.back.title = previous.pathParams.libraryName;
                    $rootScope.back.url = previous.loadedTemplateUrl;
                    break;
                case 'user':
                    $rootScope.back.title = previous.pathParams.userName;
                    $rootScope.back.url = previous.loadedTemplateUrl;
                    break;
                default:
                    $rootScope.back.url = '/dashboard/';
                    $rootScope.back.title = 'Dashboard';
                    break;
            }
        }
        $rootScope.initDetails = function (id, name, image, score, url) {

            if (id) {
                sUserDetails.setDetails(id, name, image, score, url);
                return;
            }
            sUserDetails.setDetails(null, '', $('body').data('pic'), 0, null);

        };

    });


    var rtlChars = '\u0600-\u06FF' + '\u0750-\u077F' + '\u08A0-\u08FF' + '\uFB50-\uFDFF' + '\uFE70-\uFEFF';//arabic
    rtlChars += '\u0590-\u05FF' + '\uFB1D-\uFB4F';//hebrew

    var controlChars = '\u0000-\u0020';
    controlChars += '\u2000-\u200D';

    //Start Regular Expression magic
    var reRtl = new RegExp('[' + rtlChars + ']', 'g'),
        reNotRtl = new RegExp('[^' + rtlChars + controlChars + ']', 'g'),
        textAlign = $('html').css('direction') === 'ltr' ? 'left' : 'right';

    function checkRtlDirection(value) {

        if (!value) {
            return;
        }

        var rtls = value.match(reRtl);
        if (rtls !== null)
            rtls = rtls.length;
        else
            rtls = 0;

        var notrtls = value.match(reNotRtl);
        if (notrtls !== null)
            notrtls = notrtls.length;
        else
            notrtls = 0;

        return rtls > notrtls;
    }
    $(document).on('input', 'input,textarea', function () {
        if (!this.value.length) {
            $(this).css('direction', '').css('text-align', '');
            return;
        }
        if (checkRtlDirection(this.value)) {
            $(this).css('direction', 'rtl').css('text-align', 'right');
        } else {
            $(this).css('direction', 'ltr').css('text-align', 'left');
        }
    });


    var setElementDirection = function (element) {
        if (checkRtlDirection(element.textContent)) {
            $(element).css({ 'direction': 'rtl', 'text-align': textAlign });
        } else {
            $(element).css({ 'direction': 'ltr', 'text-align': textAlign });
        }
    }

    //#endregion
}]);

app.directive('postRepeatDirective',
    ['$timeout',
    function ($timeout) {
        return function (scope) {
            if (scope.$first) {
                window.a = new Date();   // window.a can be updated anywhere if to reset counter at some action if ng-repeat is not getting started from $first
            }
            if (scope.$last) {
                $timeout(function () {
                    console.log("## DOM rendering list took: " + (new Date() - window.a) + " ms");
                });
            }
        };
    }
    ]);

