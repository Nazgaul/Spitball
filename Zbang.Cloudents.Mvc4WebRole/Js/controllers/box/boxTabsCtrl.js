mBox.controller('BoxTabsCtrl',
        ['$scope', '$rootScope', '$filter', '$modal', 'sBox','sUserDetails',
        function ($scope, $rootScope, $filter, $modal, sBox, sUserDetails) {

            $scope.params = {};

            $scope.partials = {
                createTab: '/Box/CreateTabPartial/'
            };

            sBox.tabs({ id: $scope.boxId }).then(function (response) {
                var data = response.success ? response.payload : [];
                $scope.params.tabs = data;
            });

            $scope.deleteTab = function (tab) {
                var data = {
                    boxId: $scope.boxId,
                    TabId: tab.id
                }
                sBox.deleteTab(data).then(function (response) {
                    if (!response.success) {
                        alert(jsResources.DeleteError);
                        return;
                    }
                });

                var index = $scope.params.tabs.indexOf(tab);
                $scope.params.tabs.splice(index, 1);
                $scope.params.currentTab = null;
                $scope.params.manageTab = false;
                $rootScope.$broadcast('update_scroll');
            };

            $scope.renameTab = function (tab) {
                var modalInstance = $modal.open({
                    windowClass: "createTab",
                    templateUrl: $scope.partials.createTab,
                    controller: 'createTabCtrl',
                    backdrop: 'static',
                    resolve: {
                        data: function () {
                            return {
                                boxId: $scope.boxId,
                                tabName: tab.name,
                                tabId: tab.id
                            };
                        }
                    }
                });

                modalInstance.result.then(function (name) {
                    tab.name = name;
                }, function () {
                    //dismiss
                });

                $scope.$on('$destroy', function () {
                    if (modalInstance) {
                        modalInstance.close();
                    }
                });
            };

            $scope.createTab = function () {
                if (!sUserDetails.isAuthenticated()) {
                    cd.pubsub.publish('register', { action: true });
                    return;
                }

                if ($scope.info.userType === 'invite' || $scope.info.userType === 'none') {
                    alert(jsResources.NeedToFollowBox);
                    return;
                }

                var modalInstance = $modal.open({
                    windowClass: "createTab",
                    templateUrl: $scope.partials.createTab,
                    controller: 'createTabCtrl',
                    backdrop: 'static',
                    resolve: {
                        data: function () {
                            return {
                                boxId: $scope.boxId
                            };
                        }
                    }


                });

                modalInstance.result.then(function (tab) {
                    if (!$scope.params.tabs) {
                        $scope.params.tabs = [];
                    }
                    $scope.params.tabs.unshift(tab);
                    $rootScope.$broadcast('update_scroll');

                }, function () {
                    //dismiss
                });

                $scope.$on('$destroy', function () {
                    if (modalInstance) {
                        modalInstance.close();
                    }
                });
            };

            $scope.manageTab = function () {
                $rootScope.$broadcast('manageTab');
            };

            $scope.selectTab = function (tab) {
                //$scope.info.currentTab = tab;
                //if (!tab) { //all
                //    return;
                //}

                $scope.params.currentTab = tab;
                $rootScope.$broadcast('selectTab', tab);
            };

            $scope.addDraggedItem = function (item, tabId) {
                var data = {
                    item: item,
                    tabId: tabId
                }
                $rootScope.$broadcast('tabItemAdded',data);
            };
        }]);