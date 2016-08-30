(function () {
    angular.module('app').service('soundsService', sounds);

    function sounds() {
        var handlers = [];
        this.handlers = handlers;

        function onSoundAlert(handler) {
            handlers.push(handler);
        }
    }

})()