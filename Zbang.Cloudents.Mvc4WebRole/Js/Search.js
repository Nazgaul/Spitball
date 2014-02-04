(function (cd, pubsub, ko, dataContext, $, analytics) {
    if (window.scriptLoaded.isLoaded('search')) {
        return;
    }

    var eById = document.getElementById.bind(document);

    cd.loadModel('search', 'SearchContext', SearchViewModel);

    //function registerKOUser() {
    //    ko.applyBindings(new SearchViewModel(), document.getElementById('search'));
    //}


    function SearchViewModel() {
        pubsub.publish('search_load');
    }

})(cd, cd.pubsub, ko, cd.data, jQuery, cd.analytics);