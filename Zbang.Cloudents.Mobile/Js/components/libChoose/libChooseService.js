angular.module('libChoose')
    .service('libChooseService',
    ['$rootScope', '$state', '$q', 'account', 'library', 'Facebook', '$analytics', function ($rootScope, $state, $q, account, library, facebook, $analytics) {
        var service = this;

        service.facebookSuggestions = function () {
            var defer = $q.defer();
            facebook.getLoginStatus(function (response) {
                if (response.status === 'connected') {
                    var token = response.authResponse.accessToken;
                    library.facebookSuggestions({ authToken: token }).then(function (response) {
                        defer.resolve(response);
                    }).catch(function () {
                        defer.reject();
                    });
                    return;
                }

                defer.reject();
            });



            return defer.promise;
        };

        service.searchUnis = function (term, page) {
            $analytics.searchTrack($state.current.name, term, 'unis');

            return library.search({ term: term, page: page });
        };

        service.selectUniversity = function (universityId) {

            $analytics.eventTrack('Select university', {
                category: 'Library choose page',
                label: 'User selected a university'
            });


            return account.selectUniversity({ universityId: universityId }).then(function () {
                $state.go('root.dashboard');
            }).catch(function (response) {
                alert(response);
            });
        };

        service.doneLoad = function () {
            $rootScope.$broadcast('$stateLoaded');
        };
    }]
);