angular.module('stackTrace',[]).factory('stackTraceService', function () {
    return {
        print: printStackTrace
    };
});