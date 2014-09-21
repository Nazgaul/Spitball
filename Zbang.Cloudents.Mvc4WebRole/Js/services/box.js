//define('box',['app'], function (app) {
mBox.factory('sBox',
    ['$http',
     '$q',

    function ($http, $q) {
        var Box = '/Box/';
        function ajaxRequest(data, type, link) {
            var dfd = $q.defer();
            if (type === $http.get) {
                data = { params: data };
            }
            type(Box + link, data).success(function (response) {
                dfd.resolve(response);
            }).error(function (response) {
                dfd.reject(response);
            });
            return dfd.promise;
        }
        return {
            createPrivate: function (data) {
                var dfd = $q.defer();
                $http.post('/Dashboard/Create/', data).success(function (response) {
                    dfd.resolve(response);
                }).error(function (response) {
                    dfd.reject(response);
                });

                return dfd.promise;
            },
            createAcademic: function (data) {
                var dfd = $q.defer();
                $http.post('/Library/CreateBox/', data).success(function (response) {
                    dfd.resolve(response);
                }).error(function (response) {
                    dfd.reject(response);
                });

                return dfd.promise;
            },
            update: function (data) {
                var dfd = $q.defer();
                $http.post(Box + 'UpdateInfo/', data).success(function (response) {
                    dfd.resolve(response);
                }).error(function (response) {
                    dfd.reject(response);
                });

                return dfd.promise;
            },
            items: function (data) {
                return ajaxRequest(data, $http.get, 'Items/');
            },
            quizes: function (data) {
                return ajaxRequest(data, $http.get, 'Quizes/');
            },
            info: function (data) {
                var dfd = $q.defer();
                $http.get(Box + 'Data/', { params: data }).success(function (response) {
                    dfd.resolve(response);
                }).error(function (response) {
                    dfd.reject(response);
                });

                return dfd.promise;
            },
            createTab: function (data) {
                var dfd = $q.defer();
                $http.post(Box + 'CreateTab/', data).success(function (response) {
                    dfd.resolve(response);
                }).error(function (response) {
                    dfd.reject(response);
                });

                return dfd.promise;
            },
            deleteTab: function (data) {
                var dfd = $q.defer();
                $http.post(Box + 'DeleteTab/', data).success(function (response) {
                    dfd.resolve(response);
                }).error(function (response) {
                    dfd.reject(response);
                });

                return dfd.promise;
            },
            renameTab: function (data) {
                var dfd = $q.defer();
                $http.post(Box + 'RenameTab/', data).success(function (response) {
                    dfd.resolve(response);
                }).error(function (response) {
                    dfd.reject(response);
                });

                return dfd.promise;
            },
            addItemsToTab: function (data) {
                var dfd = $q.defer();
                $http.post(Box + 'AddItemToTab/', data).success(function (response) {
                    dfd.resolve(response);
                }).error(function (response) {
                    dfd.reject(response);
                });

                return dfd.promise;
            },
            members: function (data) {
                var dfd = $q.defer();
                $http.get(Box + 'Members/', { params: data }).success(function (response) {
                    dfd.resolve(response);
                }).error(function (response) {
                    dfd.reject(response);
                });

                return dfd.promise;
            },
            updateInfo: function (data) {
                var dfd = $q.defer();
                $http.post(Box + 'UpdateInfo/', data).success(function (response) {
                    dfd.resolve(response);
                }).error(function (response) {
                    dfd.reject(response);
                });

                return dfd.promise;
            },
            invite: function (data) {
                var dfd = $q.defer();
                $http.post('/Share/InviteBox/', data).success(function (response) {
                    dfd.resolve(response);
                }).error(function (response) {
                    dfd.reject(response);
                });

                return dfd.promise;
            },
            removeUser: function (data) {
                var dfd = $q.defer();
                $http.post(Box + 'RemoveUser/', data).success(function (response) {
                    dfd.resolve(response);
                }).error(function (response) {
                    dfd.reject(response);
                });

                return dfd.promise;
            },
            remove: function (data) {
                var dfd = $q.defer();
                $http.post(Box + 'Delete2/', data).success(function (response) {
                    dfd.resolve(response);
                }).error(function (response) {
                    dfd.reject(response);
                });

                return dfd.promise;
            },
            notification: function (data) {
                var dfd = $q.defer();
                $http.get(Box + 'GetNotification/', { params: data }).success(function (response) {
                    dfd.resolve(response);
                }).error(function (response) {
                    dfd.reject(response);
                });

                return dfd.promise;
            },
            follow: function (data) {
                var dfd = $q.defer();
                $http.post('/Share/SubscribeToBox/', data).success(function (response) {
                    dfd.resolve(response);
                }).error(function (response) {
                    dfd.reject(response);
                });

                return dfd.promise;
            },
            tabs: function (data) {
                return ajaxRequest(data, $http.get, 'Tabs/');
                $http.get(Box + 'Tabs/', { params: data }).success(function (response) {
            }
        };
    }
    ]);
//});