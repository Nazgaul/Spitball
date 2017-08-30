app.factory('sSearch',
    ['ajaxService',
    function (ajaxService) {
        function buildPath(path) {
            return '/search/' + path + '/';
        }
        return {
            searchByPage: function (data) {
                return ajaxService.get(buildPath('data'), data, null, false, true);
            },
            searchOtherUnis: function (data) {
                return ajaxService.get(buildPath('otheruniversities'), data);
            }
            
        };
    }
    ]);
