﻿mBox.controller('BoxTabsCtrl',
        ['$scope', '$rootScope', '$filter', '$analytics','sModal', 'sBox', 'sUserDetails','resManager',
        function ($scope, $rootScope, $filter, $analytics, sModal, sBox, sUserDetails, resManager) {
            "use strict";
            $scope.params = {};

            sBox.tabs({ id: $scope.boxId }).then(function (tabs) {
                $scope.params.tabs = $filter('orderBy')(tabs, 'name');
                $rootScope.$broadcast('update-scroll');
            });

            $scope.deleteTab = function (tab) {
                var data = {
                    boxId: $scope.boxId,
                    TabId: tab.id
                }
                sBox.deleteTab(data).then(function () { }, function () {
                    alert(resManager.get('DeleteError'));

                });


                $analytics.eventTrack('Box Tabs', {
                    category: 'Removed Tab'
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

                $analytics.eventTrack('Box Tabs', {
                    category: 'Rename Tab'
                });
            };

            $scope.createTab = function () {
                if (!sUserDetails.isAuthenticated()) {
                    cd.pubsub.publish('register', { action: true });
                    return;
                }

                if ($scope.info.userType === 'invite' || $scope.info.userType === 'none') {
                    alert(resManager.get('NeedToFollowBox'));
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
                            $scope.params.tabs.push(tab);
                            $scope.params.tabs = $filter('orderBy')($scope.params.tabs, 'name');
                            $rootScope.$broadcast('update-scroll');
                        }
                    }
                });

                $analytics.eventTrack('Box Tabs', {
                    category: 'Create Tab'
                });
            };

            $scope.manageTab = function () {
                $rootScope.$broadcast('manageTab');
                $analytics.eventTrack('Box Tabs', {
                    category: 'Manage Tab'
                });
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

                $analytics.eventTrack('Box Tabs', {
                    category: 'Dragged Item',
                    label: 'User dragged an item to a tab'
                });

                $rootScope.$broadcast('tabItemAdded', data);
            };
        }]);