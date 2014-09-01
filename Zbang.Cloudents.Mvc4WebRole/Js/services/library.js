mLibrary.factory('sLibrary',
    ['$http',
     '$q',

    function ($http, $q) {
        var Lib = '/Library/';
        return {

            'items': function (data) {
                var dfd = $q.defer();
                $http.get(Lib + 'Nodes/', { params: data }).success(function (response) {
                    dfd.resolve(response);
                }).error(function (response) {
                    dfd.reject(response);
                });

                return dfd.promise;
            },
            box: {
                create: function (data) {
                    var dfd = $q.defer();
                    $http.post(Lib + 'CreateBox/', data).success(function (response) {
                        dfd.resolve(response);
                    }).error(function (response) {
                        dfd.reject(response);
                    });

                    return dfd.promise;
                }
            },
            department: {
                create: function (data) {
                    var dfd = $q.defer();
                    $http.post(Lib + 'Create/', data).success(function (response) {
                        dfd.resolve(response);
                    }).error(function (response) {
                        dfd.reject(response);
                    });

                    return dfd.promise;
                },
                'delete': function (data) {
                    var dfd = $q.defer();
                    $http.post(Lib + 'DeleteNode/', data).success(function (response) {
                        dfd.resolve(response);
                    }).error(function (response) {
                        dfd.reject(response);
                    });

                    return dfd.promise;
                }
            },
            departments: function (data) {
                var dfd = $q.defer();
                $http.get(Lib + 'Departments/', { params: data }).success(function (response) {
                    dfd.resolve(response);
                }).error(function (response) {
                    dfd.reject(response);
                });

                return dfd.promise;
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
            searchUniversities : function(data){
                var dfd = $q.defer();
                $http.get(Lib + 'SearchUniversity/', { params: data }).success(function (response) {
                    dfd.resolve(response);
                }).error(function (response) {
                    dfd.reject(response);
                });

                return dfd.promise;
            },
            facebookFriends: function (data) {
                var dfd = $q.defer();
                $http.get(Lib + 'GetFriends/', { params: data }).success(function (response) {
                    dfd.resolve(response);
                }).error(function (response) {
                    dfd.reject(response);
                });

                return dfd.promise;
            },
            searchDepartment: function (data) {
                var dfd = $q.defer();
                $http.get(Lib + 'SearchDepartment/', { params: data }).success(function (response) {
                    dfd.resolve(response);
                }).error(function (response) {
                    dfd.reject(response);
                });

                return dfd.promise;
            }
        };
    }
    ]);
