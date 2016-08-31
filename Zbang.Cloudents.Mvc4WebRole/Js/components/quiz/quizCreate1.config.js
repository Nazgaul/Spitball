(function () {
    angular.module('app').config(config);

    config.$inject = ['$ocLazyLoadProvider'];

    function config($ocLazyLoadProvider) {
        $ocLazyLoadProvider.config({
            modules: [{
                name: 'quizCreate',
                serie: true,
                files: [
                   '/cdn/gzip/jFC71FED19C4367AD8795575E90F6B75E.js'
                ]
            }]
        });
    }
})();
