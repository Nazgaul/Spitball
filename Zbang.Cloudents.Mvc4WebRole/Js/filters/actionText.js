mDashboard.filter('actionText',
[
    function () {
        var dashFeed = document.getElementById('dash_feed'),
            dashWallTextItem = dashFeed.getAttribute('data-itemText'),
            dashWallTextComment = dashFeed.getAttribute('data-commenttext');

        return function (action) {
            switch (action) {
                case 'item': return dashWallTextItem;
                case 'question': return dashWallTextComment;
                case 'answer': return dashWallTextComment;
                default:
                    return '';
            }
        };
    }
]);
