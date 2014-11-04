
mBox.controller('BoxTabsCtrl',
        ['$scope', '$rootScope', '$filter', 'sModal', 'sBox', 'sUserDetails',
        function ($scope, $rootScope, $filter, sModal, sBox, sUserDetails) {
            "use strict";
            var jsResources = window.JsResources;
            $scope.params = {};
         
            sBox.tabs({ id: $scope.boxId }).then(function (response) {
                var data = response.success ? response.payload : [];
                $scope.params.tabs = data;
                $rootScope.$broadcast('update-scroll');
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
                sModal.open('tab', {
                    data: {
                        boxId: $scope.boxId,
                        tabName: tab.name,
                        tabId: tab.id
                    },
                    callback: {
                        close: function (name) {
                            tab.name = name;
                        }
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

                sModal.open('tab', {
                    data: {
                        boxId: $scope.boxId
                    },
                    callback: {
                        close: function (tab) {
                            if (!$scope.params.tabs) {
                                $scope.params.tabs = [];
                            }
                            $scope.params.tabs.unshift(tab);
                            $rootScope.$broadcast('update-scroll');
                        }
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
                $rootScope.$broadcast('tabItemAdded', data);
            };
        }]);