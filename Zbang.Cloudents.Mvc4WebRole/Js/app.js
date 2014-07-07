﻿//define('app', ['routes', 'dependencyResolverFor'], function (config, dependencyResolverFor) {
var app = angular.module('app', ['ngRoute', 'ngSanitize', 'infinite-scroll', 'custom_scrollbar',
    'pasvaz.bindonce', 'ui.bootstrap', 'ngAnimate', 'mDashboard', 'mBox', 'mItem', 'mLibrary', 'mQuiz', 'mUser','debounce']);

app.config([
    '$routeProvider',
    '$locationProvider',
    '$controllerProvider',
    '$compileProvider',
    '$filterProvider',
    '$httpProvider',
    '$provide',

    function ($routeProvider, $locationProvider, $controllerProvider, $compileProvider, $filterProvider, $httpProvider, $provide) {
        $httpProvider.defaults.headers.common["X-Requested-With"] = 'XMLHttpRequest';

        //if (window.history && window.history.pushState) {
        $locationProvider.html5Mode(true).hashPrefix('!');
        //}

        $routeProvider.
        when('/dashboard/',{
            params: {
                type: 'dashboard'
            },
            templateUrl: '/dashboard/'
        }).
        when('/box/my/:boxId/:boxName/',{
            params: {
                type: 'box'
            },
            templateUrl: function (params) { return '/box/my/' + params.boxId + '/' + encodeURIComponent(params.boxName) + '/'; }
        }).
        when('/course/:uniName/:boxId/:boxName/',{
            params: {
                type: 'box'
            },
            templateUrl: function (params) { return '/course/' + encodeURIComponent(params.uniName) + '/' + params.boxId + '/' + encodeURIComponent(params.boxName) + '/'; }
        }).
        when('/course/:uniName/:boxId/:boxName/',{
            params: {
                type: 'box'
            },
            templateUrl: function (params) { return '/course/' + encodeURIComponent(params.uniName) + '/' + params.boxId + '/' + encodeURIComponent(params.boxName) + '/'; }
        }).
        when('/item/:uniName/:boxId/:boxName/:itemId/:itemName/',{
            params: {
                type: 'item'
            },
            templateUrl: function (params) {
                //return '/item/' + encodeURIComponent(params.uniName) + '/' + params.boxId + '/'
                //    + encodeURIComponent(params.boxName) + '/' + params.itemId + '/' + encodeURIComponent(params.itemName) + '/';
                return '/item/?boxUid=' + params.boxId + '&itemId=' + params.itemId;
            }
        }).
        when('/quiz/:uniName/:boxId/:boxName/:quizId/:quizName/',{
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
        when('/user/:userId/:userName/',{
            params: {
                type: 'user'
            },
            templateUrl: function (params) { return '/user/' + params.userId + '/' + encodeURIComponent(params.userName) + '/'; }
        }).
        otherwise({redirectTo: '/dashboard/'});               
    }
]);

//app.factory('UserDetails',
//  [

//  function () {
//      var userData;

//      return {
//          setDetails: function (id, name, image, score, url) {
//              userData = {
//                  id: id,
//                  name: name,
//                  image: image,
//                  score: score,
//                  url: url
//              };
//          },
//          getDetails: function () {
//              return userData;
//          }
//      };
//  }
//  ]);

app.run(['$rootScope', '$window', 'sUserDetails', function ($rootScope, $window, UserDetails) {

    $rootScope.$on('$routeChangeSuccess', function (event, current, previous) {

        //title 
        if (!previous) {
            return;
        }
        if (!previous.$$route.params) {
            return;
        }

        switch (previous.$$route.params.type) {
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
    });

    $rootScope.initDetails = function (id, name, image, score, url) {
        if (id) {
            UserDetails.setDetails(id, name, image, score, url);
            return;
        }

    };
}]);

app.controller('MainCtrl',
    ['$scope', '$rootScope', '$location',
    function ($scope, $rootScope, $location) {
        $rootScope.back = {};

        $rootScope.$back = function (url) {
            if (url && url.length) {
                $location.path(url);
            }
        };
    }
    ]);


app.directive('backButton',
   ['$rootScope',

   function ($rootScope) {
       return {
           restrict: "A",
           link: function (scope, elem, attrs) {
               $location.path($rootScope.previousUrl);
           }
       };
   }
   ]);

app.factory('sUserDetails',
 [

 function () {
     var userData,
         isAuthenticated = false;

     return {
         setDetails: function (id, name, image, score, url) {
             userData = {
                 id: parseInt(id, 10),
                 name: name,
                 image: image,
                 score: parseInt(score, 10),
                 url: url
             };
             isAuthenticated = true;

         },
         getDetails: function () {
             return userData;
         },
         isAuthenticated: function () {
             return isAuthenticated;
         }
     };
 }
 ]);

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

//return app;
//});