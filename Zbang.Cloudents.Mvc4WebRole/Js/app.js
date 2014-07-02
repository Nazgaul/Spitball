define('app', ['routes', 'dependencyResolverFor'], function (config, dependencyResolverFor) {
    var app = angular.module('app', ['ngRoute', 'ngSanitize', 'infinite-scroll', 'pasvaz.bindonce', 'ui.bootstrap']);

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
                case 'dashboard':
                    $rootScope.previousTitle = 'Dashboard';
                    break;
                case 'library': {
                    cd.pubusub.pubish('libraryclear');
                    break;
                }
                case 'item': {
                    cd.pubusub.pubish('itemclear');
                    break;
                }
            }
        });

        $rootScope.$back = function (url) {
            if (!$window.history.length) {
                $location.path(url);
                return;
            }
            $window.history.back();
        };

        $rootScope.initDetails = function (id, name, image, score, url) {
            UserDetails.setDetails(id, name, image, score, url);
        };




    }]);

    app.controller('MainCtrl',
        ['$scope', '$rootScope',
        function ($scope, $rootScope) {
       
        }
        ]);

    app.factory('UserDetails',
     [

     function () {
         var userData;

         return {
             setDetails: function (id, name, image, score, url) {
                 userData = {
                     id: parseInt(id, 10),
                     name: name,
                     image: image,
                     score: parseInt(score, 10),
                     url: url
                 };
             },
             getDetails: function () {
                 return userData;
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