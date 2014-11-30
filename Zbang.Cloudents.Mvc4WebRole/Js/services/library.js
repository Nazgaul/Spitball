mLibrary.factory('sLibrary',
    ['ajaxService',
    function (ajaxService) {
        function buildPath(path) {
            return '/library/' + path + '/';
        }
        return {
            'items': function (data) {
                return ajaxService.get(buildPath('nodes'), data);
            },
            departments: function (data) {
                return ajaxService.get(buildPath('russiandepartments'), data);
            },
            renameNode: function (data) {
                return ajaxService.post(buildPath('renamenode'), data);
            },
            searchUniversities: function (data) {
                return ajaxService.get(buildPath('searchuniversity'), data);
            },
            facebookFriends: function (data) {
                return ajaxService.get(buildPath('getfriends'), data);
            },
            updateUniversity: function (data) {
                return ajaxService.post('/account/updateuniversity/', data);
            },
            createDepartment: function (data) {
                return ajaxService.post(buildPath('create'), data);
            },
            deleteDepartment: function (data) {
                return ajaxService.post(buildPath('deletenode'), data);
            },
            createUniversity: function (data) {
                return ajaxService.post(buildPath('createuniversity'), data);
            }
        };
    }
    ]);
