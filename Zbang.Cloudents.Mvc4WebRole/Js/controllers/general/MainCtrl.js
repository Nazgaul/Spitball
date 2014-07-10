app.controller('MainCtrl',
    ['$scope', '$rootScope', '$location', '$modal', 'sUser',
    function ($scope, $rootScope, $location, $modal, User) {
        $scope.partials = {
            shareEmail: '/Share/MessagePartial/'
        }



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
