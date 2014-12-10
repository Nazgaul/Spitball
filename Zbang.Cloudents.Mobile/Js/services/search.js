app.factory('sSearch',
    ['ajaxService',
    function (ajaxService) {
        function buildPath(path) {
            return '/search/' + path + '/';
        }
        return {
            dropdown: function (data) {
                return ajaxService.get(buildPath('dropdown'), data);
            },
            searchByPage: function (data) {
                return ajaxService.get(buildPath('data'), data);
            },
            searchOtherUnis: function (data) {
                return ajaxService.get(buildPath('otheruniversities'), data);
            }
            
        };
    }
    ]);
