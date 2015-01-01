angular.module('ajax').factory('dashboard',
    ['ajaxService',
    function (ajaxService) {
        function buildPath(path) {
            return '/dashboard/' + path + '/';
        }
        return {           
            boxList: function (data) {
                return ajaxService.get(buildPath('login'), data);
            },
            recommendedBoxes: function (data) {
                return ajaxService.get(buildPath('recommended'), data);
            }
        };


    }]
);