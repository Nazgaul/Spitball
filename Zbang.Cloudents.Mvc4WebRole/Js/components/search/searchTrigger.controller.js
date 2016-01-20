(function () {
    angular.module('app.search').controller('SearchTriggerController', searchTriggerController);
    searchTriggerController.$inject = ['$state', '$rootScope', '$location'];

    function searchTriggerController($state, $rootScope, $location) {
        var st = this;

        st.search = search;
        st.change = search;

        st.term = $location.search().q;

        function search(isValid) {
            console.log($state);
            if (isValid) {
                var url = $state.href('search', { q: st.term });
                url += '?q=' + st.term;
                $location.url(url);
                if ($state.current.name === 'search') {
                    $rootScope.$broadcast('search-query');
                }
            }
        }

        $rootScope.$on('search-close', function () {
            st.term = '';
        });
    }
})()