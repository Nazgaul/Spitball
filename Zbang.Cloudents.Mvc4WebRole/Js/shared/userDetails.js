'use strict';
(function () {
    angular.module('app').factory('userDetailsFactory', userDetails);
    userDetails.$inject = ['$rootScope', '$q', 'ajaxService2', 'Analytics'];
    function userDetails($rootScope, $q, ajaxService, analytics) {
        "use strict";
        var isAuthenticated = false, userData, serverCall = false, deferDetails = $q.defer();
        function setDetails(data) {
            if (data.id) {
                isAuthenticated = true;
                __insp.push(['identify', data.id]);
            }
            analytics.set('dimension1', data.universityName || null);
            analytics.set('dimension2', data.universityCountry || null);
            analytics.set('dimension3', data.id || null);
            analytics.set('dimension4', data.theme || 'dark');
            var interval = window.setInterval(function () {
                if (googletag.pubads !== undefined && googletag.pubads) {
                    googletag.pubads().setTargeting('gender', data.sex);
                    googletag.pubads().setTargeting('university', data.universityId);
                    window.clearInterval(interval);
                }
            }, 20);
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
                unread: data.unread,
                university: {
                    country: data.universityCountry,
                    name: data.universityName,
                    id: data.universityId
                }
            };
        }
        function init() {
            if (userData) {
                deferDetails.resolve(userData);
                return deferDetails.promise;
            }
            if (!serverCall) {
                serverCall = true;
                ajaxService.get('/account/details/', null, 'accountDetail').then(function (response) {
                    setDetails(response);
                    deferDetails.resolve(userData);
                    serverCall = false;
                });
            }
            return deferDetails.promise;
        }
        return {
            init: init,
            get: function () { return userData; },
            isAuthenticated: function () { return isAuthenticated; },
            setName: function (first, last) {
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
            getUniversity: function () {
                return userData ? userData.university.id : null;
            },
            setUniversity: function () {
                ajaxService.deleteCacheCategory('accountDetail');
                userData = null;
                return init();
            },
            setTheme: function (theme) {
                userData.theme = theme;
            }
        };
    }
})();
