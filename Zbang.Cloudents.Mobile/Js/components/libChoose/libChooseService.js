angular.module('libChoose')
    .service('libChooseService',
    ['$state', '$q', 'account', 'library', 'Facebook', function ($state, $q, account, library, facebook) {
        var service = this;

        service.facebookSuggestions = function () {
            var defer = $q.defer();
            facebook.getLoginStatus(function (response) {
                if (response.status === 'connected') {
                    var token = response.authResponse.accessToken;
                    library.facebookSuggestions({ authToken: token }).then(function (response) {
                        defer.resolve(response);
                    });
                }
            });

            return defer.promise;
        };

        service.searchUnis = function (term, page) {
            return library.search({ term: term, page: page });
        };

        service.selectUniversity = function (universityId) {
            return account.selectUniversity({ universityId: universityId }).then(function () {
                $state.go('root.dashboard');
            }).catch(function (response) {
                alert(response);
            });
        };
    }]
);