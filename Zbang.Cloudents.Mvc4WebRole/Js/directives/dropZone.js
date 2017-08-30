app.directive('dropZone', ['$modalStack', '$analytics', function ($modalStack, $analytics) {
    "use strict";
    return {
        link: function (scope, elem) {
            var collection = $();
            document.addEventListener('dragenter', function (e) {
                if (collection.size() === 0) {
                    if ($modalStack.getTop()) {                        
                        return;
                    }
                    try {
                        if (e.dataTransfer.types.indexOf('Files') === -1) {
                            return;
                        }
                    } catch (ex) {

                    }
                    
                    $analytics.eventTrack('Drag Enter', {
                        category: 'Upload'
                    });
                    elem.show();
                }
                collection = collection.add(e.target);

                

            });


            document.addEventListener('dragleave', function (e) {
                setTimeout(function () {
                    collection = collection.not(e.target);
                    if (collection.length === 0) {
                        elem.hide();
                        $analytics.eventTrack('Drag Leave', {
                            category: 'Upload'
                        });
                    }
                }, 1);
                
            });

            document.addEventListener('drop', function (e) {
                e.preventDefault();
                elem.hide();
                collection = $();
                $analytics.eventTrack('Drag Drop', {
                    category: 'Upload'
                });
            });

        }
    };
}]
);