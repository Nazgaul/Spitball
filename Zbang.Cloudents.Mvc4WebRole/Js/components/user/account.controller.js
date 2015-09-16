(function () {
    angular.module('app.user.account').controller('AccountSettings', account);
    account.$inject = ['userDetailsService', '$scope', '$timeout'];

    function account(userDetailsService, $scope, $timeout) {
        var self = this;
        userDetailsService.getAccountDetails().then(function (response) {
            self.data = response;
        });

        self.simulateQuery = false;
        self.isDisabled = false;
        // list of `state` value/display objects
        //self.states = loadAll();
        self.querySearch = querySearch;
        self.selectedItemChange = selectedItemChange;
        self.searchTextChange = searchTextChange;

        function querySearch(query) {

            return userDetailsService.searchUniversity(query);

        }

        function searchTextChange(text) {
            console.log('Text changed to ' + text);
            // $log.info('Text changed to ' + text);
        }
        function selectedItemChange(item) {
            console.log('Item changed to ' + JSON.stringify(item));
        }

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
                max_file_size: '32mb',
                mime_types: [
                    { title: "Image files", extensions: "jpg,gif,png" }
                ]
                //headers: {
                //    'token': 'xxx token'
                //}
            },
            callbacks: {
                filesAdded: function (uploader, files) {
                    $scope.loading = true;
                    $timeout(function () {
                        uploader.start();
                    }, 1);
                },
                uploadProgress: function (uploader, file) {
                    $scope.loading = file.percent / 100.0;
                },
                fileUploaded: function (uploader, file, response) {
                    $scope.loading = false;
                    alert('Upload Complete!');
                },
                error: function (uploader, error) {
                    $scope.loading = false;
                    alert(error.message);
                }
            }
        }
    }
})();