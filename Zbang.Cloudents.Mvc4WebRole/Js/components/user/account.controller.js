(function () {
    angular.module('app.user.account').controller('AccountSettingsController', account);
    account.$inject = ['userDetailsService'];

    function account(userDetailsService) {
        var self = this;


        userDetailsService.getAccountDetails().then(function (response) {
            self.data = response;
        });
    }
})();

(function() {
    angular.module('app.user.account').controller('AccountSettingsInfoController', info);
    info.$inject = ['userDetailsService', '$scope', '$timeout'];
    function info(userDetailsService, $scope, $timeout) {
        var self = this;
        self.querySearch = function querySearch(query) {
            return userDetailsService.searchUniversity(query);
        };
        self.selectedItemChange = function selectedItemChange(item) {
            self.data.university = item.name;
            self.data.universityId = item.id;

        };

        self.submit = function () {
            var firstName = self.firstName || self.data.firstName,
            lastName = self.lastName || self.data.lastName;
            userDetailsService.setAccountDetails(self.data.universityId, firstName, lastName, self.data.university).then(function () {
                alert('changes saved');
                self.firstName = '';
                self.lastName = '';
            });
        }


        self.fileUpload = {
            url: '/upload/profilepicture/',
            options: {
                multi_selection: false,
                max_file_size: '10mb',
                mime_types: [
                    { title: "Image files", extensions: "jpg,gif,png" }
                ]
            },
            callbacks: {
                filesAdded: function (uploader) {
                    //$scope.loading = true;
                    $timeout(function () {
                        uploader.start();
                    }, 1);
                },
                //uploadProgress: function (uploader, file) {
                //    $scope.loading = file.percent / 100.0;
                //},
                fileUploaded: function (uploader, file, response) {
                    // $scope.loading = false;
                    var obj = JSON.parse(response.response);
                    if (obj.success) {
                        self.data.image = obj.payload;
                        userDetailsService.changeImage(obj.payload);
                    }
                },
                error: function (uploader, error) {
                    //$scope.loading = false;
                    alert(error.message);
                }
            }
        }
    }
})();



(function () {
    angular.module('app.user.account').controller('AccountSettingsPasswordController', password);

    function password() {
        var self = this;



    }
})();
(function () {
    angular.module('app.user.account').controller('AccountSettingsNotificationController', notification);

    function notification() {
        var self = this;

    }
})();