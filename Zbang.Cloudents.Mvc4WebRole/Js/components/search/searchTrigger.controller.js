(function () {
    angular.module('app.search').controller('SearchTriggerController', searchTriggerController);
    searchTriggerController.$inject = ['$state', '$rootScope', '$stateParams'];

    function searchTriggerController($state, $rootScope, $stateParams) {
        var st = this;

        st.search = search;
        st.change = search;

        st.term = $stateParams.q;

        function search(isValid) {
            if (isValid) {
                //var url = $state.href('searchinfo', { q: st.term });
                //url += '?q=' + st.term;
                //$location.url(url);
                $state.go('searchinfo', { q: st.term, t: $stateParams.t });
                if ($state.current.name === 'searchinfo') {
                  //  $rootScope.$broadcast('search-query');
                }
            }
        }

        $rootScope.$on('search-close', function () {
            st.term = '';
        });
    }
})()