(function () {
    'use strict';
    angular.module('app.box.items').controller('TabsController', tabs);
    tabs.$inject = ['boxService', '$stateParams', '$rootScope', 'itemThumbnailService', '$mdDialog',
        '$scope', '$q', 'resManager', '$state'];

    function tabs(boxService, $stateParams, $rootScope, itemThumbnailService, $mdDialog, $scope, $q, resManager, $state) {
        var t = this,
            boxId = $stateParams.boxId;
            
        $scope.stateParams = $stateParams;
        //t.tabId = $stateParams.tabId,
        //item = $scope.i;
        t.uploadShow = true;
        t.tabChange = tabChange;
        t.upDir = upDir;
        t.addTab = addTab;
        t.renameTab = renameTab;
        t.renameTabOpen = renameTabOpen;
        t.deleteTab = deleteTab;

        boxService.getTabs(boxId).then(function (data) {
            t.tabs = data;
            findSelectedItemFromUrl();
        });

        function findSelectedItemFromUrl() {
            if (!$stateParams.tabId) {
                return;
            }
            var item = t.tabs.find(function (x) {
                return x.id === $stateParams.tabId;
            });
            if (!item) {
                $state.go('box.items', { tabId: null });
            }
            t.tabSelected = item;
        }

        function renameTabOpen() {
            t.openRenameTab = true;
            t.tabNewName = t.tabSelected.name;
        }

        var submitDisabled = false;
        function addTab() {
            if (!t.newFolderName) {
                t.newFolderTabOpened = false;
                return;
            }
            if (submitDisabled) {
                return;
            }
            submitDisabled = true;
            boxService.createTab(t.newFolderName, boxId).then(function (response) {
                t.tabs.push(response);
                $scope.$emit('follow-box');
                t.newFolderTabOpened = false;
            }, function (response) {
                $scope.app.showToaster(response, 'tabSection');
            }).finally(function () {
                submitDisabled = false;
                t.newFolderName = '';
            });
        }

        function tabChange(tab) {
            $state.go('box.items', { tabId: tab.id });
            //$rootScope.$broadcast('close-collapse');
            //$scope.setTab(tab);
            //$scope.$emit('resetParams');
            //$scope.$emit('getItems');
        }

        function upDir() {
            t.openRenameTab = false;
            //tabChange({});
            $state.go('box.items', { tabId: null });
            //$stateParams.tabId = null;
        }

        function renameTab() {
            boxService.renameTab($stateParams.tabId, t.tabNewName, boxId).then(function () {
                t.tabSelected.name = t.tabNewName;
                t.openRenameTab = false;
            }, function (response) {
                $scope.app.showToaster(response, 'tabSection');
            });
        }

        function deleteTab(ev) {
            $scope.$emit('disablePaging');
            var confirm = $mdDialog.confirm()
                 .title(resManager.get('deleteTab'))
                 .targetEvent(ev)
                 .ok(resManager.get('dialogOk'))
                 .cancel(resManager.get('dialogCancel'));

            $mdDialog.show(confirm).then(function () {
                var index = t.tabs.indexOf(t.tabSelected);
                t.tabs.splice(index, 1);
                boxService.deleteTab($stateParams.tabId, boxId).then(function () {
                    upDir();
                });
            }).finally(function () {
                $scope.$emit('enablePaging');
            });
        }

        $scope.$watch(function () {
            return $state.params.tabId;
        }, function (newParams, oldParams) {
            if (newParams !== oldParams) {
                if (newParams) {
                    findSelectedItemFromUrl();
                } else {
                    t.tabSelected = null;
                }
            }
        });
        $scope.$on('tab-item-remove',
            function () {
                if (t.tabSelected) {
                    t.tabSelected.count--;
                }
            });
    }
})();