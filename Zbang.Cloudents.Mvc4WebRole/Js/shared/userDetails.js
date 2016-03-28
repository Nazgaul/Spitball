/// <reference path="../../scripts/typings/angularjs/angular.d.ts" />
(function () {
    angular.module('app').factory('userDetailsFactory', userDetails);
    userDetails.$inject = ['$rootScope', '$filter', '$timeout', '$q', '$http', 'ajaxService', 'Analytics'];
    function userDetails($rootScope, $filter, $timeout, $q, $http, ajaxService, analytics) {
        "use strict";
        var isAuthenticated = false, userData, serverCall = false, deferDetails = $q.defer();
        function setDetails(data) {
            // data = data || {};
            if (data.id) {
                isAuthenticated = true;
                // ReSharper disable UseOfImplicitGlobalInFunctionScope
                __insp.push(['identify', data.id]);
            }
            analytics.set('dimension1', data.universityName || null);
            analytics.set('dimension2', data.universityCountry || null);
            analytics.set('dimension3', data.id || null);
            analytics.set('dimension4', data.theme || 'dark');
            $timeout(function () {
                googletag.pubads().setTargeting('gender', data.sex);
            }, 1000);
            userData = {
                id: data.id,
                name: data.name,
                image: data.image,
                sex: data.sex,
                score: data.score,
                url: data.url,
                createTime: new Date(data.dateTime),
                isAdmin: data.isAdmin,
                theme: data.theme,
                culture: data.culture,
                email: data.email,
                university: {
                    country: data.universityCountry,
                    name: data.universityName,
                    id: data.universityId
                }
            };
        }
        return {
            init: function (refresh) {
                if (refresh) {
                    deferDetails = $q.defer();
                    userData = null;
                }
                if (userData) {
                    deferDetails.resolve(userData);
                    return deferDetails.promise;
                }
                if (!serverCall) {
                    serverCall = true;
                    ajaxService.get('/account/details/').then(function (response) {
                        setDetails(response);
                        deferDetails.resolve(userData);
                        serverCall = false;
                    });
                }
                return deferDetails.promise;
            },
            get: function () { return userData; },
            isAuthenticated: function () { return isAuthenticated; },
            setName: function (first, last) {
                //userData.firstName = first;
                //userData.lastName = last;
                userData.name = first + " " + last;
                $rootScope.$broadcast('userDetailsChange');
            },
            setImage: function (image) {
                if (!image) {
                    return;
                }
                userData.image = image;
                $rootScope.$broadcast('userDetailsChange');
            },
            setUniversity: function (name, id) {
                userData.university.name = name;
                userData.university.id = id;
                $rootScope.$broadcast('universityChange', userData);
            },
            setTheme: function (theme) {
                userData.theme = theme;
                //$rootScope.$broadcast('themeChange', userData);
            }
        };
    }
})();
