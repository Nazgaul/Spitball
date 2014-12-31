angular.module('ajax').factory('library',
    ['ajaxService',
    function (ajaxService) {
        function buildPath(path) {
            return '/library/' + path + '/';
        }
        return {
            search: function (data) {
                return ajaxService.get(buildPath('search'), data);
            }
        };


    }]
);