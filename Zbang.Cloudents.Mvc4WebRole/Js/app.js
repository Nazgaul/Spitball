define('app', ['routes', 'dependencyResolverFor'], function (config, dependencyResolverFor) {
    var app = angular.module('app', ['ngRoute', 'ngSanitize', 'infinite-scroll', 'debounce', 'pasvaz.bindonce', 'ui.bootstrap','ngAnimate']);

    app.config([
        '$routeProvider',
        '$locationProvider',
        '$controllerProvider',
        '$compileProvider',
        '$filterProvider',
        '$httpProvider',
        '$provide',

        function ($routeProvider, $locationProvider, $controllerProvider, $compileProvider, $filterProvider, $httpProvider, $provide) {
            app.controller = $controllerProvider.register;
            app.directive = $compileProvider.directive;
            app.filter = $filterProvider.register;
            app.factory = $provide.factory;
            app.service = $provide.service;

            //if (window.history && window.history.pushState) {
            $locationProvider.html5Mode(true).hashPrefix('!');
            //}

            if (config.routes !== undefined) {
                angular.forEach(config.routes, function (route, path) {
                    $routeProvider.when(path, { templateUrl: route.templateUrl, params: route.params, resolve: dependencyResolverFor(route.dependencies) });
                });
            }

            if (config.defaultRoutePath !== undefined) {
                $routeProvider.otherwise({ redirectTo: config.defaultRoutePath });
            }

            $httpProvider.defaults.headers.common["X-Requested-With"] = 'XMLHttpRequest';

        }
    ]);

    app.factory('UserDetails',
      [

      function () {
          var userData;

          return {
              setDetails: function (id, name, image, score, url) {
                  userData = {
                      id: id,
                      name: name,
                      image: image,
                      score: score,
                      url: url
                  };
              },
              getDetails: function () {
                  return userData;
              }
          };
      }
      ]);

    app.run(['$rootScope', '$window', 'UserDetails', function ($rootScope, $window, UserDetails) {
        
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
                        previous.pathParams.userName;
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

    app.factory('UserDetails',
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

    return app;
});