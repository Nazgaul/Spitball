(function () {
    angular.module('app').factory('userDetailsFactory', userDetails);
    userDetails.$inject = ['$rootScope', '$filter', '$timeout', '$q', '$http', 'ajaxService', 'Analytics'];
    function userDetails($rootScope, $filter, $timeout, $q, $http, ajaxService, analytics) {
        "use strict";
        var
            isAuthenticated = false,
            userData,
            serverCall = false,
            deferDetails = $q.defer();

        function setDetails(data) {
            data = data || {};
            if (data.id) {
                isAuthenticated = true;
                // ReSharper disable UseOfImplicitGlobalInFunctionScope
                __insp.push(['identify', data.id]);
                // ReSharper restore UseOfImplicitGlobalInFunctionScope
            }
            analytics.set('dimension1', data.universityName || null);
            analytics.set('dimension2', data.universityCountry || null);
            analytics.set('dimension3', data.id || null);
            analytics.set('dimension4', data.theme || 'dark');
            userData = {
                id: data.id,
                name: data.name,
                image: data.image,
                score: data.score,
                url: data.url,
                isAdmin: data.isAdmin,
                theme: data.theme,
                university: {
                    country: data.universityCountry, // for google analytics
                    name: data.universityName, // in library page
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
            get: function () {
                return userData;

            },



            isAuthenticated: function () {
                return isAuthenticated;
            },
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
            //updateChange: function () {
            //    $rootScope.$broadcast('userDetailsChange');
            //},

            //getUniversity: function () {
            //    if (_.isEmpty(userData.university)) {
            //        return false;
            //    }
            //    return userData.university;

            //},
            //initDetails: function () {
            //    if (this.isAuthenticated()) {
            //        var defer = $q.defer();
            //        $timeout(function () {
            //            defer.resolve();
            //        });
            //        return defer.promise;
            //    }

            //    var promise = sAccount.details();

            //    promise.then(function (response) {
            //        setDetails(response);
            //    });

            //    return promise;
            //}
        };
    }

})();