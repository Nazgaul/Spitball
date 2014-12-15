sStore.controller('StoreCtrl',
    ['$scope', '$route', 'sModal', 'sUserDetails',
    function ($scope, $route, sModal, sUserDetails) {
        "use strict";

        $scope.tab = {};

        $scope.setCurrentTab = function () {
            var routeName = $route.current.$$route.params.type,
                params = $route.current.params,
                currentTab;

            switch (routeName) {
                case 'product':
                case 'products':
                case 'checkout':
                    currentTab = 1;
                    break;
                case 'sales':
                    currentTab = 2;
                    break;
                case 'about':
                    currentTab = 3;
                    break;
                case 'contact':
                    currentTab = 4;
                    break;
                default:
                    currentTab = 0;
            }



            if (currentTab === 1 && params && params.categoryId === '646') {
                currentTab = 2;
            }

            $scope.tab.current = currentTab;
        };

        if (!sUserDetails.isAuthenticated()) {
            sModal.open('coupon');
        }

    }]
);
