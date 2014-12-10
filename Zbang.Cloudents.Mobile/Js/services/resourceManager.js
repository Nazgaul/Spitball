app.factory('resManager',
    ['$filter','$location',
    function ($filter,$location) {
        "use strict";

        var jsResources = window.JsResources;

        var manager = {
            get: function (value) {
                var result = '';
                if (_.isEmpty(value)) {
                    return result;
                }

                var resource = jsResources[value];

                if (_.isEmpty(resource)) {
                    logError('missing resource',value);
                    return result;
                }

                return resource;

            },
            getParsed: function (value, params) {
                var string = this.get(value),
                    result = '';

                if (_.isEmpty(string)) {
                    return result;
                }
                
                result = $filter('stringFormat')(string, params);

                if (result.indexOf('{') > -1) {
                    logError('missing params',value, params);
                }

                return result;
            }
        }

        return manager;

        function logError(cause,value,params) {
            $.ajax({
                type: 'POST',
                url: '/Error/JsLog',
                contentType: 'application/json',
                data: angular.toJson({
                    errorUrl: $location.absUrl(),
                    errorMessage: value + ';' + params,
                    cause: 'jsResources ' + cause,
                    stackTrace: ''
                })
            });
        }
        


    }
    ]);

