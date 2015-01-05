angular.module('ajax').factory('library',
    ['ajaxService','$q',
    function (ajaxService,$q) {
        function buildPath(path) {
            return '/library/' + path + '/';
        }      
        return {
            search: function (data) {
                return ajaxService.get(buildPath('searchuniversity'), data);
            },
            facebookSuggestions: function (data) {
                return ajaxService.get(buildPath('getfriends'), data);
            }       
        };


    }]
);