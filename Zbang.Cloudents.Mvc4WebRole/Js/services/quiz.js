mBox.factory('sQuiz', [
    '$http',
    '$q',
    function ($http, $q) {
        return {
            create: function (data) {
                var dfd = $q.defer();
                $http.post('/Quiz/Create/', data).success(function (response) {
                    dfd.resolve(response);
                }).error(function (response) {
                    dfd.reject(response);
                });

                return dfd.promise;
            },
            update: function (data) {
                var dfd = $q.defer();
                $http.post('/Quiz/Update/', data).success(function (response) {
                    dfd.resolve(response);
                }).error(function (response) {
                    dfd.reject(response);
                });

                return dfd.promise;
            },
            'delete': function (data) {
                var dfd = $q.defer();
                $http.post('/Quiz/Delete/', data).success(function (response) {
                    dfd.resolve(response);
                }).error(function (response) {
                    dfd.reject(response);
                });

                return dfd.promise;
            },
            save: function (data) {
                var dfd = $q.defer();
                $http.post('/Quiz/Save/', data).success(function (response) {
                    dfd.resolve(response);
                }).error(function (response) {
                    dfd.reject(response);
                });

                return dfd.promise;
            },
            getDraft: function (data) {
                var dfd = $q.defer();
                $http.get('/Quiz/GetDraft', { params: data }).success(function (response) {
                    dfd.resolve(response);
                }).error(function (response) {
                    dfd.reject(response);
                });
                return dfd.promise;

            },
            question: {
                create: function (data) {
                    var dfd = $q.defer();
                    $http.post('/Quiz/CreateQuestion/', data).success(function (response) {
                        dfd.resolve(response);
                    }).error(function (response) {
                        dfd.reject(response);
                    });
                    return dfd.promise;

                },
                update: function (data) {
                    var dfd = $q.defer();
                    $http.post('/Quiz/UpdateQuestion/', data).success(function (response) {
                        dfd.resolve(response);
                    }).error(function (response) {
                        dfd.reject(response);
                    });
                    return dfd.promise;

                },
                'delete': function (data) {
                    var dfd = $q.defer();
                    $http.post('/Quiz/DeleteQuestion/', data).success(function (response) {
                        dfd.resolve(response);
                    }).error(function (response) {
                        dfd.reject(response);
                    });
                    return dfd.promise;

                }
            },
            answer: {
                create: function (data) {
                    var dfd = $q.defer();
                    $http.post('/Quiz/CreateAnswer/', data).success(function (response) {
                        dfd.resolve(response);
                    }).error(function (response) {
                        dfd.reject(response);
                    });
                    return dfd.promise;

                },
                update: function (data) {
                    var dfd = $q.defer();
                    $http.post('/Quiz/UpdateAnswer/', data).success(function (response) {
                        dfd.resolve(response);
                    }).error(function (response) {
                        dfd.reject(response);
                    });
                    return dfd.promise;

                },
                'delete': function (data) {
                    var dfd = $q.defer();
                    $http.post('/Quiz/DeleteAnswer/', data).success(function (response) {
                        dfd.resolve(response);
                    }).error(function (response) {
                        dfd.reject(response);
                    });
                    return dfd.promise;

                },
                markCorrect: function (data) {
                    var dfd = $q.defer();
                    $http.post('/Quiz/MarkCorrect/', data).success(function (response) {
                        dfd.resolve(response);
                    }).error(function (response) {
                        dfd.reject(response);
                    });
                    return dfd.promise;

                }
            },
            discussion: {
                createDiscussion: function (data) {
                    var dfd = $q.defer();
                    $http.post('/Quiz/CreateDiscussion/', data).success(function (response) {
                        dfd.resolve(response);
                    }).error(function (response) {
                        dfd.reject(response);
                    });
                    return dfd.promise;

                },
                deleteDiscussion: function (data) {
                    var dfd = $q.defer();
                    $http.post('/Quiz/DeleteDiscussion/', data).success(function (response) {
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
