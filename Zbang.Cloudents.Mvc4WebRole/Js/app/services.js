// Define you service here. Services can be added to same module as 'main' or a seperate module can be created.

var cloudentsServices = angular.module('apiService', ['jmdobry.angular-cache']).config(function ($angularCacheFactoryProvider) {
    $angularCacheFactoryProvider.setCacheDefaults({
        maxAge: 300000,
        deleteOnExpire: 'aggressive',
        recycleFreq: 60000,
        storageMode: 'sessionStorage'
    });
}),

methods = { POST: 'POST', GET: 'GET' };

cloudentsServices.config(['$httpProvider', function ($httpProvider) {
    $httpProvider.defaults.headers.common["X-Requested-With"] = 'XMLHttpRequest';
}]);

cloudentsServices.factory('Dashboard', function ($http) {
    return {
        boxList: function (payload) {
            submitRequest($http, '/Dashboard/BoxList', methods.GET, payload.data, payload.success, payload.error);
        }
    };
});
cloudentsServices.factory('User', function ($http) {
    return {
        friends: function (payload) {
            submitRequest($http, '/User/Friends', methods.GET, payload.data, payload.success, payload.error);
        }
    };
});

cloudentsServices.factory('Box', function ($http) {
    return {
        create: function (payload) {
            submitRequest($http, '/Dashboard/Create', methods.POST, payload.data, payload.success, payload.error);
        },
        remove: function (payload) {
            submitRequest($http, '/Box/Delete2', methods.POST, payload.data, payload.success, payload.error);
        },
        update: function (payload) {
            submitRequest($http, '/Box/UpdateInfo', methods.POST, payload.data, payload.success, payload.error);
        }

    };
});

cloudentsServices.factory('QuizService', function ($http,$q) {
    return {
        create: function (data) {
            var dfd = $q.defer();
            $http.post('/Quiz/Create', data).success(function (data, status) {
                dfd.resolve(data);
            }).error(function (data, status) {
                dfd.reject(data);
            });

            return dfd.promise;
        },
        update: function (data) {
            var dfd = $q.defer();
            $http.post('/Quiz/Update', data).success(function (data, status) {
                dfd.resolve(data);
            }).error(function (data, status) {
                dfd.reject(data);
            });

            return dfd.promise;
        },
        'delete': function (data) {
            var dfd = $q.defer();
            $http.post('/Quiz/Delete', data).success(function (data, status) {
                dfd.resolve(data);
            }).error(function (data, status) {
                dfd.reject(data);
            });

            return dfd.promise;
        },
        save: function (data) {
            var dfd = $q.defer();
            $http.post('/Quiz/Save', data).success(function (data, status) {
                dfd.resolve(data);
            }).error(function (data, status) {
                dfd.reject(data);
            });

            return dfd.promise;
        },
        getDraft: function (data) {
            var dfd = $q.defer();
            $http.get('/Quiz/GetDraft', { params: data }).success(function (data, status) {
                dfd.resolve(data);
            }).error(function (data, status) {
                dfd.reject(data);
            });
            return dfd.promise;

        },
        question: {
            create: function (data) {
                var dfd = $q.defer();
                $http.post('/Quiz/CreateQuestion', data).success(function (data, status) {
                    dfd.resolve(data);
                }).error(function (data, status) {
                    dfd.reject(data);
                });
                return dfd.promise;

            },
            update: function (data) {
                var dfd = $q.defer();
                $http.post('/Quiz/UpdateQuestion', data).success(function (data, status) {
                    dfd.resolve(data);
                }).error(function (data, status) {
                    dfd.reject(data);
                });
                return dfd.promise;

            },
            'delete': function (data) {
                var dfd = $q.defer();
                $http.post('/Quiz/DeleteQuestion', data).success(function (data, status) {
                    dfd.resolve(data);
                }).error(function (data, status) {
                    dfd.reject(data);
                });
                return dfd.promise;

            }
        },
        answer: {
            create: function (data) {
                var dfd = $q.defer();
                $http.post('/Quiz/CreateAnswer', data).success(function (data, status) {
                    dfd.resolve(data);
                }).error(function (data, status) {
                    dfd.reject(data);
                });
                return dfd.promise;

            },
            update: function (data) {
                var dfd = $q.defer();
                $http.post('/Quiz/UpdateAnswer', data).success(function (data, status) {
                    dfd.resolve(data);
                }).error(function (data, status) {
                    dfd.reject(data);
                });
                return dfd.promise;

            },
            'delete': function (data) {
                var dfd = $q.defer();
                $http.post('/Quiz/DeleteAnswer', data).success(function (data, status) {
                    dfd.resolve(data);
                }).error(function (data, status) {
                    dfd.reject(data);
                });
                return dfd.promise;

            },
            markCorrect: function (data) {
                var dfd = $q.defer();
                $http.post('/Quiz/MarkCorrect', data).success(function (data, status) {
                    dfd.resolve(data);
                }).error(function (data, status) {
                    dfd.reject(data);
                });
                return dfd.promise;

            }
        },
        discussion: {
            createDiscussion: function (data) {
                var dfd = $q.defer();
                $http.post('/Quiz/CreateDiscussion', data).success(function (data, status) {
                    dfd.resolve(data);
                }).error(function (data, status) {
                    dfd.reject(data);
                });
                return dfd.promise;

            },
            deleteDiscussion: function (data) {
                var dfd = $q.defer();
                $http.post('/Quiz/DeleteDiscussion', data).success(function (data, status) {
                    dfd.resolve(data);
                }).error(function (data, status) {
                    dfd.reject(data);
                });
                return dfd.promise;

            }
        }

    };
});

cloudentsServices.factory('PartialView', function ($http) {
    return {
        fetch: function (payload) {
            submitRequest($http, '/Home/Partial', methods.GET, payload.data, payload.success, payload.error);
        }
    };
});