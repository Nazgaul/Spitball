app.directive('dropZone', ['$modalStack', function ($modalStack) {
    return {
        link: function (scope, elem, attrs) {

            var collection = $();
            

            document.addEventListener('dragenter', function (e) {
                if (collection.length === 0) {
                    if ($modalStack.getTop()) {
                        return;
                    }
                    elem.show();                    
                }
                
                collection = collection.add(e.target);

            }, false);


            document.addEventListener('dragleave', function (e) {
                collection = collection.not(e.target);
                if (collection.length === 0) {
                    elem.hide();
                }
            });

            document.addEventListener('drop', function (e) {
                e.preventDefault();
                elem.hide();
                collection = $();
            });

        }
    };
}]
);