mUser.factory('sUser', [
    '$http',
    '$q',
    function ($http, $q) {
        var User = '/User/';
        return {
            friends: function (data) {
                var dfd = $q.defer();
                $http.get(User + 'Friends/', { params: data }).success(function (response) {
                    dfd.resolve(response);
                }).error(function (response) {
                    dfd.reject(response);
                });
                return dfd.promise;
            },
            minProfile: function (data) {
                var dfd = $q.defer();
                $http.get(User + 'MinProfile/', { params: data }).success(function (response) {
                    dfd.resolve(response);
                }).error(function (response) {
                    dfd.reject(response);
                });
                return dfd.promise;
            },
            adminFriends: function (data) {
                var dfd = $q.defer();
                $http.get(User + 'AdminFriends/', { params: data }).success(function (response) {
                    dfd.resolve(response);
                }).error(function (response) {
                    dfd.reject(response);
                });
                return dfd.promise;
            },
            boxes: function (data) {
                var dfd = $q.defer();
                $http.get(User + 'Boxes/', { params: data }).success(function (response) {
                    dfd.resolve(response);
                }).error(function (response) {
                    dfd.reject(response);
                });
                return dfd.promise;
            },
            invites: function (data) {
                var dfd = $q.defer();
                $http.get(User + 'OwnedInvites/', { params: data }).success(function (response) {
                    dfd.resolve(response);
                }).error(function (response) {
                    dfd.reject(response);
                });
                return dfd.promise;
            },
            activity: function (data) {
                var dfd = $q.defer();
                $http.get(User + 'Activity/', { params: data }).success(function (response) {
                    dfd.resolve(response);
                }).error(function (response) {
                    dfd.reject(response);
                });
                return dfd.promise;
            }
    };
}
]);
