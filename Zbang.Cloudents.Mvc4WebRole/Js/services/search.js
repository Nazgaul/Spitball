app.factory('sSearch',
    ['ajaxService',
    function (ajaxService) {
        function buildPath(path) {
            return '/Search/' + path + '/';
        }
        return {
            dropdown: function (data) {
                return ajaxService.get(buildPath('Dropdown'), data);
            },
            searchByPage: function (data) {
                return ajaxService.get(buildPath('Data'), data);
            },
            searchOtherUnis: function (data) {
                return ajaxService.get(buildPath('OtherUniversities'), data);
            }
            
        };
    }
    ]);
