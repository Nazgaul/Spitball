mBox.controller('BoxTabsCtrl',
[
    '$scope', '$rootScope', '$filter', '$analytics', 'sModal', 'sBox', 'sUserDetails', 'sNotify', 'sLogin', 'sTabCount', '$location',
    function ($scope, $rootScope, $filter, $analytics, sModal, sBox, sUserDetails, sNotify, sLogin, sTabCount, $location) {
        "use strict";
        $scope.params = {};

        sTabCount.countAll(showTabCount);
        sTabCount.add(addCount);
        sTabCount.remove(removeCount);
        sTabCount.set(setCount);

        sBox.tabs({ id: $scope.boxId }).then(function (tabs) {
            $scope.params.tabs = tabs;

            var tabId = $location.search().tabId;
            if (tabId) {
                var tab = _.find($scope.params.tabs, function (tab2) {
                    return tab2.id === tabId;
                });
                $scope.params.currentTab = tab;
                $rootScope.$broadcast('selectTab', tab);
            }
            $rootScope.$broadcast('update-scroll');
        });

        $scope.deleteTab = function (tab) {
            var data = {
                boxId: $scope.boxId,
                TabId: tab.id
            }
            sBox.deleteTab(data).then(function () { }, function () {
                sNotify.tAlert('DeleteError');
            });


            $analytics.eventTrack('Tabs - Removed Tab', {
                category: 'Box'
            });

            var index = $scope.params.tabs.indexOf(tab);
            $scope.params.tabs.splice(index, 1);
            $scope.params.currentTab = null;
            $scope.params.manageTab = false;
            $rootScope.$broadcast('update-scroll');
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

            $analytics.eventTrack('Tabs - Rename Tab', {
                category: 'Box'
            });
        };

        $scope.createTab = function () {
            if (!sUserDetails.isAuthenticated()) {
                sLogin.registerAction();
                return;
            }

            if ($scope.info.userType === 'invite' || $scope.info.userType === 'none') {
                sNotify.tAlert('NeedToFollowBox');
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
                        tab.itemCount = 0;
                        $rootScope.$broadcast('update-scroll');
                    }
                }
            });

            $analytics.eventTrack('Tabs - Create Tab', {
                category: 'Box'
            });
        };

        $scope.manageTab = function () {
            $rootScope.$broadcast('manageTab');
            $analytics.eventTrack('Tabs - Manage Tab', {
                category: 'Box'
            });
        };

        $scope.selectTab = function (tab) {
            if (!tab) {
                $location.search({ tabId: null });
                $scope.params.currentTab = null;
                $rootScope.$broadcast('selectTab', null);
                return;
            }
            $location.search({ tabId: tab.id });
        };

        $scope.addDraggedItem = function (item, tab) {
            $analytics.eventTrack('Tabs - Dragged Item', {
                category:'Box',
                label: 'User dragged an item to a tab'
            });

            var data = {
                boxId: $scope.boxId,
                tabId: tab.id, //tabId from draganddrop
                itemId: item.id,
                nDelete: !tab.id //delete is false if only one item added from draganddrop
            };

            $rootScope.$broadcast('tabItemAdded', { tabId: tab.id, itemId: item.id });

            sBox.addItemsToTab(data).then(function () { }, function () {
                sNotify.tAlert('FolderItemError');
            });

            tab.itemCount++;
        };

        $scope.$on('$routeUpdate', function () {
            var tabId = $location.search().tabId,
                tab = _.find($scope.params.tabs, function (tab2) {
                    return tab2.id === tabId;
                });
            $scope.params.currentTab = tab;
            $rootScope.$broadcast('selectTab', tab);
            $rootScope.$broadcast('update-scroll');

        });

        function showTabCount(count) {
            _.forEach($scope.params.tabs, function (tab) {
                tab.itemCount = count[tab.id] || 0;
            });
        }

        function addCount(tabId) {
            var tab = getTab(tabId);
            if (!tab) {
                return;
            }

            tab.itemCount++;
        }

        function removeCount(tabId) {
            var tab = getTab(tabId);
            if (!tab) {
                return;
            }

            tab.itemCount--;
        }

        function setCount(tabId, count) {
            var tab = getTab(tabId);
            if (!tab) {
                return;
            }

            tab.itemCount = count;
        }

        function getTab(tabId) {
            return _.find($scope.params.tabs, function (tab) {
                return tab.id === tabId;
            });
        }
    }
]);
mBox.factory('sTabCount',
[function () {
    "use strict";
    var countAll, add, remove, set;
    return {
        notifyAll: function (count) {
            countAll(count);
        },
        notifyRemove: function (tabId) {
            remove(tabId);
        },
        notifyAdd: function (tabId) {
            add(tabId);
        },
        notifySet: function (tabId, count) {
            set(tabId, count);
        },
        countAll: function (cb) {
            countAll = cb;
        },
        add: function (cb) {
            add = cb;
        },
        remove: function (cb) {
            remove = cb;
        },
        set: function (cb) {
            set = cb;
        }

    };
}
]);

