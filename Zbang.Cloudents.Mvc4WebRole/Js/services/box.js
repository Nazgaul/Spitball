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
                return ajaxRequest(data, $http.post, 'UpdateInfo/');
            },
            items: function (data) {
                return ajaxRequest(data, $http.get, 'Items/');
            },
            quizes: function (data) {
                return ajaxRequest(data, $http.get, 'Quizes/');
            },
            info: function (data) {
                return ajaxRequest(data, $http.get, 'Data/');
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
                return ajaxRequest(data, $http.post, 'DeleteTab/');
            },
            renameTab: function (data) {
                return ajaxRequest(data, $http.post, 'RenameTab/');
            },
            addItemsToTab: function (data) {
                return ajaxRequest(data, $http.post, 'AddItemToTab/');
            },
            members: function (data) {
                return ajaxRequest(data, $http.get, 'Members/');
            },
            updateInfo: function (data) {
                return ajaxRequest(data, $http.post, 'UpdateInfo/');
                
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
                return ajaxRequest(data, $http.post, 'RemoveUser/');
            },
            remove: function (data) {
                return ajaxRequest(data, $http.post, 'Delete2/');
            },
            notification: function (data) {
                return ajaxRequest(data, $http.get, 'GetNotification/');
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
            }
        };
    }
    ]);
//});