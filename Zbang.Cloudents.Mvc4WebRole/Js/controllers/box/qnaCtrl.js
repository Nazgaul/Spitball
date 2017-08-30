mBox.controller('QnACtrl',
['$scope', 'sModal', 'sUserDetails', 'sNewUpdates', 'sQnA', '$rootScope', 'sFocus',
    '$analytics', 'resManager', 'sNotify', 'sLogin', 'sGmfcnHandler', 'sItem', '$sce',
            function ($scope, sModal, sUserDetails, sNewUpdates, sQnA, $rootScope, sFocus, $analytics,
                resManager, sNotify, sLogin, sGmfcnHandler, sItem, $sce) {
                "use strict";
                function Question(data) {
                    var that = this;
                    data = data || {};
                    that.id = data.id;
                    that.userName = data.userName;
                    that.userImage = data.userImage;
                    that.userId = data.userId;
                    that.content = $sce.trustAsHtml(data.content ? data.content.replace(/\n/g, '<br/>') : '');
                    that.createTime = data.creationTime;
                    sNewUpdates.isNew($scope.boxId, 'questions', that.id, function (isNew) {
                        that.isNew = isNew;
                    });



                    that.answers = data.answers.map(function (answer) {
                        var answerObj = new Answer(answer);
                        if (answerObj.isNew) {
                            that.isNew = true;
                        }
                        return answerObj;
                    });
                    that.files = [];
                    that.quizes = [];
                    for (var i = 0; i < data.files.length; i++) {
                        if (data.files[i].type) {
                            that.files.push(new File(data.files[i]));
                        } else {
                            that.quizes.push(new File(data.files[i]));
                        }
                    }
                    //that.files = data.files.map(function (file) {
                    //    if (file.thumbnail) {
                    //        return new File(file);
                    //    }
                    //});
                    //that.question = data.files.map(function (file)
                    //{
                    //    if (!file.thumbnail) {
                    //        return new File(file);
                    //    }
                    //});
                    that.aFormData = {};
                }

                function Answer(data) {
                    var that = this;
                    data = data || {};
                    that.id = data.id;
                    that.userName = data.userName;
                    that.userImage = data.userImage;
                    that.userId = data.userId;
                    that.content = $sce.trustAsHtml(data.content ? data.content.replace(/\n/g, '<br/>') : '');
                    that.rating = data.rating;
                    that.iRate = data.iRate;
                    that.isAnswer = data.answer;
                    that.createTime = data.creationTime;
                    that.files = data.files.map(function (file) { return new File(file); });
                    sNewUpdates.isNew($scope.boxId, 'answers', that.id, function (isNew) {
                        that.isNew = isNew;
                    });
                }

                function File(data) {
                    var that = this;
                    data = data || {};
                    that.id = data.id;
                    that.name = data.name;
                    //if (data.type === 'File') {
                    that.thumbnail = 'https://az779114.vo.msecnd.net/preview/' + encodeURIComponent(data.source) + '.jpg?width=100&height=125&mode=crop';
                    //} else {
                    //    that.thumbnail = 'https://az779114.vo.msecnd.net/images/link_720.png';
                    //}

                    var userId = sUserDetails.getDetails().id;
                    that.isOwner = data.ownerId === userId;
                    that.isVisible = that.isOwner;

                    that.itemUrl = data.url || data.itemUrl;
                    that.download = that.itemUrl + 'download/';
                }

                var tagsToReplace = {
                    '&': '&amp;',
                    '<': '&lt;',
                    '>': '&gt;'
                };

                $scope.data = {
                    //$scope.boxId = we get this from parent scope no info
                    userName: sUserDetails.getDetails().name,
                    userImage: sUserDetails.getDetails().image
                };

                $scope.qFormData = {};


                sQnA.list({ boxId: $scope.boxId }).then(function (questions) {

                    $scope.data.questions = questions.map(function (question) {
                        return new Question(question);
                    });

                    $scope.options.loader = false;

                    //if (!questions) {
                    //  //  $scope.info.state = states.none;
                    //    return;
                    //}
                    //if (!questions.length) {
                    //    $scope.info.state = states.empty;
                    //    return;
                    //}

                    //$scope.info.state = states.questions;
                });

                $scope.answersLength = function (question) {
                    if (question.answers.length === 1) {
                        return question.answers.length + ' ' + resManager.get('Comment');
                    }

                    return question.answers.length + ' ' + resManager.get('Comments');
                };

                $scope.canDelete = function (obj) { //question || answer
                    var userId = sUserDetails.getDetails().id;
                    return obj.userId === userId || $scope.info.ownerId === userId || sUserDetails.getDetails().isAdmin;
                };


                $scope.postQuestion = function () {
                    $scope.qFormData.boxId = $scope.boxId;

                    var fileDisplay = $scope.qFormData.files;
                    if ($scope.qFormData.files && $scope.qFormData.files.length) {
                        $scope.qFormData.files = $scope.qFormData.files.map(function (file) {
                            return file.id;
                        });
                    }
                    $analytics.eventTrack('Feed - Add a question', {
                        category: 'Box'
                    });
                    sQnA.post.question($scope.qFormData).then(function (details) {


                        $scope.qFormData.content = $scope.qFormData.content || '';



                        var obj = {
                            id: details.commentId,
                            userName: details.userName,
                            userImage: details.userImage,
                            userId: details.userId,
                            userUrl: details.userUrl,
                            content: extractUrls($scope.qFormData.content.replace(/[&<>]/g, replaceTag)),
                            creationTime: new Date().toISOString(),
                            answers: [],
                            files: fileDisplay || []
                        }

                        $scope.data.questions.unshift(new Question(obj));
                        $scope.$broadcast('update-scroll');
                        $scope.qFormData = {};

                        $scope.followBox(true);

                        sGmfcnHandler.addPoints({ type: 'question' });

                        $scope.info.feedLength++;

                    }, function () {
                        sNotify.alert('Error');
                    });
                };

                $scope.displayComment = function (question) {
                    question.displayComment = !question.displayComment;
                };

                $scope.postAnswer = function (question) {

                    $analytics.eventTrack('Feed - Add an answer', {
                        category: 'Box Feed'
                    });
                    sGmfcnHandler.addPoints({ type: 'answer' });


                    question.aFormData.questionId = question.id;
                    question.aFormData.boxId = $scope.boxId;
                    var fileDisplay = question.aFormData.files;
                    if (question.aFormData.files && question.aFormData.files.length) {
                        question.aFormData.files = question.aFormData.files.map(function (file) {
                            return file.id;
                        });
                    }
                    question.aFormData.content = question.aFormData.content || '';
                    sQnA.post.answer(question.aFormData).then(function (response) {
                        var answerId = response;


                        var obj = {
                            id: answerId,
                            userName: sUserDetails.getDetails().name,
                            userImage: sUserDetails.getDetails().image,
                            userId: sUserDetails.getDetails().id,
                            userUrl: sUserDetails.getDetails().url,
                            content: extractUrls(question.aFormData.content.replace(/[&<>]/g, replaceTag)),
                            rating: 0,
                            creationTime: new Date().toISOString(),
                            iRate: false,
                            answer: false,
                            files: fileDisplay || []
                        };

                        question.answers.push(new Answer(obj));
                        $scope.$broadcast('update-scroll');
                        $scope.followBox(true);

                        //updatetime
                        //notify
                        question.aFormData = {};

                    }, function () {
                        sNotify.alert('Error');
                    });
                };

                $scope.deleteQuestion = function (question) {
                    sNotify.confirm(resManager.get('SureYouWantToDelete') + '?').then(function () {
                        var index = $scope.data.questions.indexOf(question);
                        $scope.data.questions.splice(index, 1);

                        sQnA.delete.question({ questionId: question.id });

                        $analytics.eventTrack('Feed - Remove Question', {
                            category: 'Box'
                        });

                        if ($scope.info.feedLength) {
                            $scope.info.feedLength--;
                        }

                    });
                };

                $scope.deleteAnswer = function (question, answer) {
                    sNotify.confirm(resManager.get('SureYouWantToDelete') + '?').then(function () {

                        var index = question.answers.indexOf(answer);
                        question.answers.splice(index, 1);
                        sQnA.delete.answer({ answerId: answer.id });

                        $analytics.eventTrack('Feed - Remove Answer', {
                            category: 'Box'
                        });
                    });
                };

                $scope.removeAttachmentQ = function (question, item) {
                    sNotify.confirm(resManager.get('SureYouWantToDelete') + ' ' + (item.name ? item.name : '') + '?').then(function () {
                        sItem.delete({ itemId: item.id, boxId: $scope.boxId }).then(function () {
                            var index = question.files.indexOf(item);
                            question.files.splice(index, 1);
                            if (question.files.length === 0 && question.content.length === 0) {
                                index = $scope.data.questions.indexOf(question);
                                $scope.data.questions.splice(index, 1);
                            }
                        }, function (response) {
                            sNotify.alert(response);
                        });
                    });


                    $analytics.eventTrack('Feed - Remove question attachment', {
                        category: 'Box'
                    });
                };

                $scope.removeAttachmentA = function (question, answer, item) {
                    sNotify.confirm(resManager.get('SureYouWantToDelete') + ' ' + (item.name ? item.name : '') + '?').then(function () {
                        sItem.delete({ itemId: item.id, boxId: $scope.boxId }).then(function () {
                            var index = answer.files.indexOf(item);
                            answer.files.splice(index, 1);
                            if (answer.files.length === 0 && answer.content.length === 0) {
                                index = question.answers.indexOf(answer);
                                question.answers.splice(index, 1);

                            }
                        }, function (response) {
                            sNotify.alert(response);
                        });

                    });

                    $analytics.eventTrack('Feed - Remove answer attachment', {
                        category: 'Box'
                    });
                };

                $scope.downloadItem = function (event/*, item*/) {
                    if (!sUserDetails.isAuthenticated()) {
                        event.preventDefault();
                        sLogin.registerAction();
                        return;
                    }

                    $scope.followBox();

                    $analytics.eventTrack('Feed - Download Item', {
                        category: 'Box'
                    });
                };



                $scope.$on('BeforeUpload', function (e, data) {
                    data = data || {};

                    if (data.newQuestion) {
                        sFocus('newQuestion');
                        return;
                    }

                    if (data.questionId) {
                        var question = _.find($scope.data.questions, function (q) {
                            return data.questionId === q.id;
                        });

                        if (!question) {
                            return;
                        }

                        sFocus('answer' + $scope.data.questions.indexOf(question));
                    }

                });
                $scope.$on('ItemUploaded', function (e, data) {
                    if (data.boxId !== $scope.boxId) {
                        return;
                    }

                    $scope.followBox(true);

                    data.itemDto.uid = data.itemDto.id;

                    if (data.newQuestion) {

                        if ($scope.qFormData.files && $scope.qFormData.files.length) {
                            $scope.qFormData.files.push(data.itemDto);
                            return;
                        }

                        $scope.qFormData.files = [data.itemDto];


                        $analytics.eventTrack('Feed - Added question attachment', {
                            category: 'Box'
                        });


                        return;
                    }

                    if (data.questionId) {
                        var question = _.find($scope.data.questions, function (q) {
                            return data.questionId === q.id;
                        });

                        if (!question) {
                            return;
                        }



                        if (question.aFormData.files && question.aFormData.files.length) {
                            question.aFormData.files.push(data.itemDto);
                            return;
                        }

                        question.aFormData.files = [data.itemDto];

                        $analytics.eventTrack('Feed - Added answer attachment', {
                            category: 'Box'
                        });
                    }
                });

                $scope.addQuestionAttachment = function () {

                    if (!sUserDetails.isAuthenticated()) {
                        sLogin.registerAction();
                        return;
                    }

                    var data = {
                        boxId: $scope.boxId,
                        newQuestion: true
                    };

                    openUpload(data);

                    $analytics.eventTrack('Feed - Question Upload', {
                        category: 'Box'
                    });
                };

                $scope.removeQuestionAttachment = function (file) {
                    var index = $scope.qFormData.files.indexOf(file);
                    if (index !== -1) {
                        $scope.qFormData.files.splice(index, 1);
                    }

                    $analytics.eventTrack('Feed - Remove question attachment', {
                        category: 'Box'
                    });
                }

                $scope.addAnswerAttachment = function (question) {
                    if (!sUserDetails.isAuthenticated()) {
                        sLogin.registerAction();
                        return;
                    }

                    var data = {
                        boxId: $scope.boxId,
                        tabId: null,
                        questionId: question.id
                    };

                    openUpload(data);
                    $analytics.eventTrack('Feed - Upload Answer', {
                        category: 'Box'
                    });

                };
                $scope.removeAnswerAttachment = function (question, file) {
                    var index = question.aFormData.files.indexOf(file);
                    if (index !== -1) {
                        question.aFormData.files.splice(index, 1);
                    }

                    $analytics.eventTrack('Feed - Remove answer attachment', {
                        category: 'Box'
                    });
                }

                $scope.checkAuth = function () {
                    if (!sUserDetails.isAuthenticated()) {
                        sLogin.registerAction();
                        return false;
                    }

                    return true;
                };

                function openUpload(data) {

                    sModal.open('upload', {
                        data: data,
                        callback: {
                            close: function () {
                                $scope.followBox(true);
                            }
                        }
                    });
                }

                function replaceTag(tag) {
                    return tagsToReplace[tag] || tag;
                }

                function extractUrls(d) {
                    if (!d) {
                        return;
                    }
                    var urlex = /\b((?:https?:\/\/|www\d{0,3}[.]|[a-z0-9.\-]+[.][a-z]{2,4}\/)(?:[^\s()<>]+|\(([^\s()<>]+|(\([^\s()<>]+\)))*\))+(?:\(([^\s()<>]+|(\([^\s()<>]+\)))*\)|[^\s`!()\[\]{};:'"".,<>?«»“”‘’]))/i;

                    var array = d.match(urlex) || [];
                    var matches = [];
                    for (var i = 0, j = array.length; i < j; i++) {
                        if (matches.indexOf(array[i]) < 0)
                            matches.push(array[i]);
                    }
                    if (!matches.length) {
                        return d;
                    }
                    for (var i = 0; i < matches.length; i++) {
                        var url = matches[i];
                        if (!url) {
                            continue;
                        }
                        if (url.indexOf('http') !== 0) {
                            url = 'http://' + url;
                        }
                        d = d.replace(matches[i], "<a target=\"_blank\" href=\"" + url + "\">" + matches[i] + "</a>");
                    }

                    return d;
                }

            }
]);
