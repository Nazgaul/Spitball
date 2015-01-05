angular.module('libChoose')
    .service('libChooseService',
    ['$state', 'account', 'library', 'facebook', function ($state, account, library, facebook) {
        var service = this;

        service.facebookSuggestions = function () {
            var defer = $q.defer();
            facebook.getToken({ token: token }).then(function (token) {
                library.facebookSuggestions({}).then(function (response) {
                    defer.resolve(parseResults(response));
                    return;
                });
            }).catch(function () {
                defer.reject();
                return;
            });

            return defer.promise;
        };

        service.searchUnis = function (term, page) {
            return library.search({ term: term, page: page });
        };

        service.selectUniversity = function (universityId) {
            return account.selectUniversity().then(function () {
                $state.go('root.dashboard');
            }).catch(function (response) {
                alert(response);
            });
        };
    }]
);