(function () {
    angular.module('app.box').service('boxService', box);
    box.$inject = ['ajaxService'];

    function box(ajaxservice) {
        var d = this;
        d.getBox = function (boxid) {
            return ajaxservice.get('/box/data/', { id: boxid });
        };
        d.getFeed = function (boxid) {
            return ajaxservice.get('/qna/', { id: boxid });
        };
        d.leaderBoard = function (boxid) {
            return ajaxservice.get('/box/leaderboard/', { id: boxid });
        }
        d.getRecommended = function (boxid) {
            return ajaxservice.get('/box/recommended/', { id: boxid });
        };
        d.items = function (boxId,  page) {
            return ajaxservice.get('/box/items/', { id: boxId, page: page });
        };
        d.getMembers = function (boxid) {
            return ajaxservice.get('/box/members/', { boxId: boxid });
        };
        d.getQuizzes = function (boxid) {
            return ajaxservice.get('/box/quizes/', { id: boxid });
        };
        

        d.postComment = function(content,boxId, files, anonymously) {
            return ajaxservice.post('/qna/addcomment/', { content: content, boxId: boxId, files: files, anonymously: anonymously });
        }
        d.postReply = function(content,boxId,commentId, files) {
            return ajaxservice.post('/qna/addreply/', {
                content: content,
                boxId: boxId,
                commentId: commentId,
                files: files
            });
        }
        d.follow = function(boxId) {
            return ajaxservice.post('/share/subscribetobox/', {
                boxId: boxId
            });

        }
        d.notification = function(boxId) {
            return ajaxservice.get('/box/getnotification/', {
                boxId: boxId
            });
        }
        d.unfollow = function (boxId) {
            return ajaxservice.post('/box/delete/', {
                id: boxId
            });
            
        }
        /*[Required]
        public long Id { get; set; }

        [Required]
        [StringLength(Box.NameLength)]
        public string Name { get; set; }

        public string CourseCode { get; set; }

        public string Professor { get; set; }

        public Zbox.Infrastructure.Enums.BoxPrivacySettings? BoxPrivacy { get; set; }

        public Zbox.Infrastructure.Enums.NotificationSettings Notification { get; set; }*/
        d.updateBox = function (boxId, name, course, professor, privacy, notification) {
            return ajaxservice.post('/box/updateinfo/', {
                id: boxId,
                name: name,
                courseCode: course,
                professor: professor,
                boxPrivacy: privacy,
                notification: notification
            });
        }
    }
})();