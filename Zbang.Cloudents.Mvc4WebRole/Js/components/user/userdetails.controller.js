/// <reference path="~/Scripts/angular.js" />
(function () {
    angular.module('app.user.details').controller('UserDetails', userDetails);
    userDetails.$inject = ['userDetailsService', '$scope'];

    function userDetails(userDetailsService, $scope) {
        var ud = this;
        ud.isLoggedIn = false;

        userDetailsService.getDetails().then(function (response) {
            if (response.id) {
                ud.isLoggedIn = true;
            }
            assignValues(response);
        });

        $scope.$on('userDetailsChange', function (e) {
            userDetailsService.getDetails().then(function (response) {
                assignValues(response);
            });
        });

        function assignValues(response) {
            ud.id = response.id;
            ud.name = response.name;
            ud.image = response.image;
            ud.score = response.score;
        }
    }
})();



(function () {
    angular.module('app.user').service('userDetailsService', userDetails);
    userDetails.$inject = ['ajaxService', '$q', '$rootScope'];

    function userDetails(ajaxservice, $q, $rootScope) {
        var ud = this;

        var serverCall = false;
        var defer = $q.defer();

        ud.changeImage = function(src) {
            ud.details.image = src;
            $rootScope.$broadcast('userDetailsChange');
        }

        /*culture: "en-US"
firstTimeBox: false
firstTimeDashboard: false
firstTimeItem: false
firstTimeLibrary: false
id: 1
image: "http://127.0.0.1:10000/devstoreaccount1/zboxprofilepic/S100X100/d35f6930-4e44-469d-adf8-ecac2a1b934f.jpg"
isAdmin: true
name: "ram y"
score: 51550
universityCountry: "IL"
universityId: 920
universityName: "האוניברסיטה הפתוחה"
url: "/user/1/ram-y/"
*/

        ud.getDetails = function () {
            
            if (ud.details) {
                defer.resolve(ud.details);
                return defer.promise;
            }
            if (!serverCall) {
                serverCall = true;
                ajaxservice.get('/account/details/', null, 1800000).then(function (response) {
                    serverCall = false;
                    ud.details = response;
                    defer.resolve(ud.details);
                });
            }
            return defer.promise;
        }

        ud.getAccountDetails = function() {
            return ajaxservice.get('/account/settingsdata/', null, 1800000);
        }

        ud.setAccountDetails = function (universityId, firstName, lastName, universityName) {
            ud.details.name = firstName + " " + lastName;
            ud.details.universityName = universityName;
            ud.details.universityId = universityId;
            $rootScope.$broadcast('userDetailsChange');
            return ajaxservice.post('/account/changeprofile/', { UniversityId: universityId, FirstName: firstName, LastName: lastName });
            
        }

        ud.searchUniversity = function(term) {
            return ajaxservice.get('/library/searchuniversity/', {
                term: term
            });
        }
    }
})();