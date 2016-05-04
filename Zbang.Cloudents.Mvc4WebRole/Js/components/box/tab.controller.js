'use strict';
(function () {
    angular.module('app.box.items').controller('TabsController', tabs);
    tabs.$inject = ['boxService', '$stateParams', '$rootScope', 'itemThumbnailService', '$mdDialog',
        '$scope', '$q', 'resManager'];

    function tabs(boxService, $stateParams, $rootScope, itemThumbnailService, $mdDialog, $scope, $q, resManager) {
        var t = this,
        boxId = $stateParams.boxId,
        item = $scope.i;
        t.uploadShow = true;
        t.tabChange = tabChange;
        t.upDir = upDir;
        t.addTab = addTab;
        t.renameTab = renameTab;
        t.renameTabOpen = renameTabOpen;
        t.deleteTab = deleteTab;

        boxService.getTabs(boxId).then(function (data) {
            t.tabs = data;
        });

        function renameTabOpen() {
            t.openRenameTab = true;
            t.tabNewName = item.tabSelected.name;
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
            $rootScope.$broadcast('close-collapse');
            $scope.setTab(tab);
            $scope.$emit('resetParams');
            $scope.$emit('getItems');
        }

        function upDir() {
            t.openRenameTab = false;
            tabChange({});
        }

        function renameTab() {
            boxService.renameTab(item.tabSelected.id, t.tabNewName, boxId).then(function () {
                item.tabSelected.name = t.tabNewName;
                t.openRenameTab = false;
            }, function (response) {
                $scope.app.showToaster(response, 'tabSection');
            });
        }

        function deleteTab(ev) {
            var confirm = $mdDialog.confirm()
                 .title(resManager.get('deleteTab'))
                 .targetEvent(ev)
                 .ok(resManager.get('dialogOk'))
                 .cancel(resManager.get('dialogCancel'));

            $mdDialog.show(confirm).then(function () {
                var index = t.tabs.indexOf(item.tabSelected);
                t.tabs.splice(index, 1);
                boxService.deleteTab(item.tabSelected.id, boxId).then(function () {
                    upDir();
                });
            });
        }
    }
})();