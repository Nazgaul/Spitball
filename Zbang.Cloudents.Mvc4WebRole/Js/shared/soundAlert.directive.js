(function () {
    'use strict';

    angular.module('app').directive('soundAlert', soundAlert);
    soundAlert.$inject = ['soundService'];

    function soundAlert(soundService) {
        return {
            scope: {
                onSoundAlert: '&'
            },
            link: function (scope, element, attrs) {
                var handler = scope.onSoundAlert({
                    chat: function () {
                        $('chat-sound').play();
                    }
                });

                scope.$on('$destroy', handler);
            }
        };
    }
})();