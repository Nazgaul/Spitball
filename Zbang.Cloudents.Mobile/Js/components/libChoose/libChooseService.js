angular.module('libChoose')
    .service('libChooseService',
    ['library', 'facebook', function (library, facebook) {
        var service = this;

        service.facebookSuggestions = function () {
            var defer = $q.defer();
            facebook.getToken({ token: token }).then(function (token) {
                library.facebookSuggestions({}).then(function (response) {
                    return parseResults(response);
                });
            }).catch(function () {

            });

            return defer.promise;
        }
    }]
);