app.controller('CheckoutCtrl',
    ['$scope', '$filter', '$timeout', '$window', '$routeParams', '$location', 'Store', 'sUserDetails',
    function ($scope, $filter, $timeout, $window, $routeParams, $location, Store, sUserDetails) {

        //ATTENTION: scope.products comes from ViewBag using bag-data directive
        $timeout(function () {
            $scope.product.categories = _.groupBy($scope.product.features, 'category');
            $scope.$emit('viewContentLoaded');
            if (sUserDetails.isAuthenticated()) {
                $scope.coupon.code = '100100';
                $scope.coupon.valid = true;
                $scope.validateCoupon();
            }

        });

        $scope.page = {
            step: 1
        };

        $scope.formData = {
            features: {},
            universityId: $routeParams.universityId || $routeParams.universityid || null,
            numberOfPayments:1
        };

        $scope.coupon = {
            valid: false
        };


        $scope.upgradeCost = function () {
            var cost = 0, cValue = 0;
            _.forEach($scope.formData.features, function (value, key, list) {
                if (value && angular.isNumber(value.price)) {
                    cValue = value.price;
                } else {
                    cValue = 0;
                }

                cost += cValue;
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
            var value;
            if (!$scope.coupon.valid) {
                value = 0;
            } else {
                value = $scope.product.coupon;
            }


            return value;
        };

        $scope.validateCoupon = function () {
            if (!$scope.coupon.code) {
                return;
            }

            var isNumber = /^\d+$/.test($scope.coupon.code);

            if (!isNumber) {
                alert('Invalid coupon');
                return;
            }

            $scope.coupon.buttonDisabled = true;

            Store.validateCoupon({ code: parseInt($scope.coupon.code, 10) }).then(function (response) {
                $scope.coupon.buttonDisabled = false;
                if (!response.success) {
                    return;
                }
                if (response.payload.isValid) {
                    $scope.coupon.valid = true;
                    return;
                }
            }, function () {
                $scope.coupon.buttonDisabled = false;
            });
            //var code = '1234';

            //if ($scope.coupon.code === code) {
            //    $scope.coupon.valid = true;
            //    return;
            //}



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
            $scope.order.buttonDisabled = true;
            $scope.formData.productId = $routeParams.productId;

            $scope.formData.features = _.map($scope.formData.features, function (feature, key) {
                return feature.id;
            });

            Store.order($scope.formData).then(function (response) {
                if (!response.success) {
                    alert(response.payload);
                    return;
                }
                $scope.order.buttonDisabled = false;
                $location.path(response.payload.url);
            });
        };
    }]
);
