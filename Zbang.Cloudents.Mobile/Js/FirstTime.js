(function ($, cd, dataContext) {
    var $firstTime = $('#firstTime'),
        ftLib = $firstTime.data('libf'),
        ftMy = $firstTime.data('myf'),
        ftBox = $firstTime.data('boxf'),
        ftItem = $firstTime.data('itemf');

    //None = 0,
    //Dashboard,
    //Library,
    //Box,
    //Item
    if (ftMy) {

        cd.pubsub.subscribe('dash_boxes', function () {
            dataContext.firstTimeUpdate({
                data: { firstTime: 'Dashboard' }
            });
        });
    }
    if (ftLib) {
        cd.pubsub.subscribe('lib_nodes', function () {
            dataContext.firstTimeUpdate({
                data: { firstTime: 'Library' }
            });
        });
    }
    if (ftBox) {
        cd.pubsub.subscribe('box', function () {
            dataContext.firstTimeUpdate({
                data: { firstTime: 'Box' }
            });
        });
    }
    if (ftItem) {
        cd.pubsub.subscribe('item', function () {
            dataContext.firstTimeUpdate({
                data: { firstTime: 'Item' }
            });
        });
    }


})(jQuery, cd, cd.data);