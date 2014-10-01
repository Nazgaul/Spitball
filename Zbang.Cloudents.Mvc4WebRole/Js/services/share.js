app.factory('sShare',
    ['$http',
     '$q',

    function ($http, $q) {
        var Share = '/Share/';
        return {
            cloudentsFriends: function (data) {
                var dfd = $q.defer();
                $http.get('/User/Friends', { params: data }).success(function (response) {
                    dfd.resolve(response);
                }).error(function (response) {
                    dfd.reject(response);
                });

                return dfd.promise;
            },
            googleFriends: function (data) {
                var dfd = $q.defer();
                $http.post('/User/GoogleContacts', data).success(function (response) {
                    dfd.resolve(response);
                }).error(function (response) {
                    dfd.reject(response);
                });

                return dfd.promise;
            },
            message: function (data) {
                var dfd = $q.defer();
                $http.post(Share + '/Message', data).success(function (response) {
                    dfd.resolve(response);
                }).error(function (response) {
                    dfd.reject(response);
                });

                return dfd.promise;
            },
            invite: {
                box: function (data) {
                    var dfd = $q.defer();
                    $http.post(Share + 'InviteBox', data).success(function (response) {
                        dfd.resolve(response);
                    }).error(function (response) {
                        dfd.reject(response);
                    });

                    return dfd.promise;
                },
                cloudents: function (data) {
                    var dfd = $q.defer();
                    $http.post(Share + '/Invite', data).success(function (response) {
                        dfd.resolve(response);
                    }).error(function (response) {
                        dfd.reject(response);
                    });

                    return dfd.promise;
                }
            },
            facebookInvite: {
                box: function (data) {
                    var dfd = $q.defer();
                    $http.post(Share + '/InviteBoxFacebook', data).success(function (response) {
                        dfd.resolve(response);
                    }).error(function (response) {
                        dfd.reject(response);
                    });

                    return dfd.promise;
                },
                cloudents: function (data) {
                    var dfd = $q.defer();
                    $http.post(Share + '/InviteFacebook', data).success(function (response) {
                        dfd.resolve(response);
                    }).error(function (response) {
                        dfd.reject(response);
                    });

                    return dfd.promise;
                }
            }
        };
    }
    ]);


