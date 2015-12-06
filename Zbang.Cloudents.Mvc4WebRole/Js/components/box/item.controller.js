(function () {
    angular.module('app.box.items').controller('ItemsController', items);
    items.$inject = ['boxService', '$stateParams', '$rootScope', 'itemThumbnailService', '$mdDialog'];

    function items(boxService, $stateParams, $rootScope, itemThumbnailService, $mdDialog) {
        var i = this,
        boxId = $stateParams.boxId;
        i.items = [];
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
        i.test = function(ev, x) {
            console.log(ev, x);
        }
        getItems();
        function tabChange(tab) {
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

       

        function openUpload() {
            $rootScope.$broadcast('open_upload');
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
            var item = response.item, retVal = itemThumbnailService.assignValue(item.source);

            item.thumbnail = retVal.thumbnail;
            item.icon = retVal.icon;
            i.items.unshift(item);
        });
        $rootScope.$on('close_upload', function () {
            i.uploadShow = true;
        });

        //function iterateItem() {
        //    for (var j = 0; j < i.items.length; j++) {
        //        if (i.items[j].thumbnail) {
        //            continue;
        //        }
        //        i.items[j].thumbnail = buildThumbnailUrl(i.items[j].source);
        //    }
        //}
        //function buildThumbnailUrl(name) {
        //    return itemThumbnail.get(name, 368, 520);
        //}


    }
})();