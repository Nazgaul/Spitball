app.directive('dropZone', ['$modalStack', '$analytics', function ($modalStack, $analytics) {
    "use strict";
    return {
        link: function (scope, elem, attrs) {

            var collection = $();


            document.addEventListener('dragenter', function (e) {
                if (collection.length === 0) {
                    if ($modalStack.getTop()) {
                        return;
                    }
                    if (e.dataTransfer.types.indexOf('Files') === -1) {
                        return;
                    }
                    $analytics.eventTrack('Drag Enter', {
                        category: 'Box Items'
                    });
                    elem.show();
                }

                collection = collection.add(e.target);

            }, false);


            document.addEventListener('dragleave', function (e) {
                collection = collection.not(e.target);
                if (collection.length === 0) {
                    elem.hide();
                }
                $analytics.eventTrack('Drag Leave', {
                    category: 'Box Items'
                });
            });

            document.addEventListener('drop', function (e) {
                e.preventDefault();
                elem.hide();
                collection = $();
                $analytics.eventTrack('Drag Drop', {
                    category: 'Box Items'
                });
            });

        }
    };
}]
);