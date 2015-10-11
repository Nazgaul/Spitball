(function () {
    angular.module('app.box.items').controller('ItemsController', items);
    items.$inject = ['boxService', '$stateParams', '$scope', '$q'];

    function items(boxService, $stateParams, $scope, $q) {
        var i = this;
       

        $q.all([
            boxService.getItems($stateParams.boxId),
            boxService.getTabs($stateParams.boxId)
        ]).then(function (data) {
            i.items = data[0];
            for (var j = 0; j < i.items.length; j++) {
                i.items[j].thumbnail = 'https://az779114.vo.msecnd.net/preview/' + encodeURIComponent(i.items[j].source) + '.jpg?width=368&height=520&mode=crop'; // item.source
            }
            i.tabs = data[1];
        });

       


        i.changeTab = function (tabid) {
            i.tabSelectedId = tabid;
        }

        i.filterItems = function(item) {
            if (!i.tabSelectedId) {
                return true;
            }
            if (i.tabSelectedId === item.tabId) {
                return true;
            }
            return false;
        }

        //upload
        $scope.$on('item_upload', function (event, response) {
            if (response.boxId != $stateParams.boxId) { // string an int comarison
                return;
            }
            var item = response.fileDto;
            item.thumbnail = 'https://az779114.vo.msecnd.net/preview/' + encodeURIComponent(item.source) + '.jpg?width=368&height=520&mode=crop'; // item.source
            i.data.unshift(item);
        });
    }
})();