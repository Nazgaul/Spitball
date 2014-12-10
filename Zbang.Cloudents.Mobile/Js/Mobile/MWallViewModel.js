(function (dataContext, ko, ZboxResources, cd) {
    "use strict";
    if (window.scriptLoaded.isLoaded('mWvm')) {
        return;
    }
    if (!cd.register()) {
        return;
    }
    ko.applyBindings(new WallViewMode(), document.getElementById('updates'));
    function WallViewMode() {
        var that = this;
        that.wall = ko.observableArray([]);
        var dashWall = document.getElementById('updates_wall'),
       dashWallTextItem = dashWall.getAttribute('data-itemText'),
       dashWallTextQuestion = dashWall.getAttribute('data-questionText'),
       dashWallTextAnswer = dashWall.getAttribute('data-answerText');


        function Activity(data) {
            data = data || {};
            var self = this;
            self.userName = data.userName;
            self.userUid = data.userId;
            self.userImg = data.userImage;

            self.boxid = data.boxId;
            self.boxName = data.boxName;
            self.boxurl = data.url;

            self.textAction = textActionReolver();
            function textActionReolver() {
                switch (data.Action) {
                    case 'item': return dashWallTextItem;
                    case 'question': return dashWallTextQuestion;
                    case 'answer': return dashWallTextAnswer;
                    default:
                        return '';

                }
            }

        }
        cd.pubsub.subscribe('dashSideD', function (d) {
            //var initData = {
            //    Wall: initDataWall,
            //    Friends: initDataFriend
            //};
            dataContext.dashboard({
                data: { order: "Name" },
                success: function (data) {
                    var mappedactivity = [];
                    for (var i = 0, length = data.wall.length; i < length; i++) {
                        mappedactivity.push(new Activity(data.wall[i]));
                    }
                    that.wall(mappedactivity);                    
                }                
            });            
        });

        //dataContext.getDSide({
        //    success: function (data) {
        //        var mappedactivity = [];
        //        for (var i = 0,length = data.Wall.length; i < length; i++) {
        //            mappedactivity.push(new Activity(data.Wall[i]));
        //        }
        //        that.wall(mappedactivity);
        //    }
        //});
    }
})(cd.data, ko, JsResources, cd);