app.controller('CheckoutCtrl',
    ['$scope', '$timeout', '$window', '$routeParams', '$location', 'Store',
    function ($scope, $timeout, $window, $routeParams, $location, Store) {

        //ATTENTION: scope.products comes from ViewBag using bag-data directive
        $timeout(function () {
            $scope.product.categories = _.groupBy($scope.product.features, 'category');
            $scope.$emit('viewContentLoaded');
        });

        $scope.page = {
            step: 1
        };

        $scope.formData = {
            features: {}
        };

        $scope.coupon = {
            valid: false
        };

        $scope.upgradeCost = function () {
            var cost = 0;
            _.forEach($scope.formData.features, function (value, key, list) {
                cost += value.price;
            });

            return cost;
        };
        $scope.totalPrice = function () {
            var totalPrice = $scope.product.salePrice;

            if ($scope.coupon.valid) {
                totalPrice -= $scope.product.coupon;
            }

            totalPrice += $scope.upgradeCost();
            totalPrice += $scope.product.deliveryPrice;


            return totalPrice;

        };

        $scope.couponDiscount = function () {
            if (!$scope.coupon.valid) {
                return 0;
            }

            return $scope.product.coupon;
        };

        $scope.validateCoupon = function () {
            $scope.coupon.buttonDisabled = true;
            Store.validateCoupon({ code: $scope.coupon.code }).then(function (response) {
                if (!response.success) {
                    return;
                }
                if (response.payload.isValid) {
                    $scope.coupon.valid = true;
                    return;
                }
            });
            //var code = '1234';

            //if ($scope.coupon.code === code) {
            //    $scope.coupon.valid = true;
            //    return;
            //}

            $scope.coupon.buttonDisabled = false;

        };

        $scope.nextStep = function () {
            if ($scope.coupon.valid) {
                $scope.page.step = 2;
                $window.scrollTo(0, 0);
            }
        };

        $scope.order = function (isValid) {
            if (!isValid) {
                return;
            }
            $scope.formData.productId = $routeParams.productId;

            $scope.formData.features = _.map($scope.formData.features, function (feature, key) {
                return feature.id;
            });

            Store.order($scope.formData).then(function (response) {
                if (!response.success) {
                    alert(response.payload);
                    return;
                }

                $location.path(response.payload.url);
            });
        };
    }]
);
