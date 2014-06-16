// Define you service here. Services can be added to same module as 'main' or a seperate module can be created.
angular.module('apiService', ['jmdobry.angular-cache']).config(['$angularCacheFactoryProvider', function ($angularCacheFactoryProvider) {
    $angularCacheFactoryProvider.setCacheDefaults({
        maxAge: 300000,
        deleteOnExpire: 'aggressive',
        recycleFreq: 60000,
        storageMode: 'sessionStorage'
    });
}]).
    config(['$httpProvider', function ($httpProvider) {
        $httpProvider.defaults.headers.common["X-Requested-With"] = 'XMLHttpRequest';
    }]).

    factory('Dashboard', ['$http', '$q', function ($http, $q) {
        return {
            boxList: function (data) {
                var dfd = $q.defer();
                $http.get('/Dashboard/BoxList', data).success(function (response) {
                    dfd.resolve(response);
                }).error(function (response) {
                    dfd.reject(response);
                });
                return dfd.promise;
            }
        };
    }]).
    factory('User', ['$http', '$q', function ($http, $q) {
        return {
            friends: function (data) {
                var dfd = $q.defer();
                $http.get('/User/Friends', data).success(function (response) {
                    dfd.resolve(response);
                }).error(function (response) {
                    dfd.reject(response);
                });
                return dfd.promise;
            }
        };
    }]).
    factory('Box', ['$http', '$q', function ($http, $q) {
        return {
            create: function (data) {
                var dfd = $q.defer();
                $http.post('/Dashboard/Create', data).success(function (response) {
                    dfd.resolve(response);
                }).error(function (response) {
                    dfd.reject(response);
                });

                return dfd.promise;
            },
            remove: function (data) {
                var dfd = $q.defer();
                $http.post('/Box/Delete2', data).success(function (response) {
                    dfd.resolve(response);
                }).error(function (response) {
                    dfd.reject(response);
                });
                return dfd.promise;
            },
            update: function (data) {
                var dfd = $q.defer();
                $http.post('/Box/UpdateInfo', data).success(function (response) {
                    dfd.resolve(response);
                }).error(function (response) {
                    dfd.reject(response);
                });

                return dfd.promise;
            }
        }
    }]).
    factory('QuizService', ['$http', '$q', function ($http, $q) {
        return {
            create: function (data) {
                var dfd = $q.defer();
                $http.post('/Quiz/Create', data).success(function (response) {
                    dfd.resolve(response);
                }).error(function (response) {
                    dfd.reject(response);
                });

                return dfd.promise;
            },
            update: function (data) {
                var dfd = $q.defer();
                $http.post('/Quiz/Update', data).success(function (response) {
                    dfd.resolve(response);
                }).error(function (response) {
                    dfd.reject(response);
                });

                return dfd.promise;
            },
            'delete': function (data) {
                var dfd = $q.defer();
                $http.post('/Quiz/Delete', data).success(function (response) {
                    dfd.resolve(response);
                }).error(function (response) {
                    dfd.reject(response);
                });

                return dfd.promise;
            },
            save: function (data) {
                var dfd = $q.defer();
                $http.post('/Quiz/Save', data).success(function (response) {
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
                    $http.post('/Quiz/CreateQuestion', data).success(function (response) {
                        dfd.resolve(response);
                    }).error(function (response) {
                        dfd.reject(response);
                    });
                    return dfd.promise;

                },
                update: function (data) {
                    var dfd = $q.defer();
                    $http.post('/Quiz/UpdateQuestion', data).success(function (response) {
                        dfd.resolve(response);
                    }).error(function (response) {
                        dfd.reject(response);
                    });
                    return dfd.promise;

                },
                'delete': function (data) {
                    var dfd = $q.defer();
                    $http.post('/Quiz/DeleteQuestion', data).success(function (response) {
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
                    $http.post('/Quiz/CreateAnswer', data).success(function (response) {
                        dfd.resolve(response);
                    }).error(function (response) {
                        dfd.reject(response);
                    });
                    return dfd.promise;

                },
                update: function (data) {
                    var dfd = $q.defer();
                    $http.post('/Quiz/UpdateAnswer', data).success(function (response) {
                        dfd.resolve(response);
                    }).error(function (response) {
                        dfd.reject(response);
                    });
                    return dfd.promise;

                },
                'delete': function (data) {
                    var dfd = $q.defer();
                    $http.post('/Quiz/DeleteAnswer', data).success(function (response) {
                        dfd.resolve(response);
                    }).error(function (response) {
                        dfd.reject(response);
                    });
                    return dfd.promise;

                },
                markCorrect: function (data) {
                    var dfd = $q.defer();
                    $http.post('/Quiz/MarkCorrect', data).success(function (response) {
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
                    $http.post('/Quiz/CreateDiscussion', data).success(function (response) {
                        dfd.resolve(response);
                    }).error(function (response) {
                        dfd.reject(response);
                    });
                    return dfd.promise;

                },
                deleteDiscussion: function (data) {
                    var dfd = $q.defer();
                    $http.post('/Quiz/DeleteDiscussion', data).success(function (response) {
                        dfd.resolve(response);
                    }).error(function (response) {
                        dfd.reject(response);
                    });
                    return dfd.promise;

                }
            }

        };
    }]).
    factory('NewUpdates', ['$http', function ($http) {
        var updates;
        return {
            getBoxUpdates: function (boxId) {
                boxId = parseInt(boxId, 10);
                var count = 0;

                if (!updates[boxId]) {
                    return '';
                }

                for (key in updates[boxId]) {
                    count += updates[boxId][key].length;
                }

                return count > 0 ? count : '';

            },
            isNew: function (boxId, type, id) {
                id = parseInt(id, 10);
  
                if (updates[boxId] && updates[boxId][type]) {
                    return updates[boxId][type].indexOf(id) > -1 ? true : false;
                }

                return false;
                
            },
            removeUpdates: function (boxId) {
                boxId = parseInt(boxId, 10);
                $http.post('/Box/DeleteUpdates', { boxId: boxId }).success(function () {
                    if (updates[boxId]) {
                        updates[boxId] = null;
                    }
                });
            },
            loadUpdates: function () {
                var update;
                updates = {};
                $http.get('/User/Updates').success(function (response) {
                    var data = response.payload;
                    for (var i = 0, l = data.length ; i < l; i++) {
                        update = data[i];
                        var boxId = parseInt(update.boxId);
                        if (!updates[update.boxId]) {
                            updates[boxId] = {};
                            updates[boxId].items = [];
                            updates[boxId].quizzes = [];
                            updates[boxId].questions = [];
                            updates[boxId].answers = [];

                        }
                        if (update.itemId) {
                            addUpdate(updates[boxId].items, update.itemId);
                            continue;
                        }
                        if (update.quizId) {
                            addUpdate(updates[boxId].quizzes, update.quizId);
                            continue;
                        }
                        if (update.questionId) {
                            addUpdate(updates[boxId].questions, update.questionId);
                            continue;
                        }
                        if (update.answerId) {
                            addUpdate(updates[boxId].answers, update.questionId);
                        }
                    }
                });

                function addUpdate(array, id) {
                    id = parseInt(id, 10);
                    if (array.indexOf(id) === -1) {
                        array.push(id);
                    };
                }
            }

        };        
    }]);

//cloudentsServices.factory('PartialView', ['$http', function ($http) {
//    return {
//        fetch: function (payload) {
//            submitRequest($http, '/Home/Partial', methods.GET, payload.data, payload.success, payload.error);
//        }
//    };
//}]);