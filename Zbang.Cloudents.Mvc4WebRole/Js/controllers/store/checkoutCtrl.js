app.controller('CheckoutCtrl',
    ['$scope','$rootScope', '$filter', '$timeout', '$window', '$routeParams', '$location', 'Store', 'sUserDetails', 'sFocus',
    function ($scope,$rootScope, $filter, $timeout, $window, $routeParams, $location, Store, sUserDetails, sFocus) {
        "use strict";
        //ATTENTION: scope.products comes from ViewBag using bag-data directive
        $scope.product = $scope.viewBag;

        $timeout(function () {
            $scope.product.categories = _.groupBy($scope.product.features, 'category');
            $scope.$emit('viewContentLoaded');
            if (sUserDetails.isAuthenticated()) {
                $scope.coupon.code = '100100';
                $scope.coupon.valid = true;
                $scope.coupon.buttonDisabled = true;
            }
            
            if ($rootScope.params.store.coupon && $rootScope.params.store.coupon.valid) {
                $scope.coupon.code = $rootScope.params.store.coupon.code;
                $scope.coupon.valid = true;
                $scope.coupon.buttonDisabled = true;
            }

        });

        $scope.page = {
            step: 1
        };

        $scope.formData = {
            features: {},
            universityId: $routeParams.universityId || $routeParams.universityid || null,
            numberOfPayments: 1
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
            var invalidCouponMessage = 'קופון שגוי';
            if (!$scope.coupon.code) {
                return;
            }

            var isNumber = /^\d+$/.test($scope.coupon.code);

            if (!isNumber) {
                alert(invalidCouponMessage);
                return;
            }

            $scope.coupon.buttonDisabled = true;

            Store.validateCoupon({ code: parseInt($scope.coupon.code, 10) }).then(function (response) {                
                if (response.isValid) {
                    $scope.coupon.valid = true;
                    return;
                }
                alert(invalidCouponMessage);
            }, function () {
            
            }).finally(function () {
                $scope.coupon.buttonDisabled = false;
            });
        };

        $scope.nextStep = function () {
            if (!$scope.coupon.valid) {
                alert('אנא הכנס הפעל קוד קופון');
                sFocus('storeCoupon');
                return;
            }
            $scope.page.step = 2;
            $window.scrollTo(0, 0);
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
                $location.path(response.url);
                $location.search('productId', $routeParams.productId)
            }, function (response) {
                alert(response);
            }).always(function () {
                $scope.order.buttonDisabled = false;
            });
        };
    }]
);
