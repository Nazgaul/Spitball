(function (pubsub, dataContext) {
    if (window.scriptLoaded.isLoaded('st')) {
        return;
    }

    var key = 'statistics',
    items = [];

    pubsub.subscribe('item', function (data) {
        items = JSON.parse(cd.localStorageWrapper.getItem(key)) || [];
        if (items.indexOf(data) == -1 ) {
            items.push({Uid: data, Action: 1});
            cd.localStorageWrapper.setItem(key, JSON.stringify(items));
        }

    });
    pubsub.subscribe('item_Download', function (data) {
        items = JSON.parse(cd.localStorageWrapper.getItem(key)) || [];
        if (items.indexOf(data) == -1) {
            items.push({ Uid: data, Action: 2 });
            cd.localStorageWrapper.setItem(key, JSON.stringify(items));
            
        }
    });

    pubsub.subscribe('quiz', function (data) {
        items = JSON.parse(cd.localStorageWrapper.getItem(key)) || [];
        if (items.indexOf(data) == -1) {
            items.push({ Uid: data, Action: 3 });
            cd.localStorageWrapper.setItem(key, JSON.stringify(items));
        }
    });

    window.setInterval(sendData, 300000); // 5 minutes
    window.setTimeout(sendData, 60000);
    //sendData();

    function sendData() {
        var x = JSON.parse(cd.localStorageWrapper.getItem(key));
        //if (x) {
        dataContext.statistics({
            data: { Items: x },
            //error function not to go to error page
            error: function () { }
        });

        cd.localStorageWrapper.removeItem(key);
        //}
    }

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
        pubsub.subscribe('dash_boxes', function () {
            dataContext.firstTimeUpdate({
                data: { firstTime: 'Dashboard' }
            });
        });
    }
    if (ftLib) {
        pubsub.subscribe('lib_nodes', function () {
            dataContext.firstTimeUpdate({
                data: { firstTime: 'Library' }
            });
        });
    }
    if (ftBox) {
        pubsub.subscribe('box', function () {
            dataContext.firstTimeUpdate({
                data: { firstTime: 'Box' }
            });
        });
    }
    if (ftItem) {
        pubsub.subscribe('item', function () {
            dataContext.firstTimeUpdate({
                data: { firstTime: 'Item' }
            });
        });
    }
})(cd.pubsub, cd.data);