
app.controller('ContactCtrl',
    ['$scope', '$timeout','Store',
    function ($scope, $timeout, Store) {
        "use strict";
        $timeout(function () {
            $scope.$emit('viewContentLoaded');
        });

        $scope.formData = {};

        $scope.submit = function (isValid) {
            if (!isValid) {
                return;
            }

            //TODO analytics

            Store.contact($scope.formData).then(function() {               

                alert('Thank you');
                $scope.submitted = false;

                $scope.contactForm.$setPristine();
                $scope.formData = {
                    name: null,
                    phone: null,
                    email: null,
                    university: null,
                    text: null
                };

            }, function () {
                alert(response);
            });


        };

    }]
);
