
mBox.controller('BoxTabsCtrl',
        ['$scope', '$rootScope', '$filter', 'sModal', 'sBox', 'sUserDetails',
        function ($scope, $rootScope, $filter, sModal, sBox, sUserDetails) {
            "use strict";
            var jsResources = window.JsResources;
            $scope.params = {};

            sBox.tabs({ id: $scope.boxId }).then(function (tabs) {
                $scope.params.tabs = tabs;
                $rootScope.$broadcast('update-scroll');
            });

            $scope.deleteTab = function (tab) {
                var data = {
                    boxId: $scope.boxId,
                    TabId: tab.id
                }
                sBox.deleteTab(data).then(function () { }, function () {
                    alert(jsResources.DeleteError);

                });


                //TODO analytics
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

                //TODO analytics
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

                //TODO analytics
            };

            $scope.manageTab = function () {
                $rootScope.$broadcast('manageTab');
                //TODO analytics
            };

            $scope.selectTab = function (tab) {
                $scope.params.currentTab = tab;
                $rootScope.$broadcast('selectTab', tab);
            };

            $scope.addDraggedItem = function (item, tabId) {
                var data = {
                    item: item,
                    tabId: tabId
                }

                //TODO analytics 
                $rootScope.$broadcast('tabItemAdded', data);
            };
        }]);