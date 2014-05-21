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

cloudentsServices.factory('Quiz', function ($http) {
    return {
        getDraft: function (payload) {
            submitRequest($http, '/Quiz/GetDraft', methods.GET, payload.data, payload.success, payload.error);
        },
        create: function (payload) {
            submitRequest($http, '/Quiz/Create', methods.POST, payload.data, payload.success, payload.error);
        },
        update: function (payload) {
            submitRequest($http, '/Quiz/Update', methods.POST, payload.data, payload.success, payload.error);
        },
        'delete': function (payload) {
            submitRequest($http, '/Quiz/Delete', methods.POST, payload.data, payload.success, payload.error);
        },
        save: function (payload) {
            submitRequest($http, '/Quiz/Save', methods.POST, payload.data, payload.success, payload.error);
        },
        question: {
            create: function (payload) {
                submitRequest($http, '/Quiz/CreateQuestion', methods.POST, payload.data, payload.success, payload.error);
            },
            update: function (payload) {
                submitRequest($http, '/Quiz/UpdateQuestion', methods.POST, payload.data, payload.success, payload.error);
            },
            'delete': function (payload) {
                submitRequest($http, '/Quiz/SaveQuestion', methods.POST, payload.data, payload.success, payload.error);
            }

        },
        answer: {
            create: function (payload) {
                submitRequest($http, '/Quiz/CreateAnswer', methods.POST, payload.data, payload.success, payload.error);
            },
            update: function (payload) {
                submitRequest($http, '/Quiz/UpdateAnswer', methods.POST, payload.data, payload.success, payload.error);
            },
            'delete': function (payload) {
                submitRequest($http, '/Quiz/DeleteAnswer', methods.POST, payload.data, payload.success, payload.error);
            }
        },
        discussion: {
            createDiscussion: function (payload) {
                submitRequest($http, '/Quiz/DeleteAnswer', methods.POST, payload.data, payload.success, payload.error);
            },
            deleteDiscussion: function (payload) {
                submitRequest($http, '/Quiz/DeleteAnswer', methods.POST, payload.data, payload.success, payload.error);
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

function submitRequest($http, url, method, data, success, error) {
    $http({ method: method, url: url, params: data }).success(success).error(error);
}
