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

        ud.searchUniversity = function(term) {
            return ajaxservice.get('/library/searchuniversity/', {
                term: term
            });
        }
    }
})();