(function () {
    angular.module('app.user.account').controller('AccountSettings', account);
    account.$inject = ['userDetailsService', '$scope', '$timeout'];

    function account(userDetailsService, $scope, $timeout) {
        var self = this;


        userDetailsService.getAccountDetails().then(function (response) {
            console.log(response);
            self.data = response;
        });

        //University auto complete
        self.querySearch = function querySearch(query) {
            return userDetailsService.searchUniversity(query);
        };
        self.selectedItemChange = function selectedItemChange(item) {
            self.data.university = item.name;
            self.data.universityId = item.id;
            //console.log('Item changed to ' + JSON.stringify(item));
        };

        self.submit = function () {
            var firstName = self.firstName || self.data.firstName,
            lastName = self.lastName || self.data.lastName;
            userDetailsService.setAccountDetails(self.data.universityId, firstName, lastName, self.data.university).then(function (response) {
                alert('changes saved');
                self.firstName = '';
                self.lastName = '';
            });
        }


        //ud.setAccountDetails

        $scope.$on('$viewContentLoaded', function () {
            //taken from metronic.js
            if (location.hash) {
                var tabid = encodeURI(location.hash.substr(1));
                $('a[href="#' + tabid + '"]').parents('.tab-pane:hidden').each(function () {
                    tabid = $(this).attr("id");
                    $('a[href="#' + tabid + '"]').click();
                });
                $('a[href="#' + tabid + '"]').click();
            }
        });

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
                filesAdded: function (uploader, files) {
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