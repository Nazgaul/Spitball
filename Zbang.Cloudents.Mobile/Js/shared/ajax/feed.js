angular.module('ajax').factory('feed',
    ['ajaxService',
    function (ajaxService) {
        function buildPath(path) {
            return '/qna/' + path + '/';
        }
        return {
            list: function (data) {
                return ajaxService.get(buildPath('list'), data);
            },
            addQuestion: function (data) {
                return ajaxService.post(buildPath('addQuestion'), data);
            },
            addAnswer: function (data) {
                return ajaxService.post(buildPath('addAnswer'), data);
            }
        };


    }]
);