mAccount.controller('LoginCtrl',
    ['$scope', '$window', '$route', '$routeParams', 'sFacebook', 'sAccount', '$analytics', '$angularCacheFactory', 'sFocus', '$location',
function ($scope, $window, $route, $routeParams, sFacebook, sAccount,
    $analytics, $angularCacheFactory, sFocus, $location) {
    "use strict";

    $scope.params = {
        pattern: /^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,4}$/i
    };
    $scope.disabled = false;

    $scope.params.states = {
        registerFirst: 0,
        register: 1,
        login: 2
    };


    changeState($scope.data.state);

    $scope.data.formData = $scope.data.formData || {};

    $scope.formData = {
        login: $scope.data.formData.login || {},
        register: $scope.data.formData.register || {}
    };

    $scope.params.language = $scope.params.currentLanague;

    $scope.changeState = changeState;

    $scope.cancel = function () {
        $scope.close();
    };

    $scope.facebookLogin = function () {
        sFacebook.registerFacebook({ boxId: $routeParams.boxId }).then(function () {
            $window.location.reload();
        }, function () {
            $scope.close();
        });
    };

    $scope.login = function (isValid) {

        if (!isValid) {
            return;
        }
        $scope.disabled = true;
        //if (loginDisable) {
        //    return;
        //}

        //loginDisable = true;
        //$scope.params.loginServerError = null;

        sAccount.login($scope.formData.login).then(function () {
            var routeName = $route.current.$$route.params.type;

            if (routeName === 'account') {
                window.location.href = '/dashboard/';
                return;
            }

            $window.location.reload();


        }, function (response) {
            $scope.params.loginServerError = response[0].value[0];
            $scope.disabled = false;
        });
    };

    $scope.register = function (isValid) {
        if (!isValid) {
            return;
        }
        $scope.disabled = true;


        $scope.params.registerServerError = null;

        if ($routeParams.boxId) {
            $scope.formData.register.boxId = $routeParams.boxId;
        }

        sAccount.register($scope.formData.register).then(function () {
            var routeName = $route.current.$$route.params.type;
            var cache = $angularCacheFactory('points', {
                maxAge: 600000
            });

            cache.put('register', true);

            $analytics.pageTrack('hp/register/success');

            if ($routeParams.boxId) {
                $window.location.reload();
                return;
            }

            if (routeName === 'account') {
                window.location.href = '/dashboard/';
            }
            //for now we do postback all the time
            //else {
            //    $location.path('/library/choose/');

            //}


        }, function (response) {
            $analytics.pageTrack('hp/register/failed');
            $scope.params.registerServerError = response[0].value[0];
            $scope.disabled = false;
        });
    };

    $scope.changeLanguage = function () {

        //$angularCacheFactory.get('htmlCache').removeAll();

        $analytics.eventTrack('Language Change', {
            category: 'Register Popup',
            label: 'User changed language to ' + $scope.params.language
        });

        sAccount.changeLocale({ language: $scope.params.language }).then(function () {

            var cache = $angularCacheFactory.get('changeLanguage') || $angularCacheFactory('changeLanguage');

            cache.put('formData', JSON.stringify({
                formData: {
                    login: $scope.formData.login,
                    register: $scope.formData.register,
                },
                currentState: $scope.params.currentState
            }));
            location.href = '/account/'; // we need to do full refresh for css and js to reload
        });
    };

    function changeState(state) {
        $scope.params.currentState = state;
        $scope.params.loginServerError = $scope.params.registerServerError = null;
        switch (state) {
            case 1:
                sFocus('register.firstname');
                break;
            case 2:
                sFocus('logon.email');
                break;

        }
    }

}
    ]);
