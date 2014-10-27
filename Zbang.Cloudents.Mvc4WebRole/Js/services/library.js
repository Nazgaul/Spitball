mLibrary.factory('sLibrary',
    ['ajaxService',
    function (ajaxService) {
        function buildPath(path) {
            return '/library/' + path + '/';
        }
        return {
            'items': function (data) {
                return ajaxService.get(buildPath('Nodes'), data);
            },
            departments: function (data) {
                return ajaxService.get(buildPath('RussianDepartments'), data);
            },
            renameNode: function (data) {
                return ajaxService.post(buildPath('RenameNode'), data);
            },
            searchUniversities: function (data) {
                return ajaxService.get(buildPath('SearchUniversity'), data);
            },
            facebookFriends: function (data) {
                return ajaxService.get(buildPath('GetFriends'), data);
            },
            updateUniversity: function (data) {
                return ajaxService.post('/Account/UpdateUniversity/', data);
            },
            createDepartment: function (data) {
                return ajaxService.post(buildPath('Create'), data);
            },
            deleteDepartment: function (data) {
                return ajaxService.post(buildPath('DeleteNode'), data);
            },
            createUniversity: function (data) {
                return ajaxService.post(buildPath('CreateUniversity'), data);
            }
        };
    }
    ]);
