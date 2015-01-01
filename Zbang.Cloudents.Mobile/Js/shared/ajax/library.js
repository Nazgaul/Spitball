angular.module('ajax').factory('library',
    ['ajaxService',
    function (ajaxService) {
        function buildPath(path) {
            return '/library/' + path + '/';
        }
        return {
            search: function (data) {
                return ajaxService.get(buildPath('search'), data);
            },
            facebookSuggestions: function (data) {
                return ajaxService.get(buildPath('facebookSuggest'), data);
            },
            chooseUniversity: function (data) {
                return ajaxService.post(buildPath('choose'), data);
            },
        };


    }]
);