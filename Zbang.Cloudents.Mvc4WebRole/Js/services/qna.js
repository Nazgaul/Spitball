    mBox.factory('sQnA',
        ['$http',
         '$q',

        function ($http, $q) {
            var QnA = '/QnA/';

            return {
                list: function (data) {
                    var dfd = $q.defer();
                    $http.get(QnA, { params: data }).success(function (response) {
                        dfd.resolve(response);
                    }).error(function (response) {
                        dfd.reject(response);
                    });

                    return dfd.promise;
                },
                post: {
                    question: function (data) {
                        var dfd = $q.defer();
                        $http.post(QnA + 'AddQuestion/', data).success(function (response) {
                            dfd.resolve(response);
                        }).error(function (response) {
                            dfd.reject(response);
                        });

                        return dfd.promise;
                    },
                    answer: function (data) {
                        var dfd = $q.defer();
                        $http.post(QnA + 'AddAnswer/', data).success(function (response) {
                            dfd.resolve(response);
                        }).error(function (response) {
                            dfd.reject(response);
                        });

                        return dfd.promise;
                    },
                },
                'delete': {
                    question: function (data) {
                        var dfd = $q.defer();
                        $http.post(QnA + 'DeleteQuestion/', data).success(function (response) {
                            dfd.resolve(response);
                        }).error(function (response) {
                            dfd.reject(response);
                        });

                        return dfd.promise;
                    },
                    answer: function (data) {
                        var dfd = $q.defer();
                        $http.post(QnA + 'DeleteAnswer/', data).success(function (response) {
                            dfd.resolve(response);
                        }).error(function (response) {
                            dfd.reject(response);
                        });

                        return dfd.promise;
                    },
                    attachment: function (data) {
                        var dfd = $q.defer();
                        $http.post(QnA + 'RemoveFile/', data).success(function (response) {
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
