(function (pubsub, dataContext) {
    if (window.scriptLoaded.isLoaded('st')) {
        return;
    }

    var key = 'statistics',
    items = [];

    pubsub.subscribe('item', function (data) {
        items = JSON.parse(localStorage.getItem(key)) || [];
        if (items.indexOf(data.id) == -1 ) {
            items.push({Uid: data.id, Action: 1});
            localStorage.setItem(key, JSON.stringify(items));
        }

    });
    pubsub.subscribe('item_Download', function (data) {
        items = JSON.parse(localStorage.getItem(key)) || [];
        if (items.indexOf(data.id) == -1) {
            items.push({ Uid: data.id, Action: 2 });
            localStorage.setItem(key, JSON.stringify(items));
        }
    });

    window.setInterval(sendData, 300000); // 5 minutes
    sendData();

    function sendData() {
        var x = JSON.parse(localStorage.getItem(key));
        //if (x) {
        dataContext.statistics({
            data: { Items: x },
            //error function not to go to error page
            error: function () { }
        });

        localStorage.removeItem(key);
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