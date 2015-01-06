angular.module('ajax').factory('box',
    ['ajaxService',
    function (ajaxService) {
        function buildPath(path) {
            return '/box/' + path + '/';
        }
        return {
            data: function (data) {
                return ajaxService.get(buildPath('data'), data);
            },
            items: function (data) {
                return ajaxService.get(buildPath('items'), data);
            },
            tabs: function (data) {
                return ajaxService.get(buildPath('tabs'), data);
            }
        };


    }]
);