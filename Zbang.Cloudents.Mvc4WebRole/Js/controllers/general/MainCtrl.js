app.controller('MainCtrl',
    ['$scope', '$rootScope', '$location', '$modal', 'sUser','sFacebook',
    function ($scope, $rootScope, $location, $modal, User,Facebook) {
        $scope.partials = {
            shareEmail: '/Share/MessagePartial/'
        }


        $rootScope.options = {};
        $rootScope.back = {};


        $rootScope.$back = function (url) {
            if (url && url.length) {
                $location.path(url);
            }
        };

        $scope.$on('tooltipLoaded', function (e, userId) {
            User.minProfile({ userId: userId }).then(function (response) {
                $scope.userTooltip = response.payload;
            });
        });
        $scope.$on('tooltipUnloaded', function () {
            $scope.userTooltip = {};
        });


        $scope.$on('message', function (e, user) {
            if (user) {
                $scope.sendUserMessage(user);
                return;
            }

            var modalInstance = $modal.open({
                templateUrl: $scope.partials.shareEmail,
                controller: 'ShareCtrl',
                backdrop: 'static',
                resolve: {
                    data: function () {
                        return null;
                    }
                }
            });

            modalInstance.result.then(function () {
            }, function () {
                //dismiss
            });
        }); //temp

        $scope.$on('messageFB', function (e, obj) {
            Facebook.share(obj.url, obj.name, obj.caption, obj.description, obj.picture);
        });

        $scope.sendUserMessage = function (user) {
            var modalInstance = $modal.open({
                templateUrl: $scope.partials.shareEmail,
                controller: 'ShareCtrl',
                backdrop: 'static',
                resolve: {
                    data: function () {
                        return {
                            singleMessage: true,
                            user: user
                        };
                    }
                }
            });

            modalInstance.result.then(function () {
            }, function () {
                //dismiss
            });
        };
    }
    ]);
