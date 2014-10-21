mLibrary.factory('sLibrary',
    ['$http',
     '$q',

    function ($http, $q) {
        var Lib = '/Library/';
        function ajaxRequest(data, type, link) {
            var dfd = $q.defer();
            if (type === $http.get) {
                data = { params: data };
            }
            type(Lib + link, data).success(function (response) {
                dfd.resolve(response);
            }).error(function (response) {
                dfd.reject(response);
            });
            return dfd.promise;
        }
        return {

            'items': function (data) {
                return ajaxRequest(data, $http.get, 'Nodes/');
            },
            departments: function (data) {
                return ajaxRequest(data, $http.get, 'RussianDepartments/');
            },
            renameNode: function (data) {
                var dfd = $q.defer();
                $http.post(Lib + 'RenameNode/', data).success(function (response) {
                    dfd.resolve(response);
                }).error(function (response) {
                    dfd.reject(response);
                });

                return dfd.promise;
            },
            searchUniversities: function (data) {
                return ajaxRequest(data, $http.get, 'SearchUniversity/');
            },
            facebookFriends: function (data) {
                return ajaxRequest(data, $http.get, 'GetFriends/');
            },
            //searchDepartment: function (data) {
            //    return ajaxRequest(data, $http.get, 'Departments/');
            //},
            updateUniversity: function (data) {
                var dfd = $q.defer();
                $http.post('/Account/UpdateUniversity/', data).success(function (response) {
                    dfd.resolve(response);
                }).error(function (response) {
                    dfd.reject(response);
                });

                return dfd.promise;
            },
            createDepartment: function (data) {
                return ajaxRequest(data, $http.post, 'Create/');
            },
            //chooseDeparment: function (data) {
            //    return ajaxRequest(data, $http.post, 'SelectDepartment/');
            //},
            createUniversity: function (data) {
                return ajaxRequest(data, $http.post, 'CreateUniversity/');
            }
        };
    }
    ]);
