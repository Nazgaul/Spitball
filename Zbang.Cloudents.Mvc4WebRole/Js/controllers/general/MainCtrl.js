﻿app.controller('MainCtrl',
    ['$scope', '$rootScope', '$location', '$modal', '$angularCacheFactory', 'sUser', 'sFacebook', 'sUserDetails', 'Store', '$analytics', '$timeout','sNotify','sLogin',
        function ($scope, $rootScope, $location, $modal, $angularCacheFactory, sUser, sFacebook, sUserDetails, sStore, $analytics, $timeout,sNotify,sLogin) {
            "use strict";

            $rootScope.options = {
                quizOpen: false         
            };


            $scope.params = {

                store: {}
            }

            $rootScope.params = {
                store: {}
            };

            $rootScope.back = {};

            $rootScope.$back = function (url) {
                if (url && url.length) {
                    $location.path(url);
                }
            };
            $rootScope.setUrl = function (url) {
                url = url.replace(location.origin, '');
                $location.url(url, '', url).replace();
            }

            $rootScope.checkReg = function (event) {
                if (!sUserDetails.isAuthenticated()) {
                    event.preventDefault();
                    sLogin.registerAction();
                    return;
                }

            };

            $rootScope.toggleSearch = function () {
                $rootScope.openSearch = !$rootScope.openSearch;
                $location.search('search', $rootScope.openSearch ? true : null);
            };
           

            $scope.$on('$routeChangeSuccess', function (event, current) {

                $rootScope.openSearch = $location.search().hasOwnProperty('search');
                
                try {
                    $rootScope.currentRoute = current.$$route.params.type + 'Page';
                    $rootScope.params.isStore = current.$$route.originalPath.indexOf('store') > -1;
                    //$rootScope.params.isDashboard = current.$$route.originalPath.indexOf('dashboard') > -1;
                    $rootScope.params.isQuiz = current.$$route.originalPath.indexOf('quiz') > -1 || current.$$route.originalPath.indexOf('item') > -1;                    
                }
                catch (ex) {
                }
            });

            $rootScope.isStore = function () {
                if (!$rootScope.params) {
                    return false;
                }
                if (!angular.isDefined($rootScope.params.isStore)) {
                    return false;
                }

                return $rootScope.params.isStore;
            }
   
            $rootScope.isQuiz = function () {
                if (!$rootScope.params) {
                    return false;
                }
                if (!angular.isDefined($rootScope.params.isQuiz)) {
                    return false;
                }

                return $rootScope.params.isQuiz;
            };


            $rootScope.logout = function (e) {
                e.preventDefault();
                $angularCacheFactory.removeAll();
                $timeout(function () {
                    window.location.href = '/account/logoff/';
                }, 500);


                $analytics.eventTrack('Logout', {
                    category: 'Site Header'
                });
            };
        }
    ]);
