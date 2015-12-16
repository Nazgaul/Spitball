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

        //self.init();
        //ajaxService.get('/account/details/').then(function (response) {
        //    setDetails(response);
        //    //self.details = response;
        //    deferDetails.resolve(userData);
        //    deferAuth.resolve(isAuthenticated);
        //    loadData = true;
        //});




        function setDetails(data) {
            data = data || {};

            //$http.defaults.headers.common["RequestVerificationToken"] = data.token;


            if (data.id) {
                isAuthenticated = true;
                //$rootScope.user = {
                //isAuthenticated: true
                //};



            }
            //var x = window.ga;

            //x('create', 'UA-9850006-3', analyticsObj);

            analytics.set('dimension1', data.universityName || null);
            analytics.set('dimension2', data.universityCountry || null);
            analytics.set('dimension3', data.id || null);


            userData = {
                id: data.id,
                name: data.name,
                image: data.image,
                score: data.score,
                url: data.url,
                isAdmin: data.isAdmin,
                //culture: data.culture,

                //firstTimeDashboard: data.firstTimeDashboard,
                //firstTimeBox: data.firstTimeBox,
                //firstTimeLibrary: data.firstTimeLibrary,
                //firstTimeItem: data.firstTimeItem,
                university: {
                    //country: data.universityCountry, // for google analytics
                    name: data.universityName, // in library page
                    id: data.universityId
                }
            };

            //if (userData.name) {
            //    var splitted = data.name.split(' ');
            //    userData.firstName = splitted[0];
            //    switch (splitted) {
            //        case 2:
            //            userData.lastName = splitted[1];
            //            break;
            //        case 3:
            //            userData.middleName = splitted[1];
            //            userData.lastName = splitted[2];
            //            break;
            //    }
            //}

        }

        return {
            init: function () {
                if (userData) {
                    deferDetails.resolve(userData);
                    return deferDetails.promise;
                }
                if (!serverCall) {
                    serverCall = true;

                    ajaxService.get('/account/details/').then(function (response) {
                        setDetails(response);
                        deferDetails.resolve(userData);
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