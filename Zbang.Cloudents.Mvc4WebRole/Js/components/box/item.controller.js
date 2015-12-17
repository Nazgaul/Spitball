(function () {
    angular.module('app.box.items').controller('ItemsController', items);
    items.$inject = ['boxService', '$stateParams', '$rootScope', 'itemThumbnailService', '$mdDialog', '$scope', 'user'];

    function items(boxService, $stateParams, $rootScope, itemThumbnailService, $mdDialog, $scope, user) {
        var i = this,
        boxId = $stateParams.boxId;
        i.items = [];
        i.uploadShow = true;
        i.tabSelected = {};
        var page = 0, loading = false, needToBringMore = true;

        boxService.getTabs(boxId).then(function(data) {
            i.tabs = data;
        });

        

        i.myPagingFunction = function () {
            getItems(true);
        }
        i.filter = filter;
        i.openUpload = openUpload;
        i.deleteItem = deleteItem;
        i.tabChange = tabChange;
        i.upDir = upDir;
        i.addItemToTab = addItemToTab;
        i.dropToTabSuccess = dropToTabSuccess;
        i.addTab = addTab;
        i.renameTab = renameTab;
        i.renameTabOpen = renameTabOpen;
        i.deleteTab = deleteTab;
        getItems();

        function renameTabOpen() {
            i.openRenameTab = true;
            i.tabNewName = i.tabSelected.name;
        }

        function addTab() {
            if (!user.id) {
                $rootScope.$broadcast('show-unregisterd-box');
                return;
            }
            if (!i.newFolderName) {
                i.newFolderTabOpened = false;
            }
            boxService.createTab(i.newFolderName, boxId).then(function(response) {
                i.tabs.push(response);
                i.newFolderTabOpened = false;
            }, function(response) {
                $scope.app.showToaster(response, 'tabSection');
            });
        }

        function addItemToTab($data, tab) {
            boxService.addItemToTab(boxId, tab.id, $data.id);

        }
        function dropToTabSuccess(index) {
            i.items.splice(index, 1);
        }
        function tabChange(tab) {
            $rootScope.$broadcast('close-collapse');
            //i.tabSelectedId = tab.id;
            i.tabSelected = tab;
            resetParams();
            getItems();
        }

        function resetParams() {
            page = 0;
            needToBringMore = true;
        }

        function upDir() {
            tabChange({});
        }

        function renameTab() {
            boxService.renameTab(i.tabSelected.id, i.tabNewName, boxId);
            i.tabSelected.name = i.tabNewName;
            i.openRenameTab = false;
        }
        function deleteTab(ev) {
            var confirm = $mdDialog.confirm()
                 .title('Would you like to delete this tab?')
                 .targetEvent(ev)
                 .ok('Ok')
                 .cancel('Cancel');

            $mdDialog.show(confirm).then(function () {
                var index = i.tabs.indexOf(i.tabSelected);
                i.tabs.splice(index, 1);
                boxService.deleteTab(i.tabSelected.id, boxId).then(function () {
                    upDir();
                });
            });
        }

        function openUpload() {
            if (!user.id) {
                $rootScope.$broadcast('show-unregisterd-box');
                return;
            }
            $rootScope.$broadcast('open_upload', i.tabSelected.id);
            i.uploadShow = false;
        }

        function deleteItem(ev, item) {
            var confirm = $mdDialog.confirm()
                 .title('Would you like to delete this item?')
                 //.textContent('All of the banks have agreed to forgive you your debts.')
                 .targetEvent(ev)
                 .ok('Ok')
                 .cancel('Cancel');

            $mdDialog.show(confirm).then(function () {
                var index = i.items.indexOf(item);
                i.items.splice(index, 1);
                boxService.deleteItem(item.id, boxId);
            });
        }


        function getItems() {
            if (!loading && needToBringMore) {
                loading = true;

                boxService.items(boxId, i.tabSelected.id, page).then(function (response) {
                    response = itemThumbnailService.assignValues(response);
                    if (page > 0) {
                        i.items = i.items.concat(response);
                    } else {
                        i.items = response;
                    }

                    if (!response.length) {
                        needToBringMore = false;
                    }
                    page++;
                    loading = false;
                });
            }
        }
        function filter() {
            if (!i.term) {
                i.tabSelected = {};
                resetParams();
                getItems();
            }
            boxService.filterItem(i.term, boxId, 0).then(function (response) {
                response = itemThumbnailService.assignValues(response);
                i.items = response;
            });
        }


        //upload
        $rootScope.$on('item_upload', function (event, response) {
            if (response.boxId != $stateParams.boxId) { // string an int comarison
                return;
            }
            if (response.tabId != i.tabSelected.id) {
                return; //not the same tab
            }
            $scope.$emit('follow-box');
            var item = response.item, retVal = itemThumbnailService.assignValue(item.source);

            item.thumbnail = retVal.thumbnail;
            item.icon = retVal.icon;
            i.items.unshift(item);
        });
        $rootScope.$on('close_upload', function () {
            i.uploadShow = true;
        });
    }
})();