(function () {
    angular.module('app.user.account').controller('AccountSettings', account);
    account.$inject = ['userDetailsService', '$timeout', '$q'];

    function account(userDetailsService, $timeout, $q) {
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
    }
})();