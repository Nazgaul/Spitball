(function ($, ko, cd, dataContext, ZboxResources, analytics) {
    "use strict";
    if (window.scriptLoaded.isLoaded('qna')) {
        return;
    }

    cd.loadModel('box', 'BoxContext', function () {
        ko.applyBindings(new QnAViewModel(), document.getElementById('box_QA'));

    });

    function QnAViewModel() {


        function Question(data) {
            var _that = this;
            data = data || {};
            _that.id = data.id;
            _that.userName = data.userName;
            _that.userImg = data.userImage;
            _that.userid = data.userUid;
            _that.content = data.content.replace(/\n/g, '<br/>');
            _that.createTime = data.creationTime || new Date();
            data.Answers = data.answers || [];
            _that.answers = ko.observableArray($.map(data.answers, function (i) { return new Answer(i); }));
            _that.answerCount = ko.computed(function () {
                return _that.answers().length;
            });
            _that.answerLengthText = ko.computed(function () {
                if (_that.answers().length === 1) {
                    return _that.answers().length + ' ' + ZboxResources.Answer;
                }
                return _that.answers().length + ' ' + ZboxResources.Answers;
            });
            _that.bestAnswer = ko.computed(function () {
                var x = ko.utils.arrayFirst(_that.answers(), function (i) {
                    return i.isAnswer() === true;
                });
                if (x) {
                    return x;
                }
                var rate = 0, answer = null;
                ko.utils.arrayForEach(_that.answers(), function (i) {
                    if (i.rating() > rate) {
                        answer = i;
                        rate = i.rating();
                    }
                });
                if (answer) {
                    return answer;
                }
                if (_that.answers()) {
                    return _that.answers()[0];
                }
                return [];
            });

            _that.canMarkAsAnswer = _that.userid === cd.userDetail.nId;
            _that.canDelete = ko.computed(function () {
                return _that.userid === cd.userDetail().nId || self.isOwner();
            });
            data.files = data.files || [];
            _that.files = ko.observableArray($.map(data.files, function (i) { return new File(i); }));

            _that.sortQuestion = function (left, right) {
                /// <summary></summary>
                /// <param name="left" type="Answer"></param>
                /// <param name="right" type="Answer"></param>
                if (left.isAnswer()) {
                    return -1;
                }
                if (right.isAnswer()) {
                    return 1;
                }
                if (left.createTime > right.createTime) {
                    return 1;
                }
                return -1;
            };
            _that.deleteAnswer = function (a) {
                /// <summary></summary>
                /// <param name="a" type="Answer"></param>

                _that.answers.remove(a);
                dataContext.deleteAnswer({
                    data: { answerId: a.id },
                    success: function () {
                        cd.pubsub.publish('removeAnswerNoti', { boxid: boxid, questionId: _that.id, answerId: a.id });
                    }
                });
            };
            _that.isNew = ko.observable(false);

        }
        function Answer(data) {
            var _that = this;
            data = data || {};
            _that.id = data.id;

            _that.userName = data.userName;
            _that.userImg = data.userImage;
            _that.userId = data.userId;
            
            _that.content = data.content.replace(/\n/g, '<br/>');
            _that.rating = ko.observable(data.rating);
            _that.irate = data.iRate;
            _that.isAnswer = ko.observable(data.answer);
            _that.createTime = data.creationTime;
            data.files = data.files || [];
            _that.files = ko.observableArray($.map(data.files, function (i) { return new File(i); }));
            _that.canDelete = ko.computed(function () {
                return _that.userId === cd.userDetail().nId || self.isOwner();
            });
            _that.isNew = ko.observable(false);

        }
        function File(data) {
            var _that = this;
            data = data || {};
            _that.id = data.uid;
            _that.thumbnail = data.thumbnail;
            _that.download = '';

            _that.isOwner = ko.computed(function () {
                return data.ownerId === cd.userDetail().nId;
            });
            _that.isVisible = ko.computed(function () {
                return data.ownerId === cd.userDetail().nId || self.isOwner();
            });

            _that.download = "/d/" + boxid + "/" + _that.id;

            _that.itemUrl = data.url;
        }



        var self = this, boxid = '';
        self.permission = ko.observable('none');
        self.isOwner = ko.computed(function() {
            return self.permission().toLowerCase() === 'owner';
        });

        var state = {
            none: 0,
            emptyState: 1,
            question: 2,
            answers: 3
        };
        cd.pubsub.subscribe('perm', function (d) {
            self.permission(d);
        });

        self.state = ko.observable(state.none);

        self.userImg = cd.userDetail().img;
        self.userName = cd.userDetail().name;

        self.questionList = ko.observableArray([]);
        self.selectedQuestion = ko.observable();


        self.showAllAnswers = function (q, e) {
            self.selectedQuestion(q);
            analytics.trackEvent('Answer', 'Click on answer', 'The number of clicks on show answer');
            $(e.target).parents('.QItem').addClass('selected');

            q.isNew(false);
            cd.newUpdates.deleteUpdate('questions', boxid, q.id);

            $('#Answers').show();

            $('.QA').addClass('changeState');
            $('#qa_Back').show();
            var $QForm = $('.QForm');
            $QForm.css('margin-bottom', -$QForm.outerHeight(true) - 20 + 'px');
            self.state(state.answers);
            if (e.target.getAttribute('data-action')) {
                $('.enterAnswer').find('textarea').focus();
            }
            cd.pubsub.publish('clearTooltip');
        };
        self.showAllQuestion = function () {
            $('.enterAnswer').removeClass('QAfloatButton');
            $('#Answers').css('height', '');
            $('.QA').removeClass('changeState').removeClass('flipTransition');
            $('.QForm').css('margin-bottom', '-20px');

            if (self.questionList().length) {
                self.state(state.question);
                return;
            }

            self.state(state.emptyState);

        };


        cd.pubsub.subscribe('box', function (data) {
            boxid = data.id;
            getQuestions();
            cd.pubsub.publish('getUpdates');
        });

        cd.pubsub.subscribe('updates', function (updates) {
            var userId = cd.userDetail().nId,
                box = updates[userId][boxid],
                questions, question;

            if (!box) {
                return;
            }

            questions = updates[userId][boxid].questions;

            if (!questions) {
                return;
            }

            for (var i = 0, l = self.questionList().length; i < l; i++) {
                question = self.questionList()[i];
                question.isNew(questions.indexOf(question.id) > -1);
            }

        });

        cd.pubsub.subscribe('boxclear', function () {
            self.state(state.none);
            self.questionList([]);
            self.showAllQuestion();
            self.permission('none');

            var forms = $('#box_QA').find('form');
            for (var i = 0; i < forms.length; i++) {
                forms[0].reset();
                forms.find('textarea').css('height', '');
                forms.find('button').hide();
            }
        });

        cd.pubsub.subscribe('addQuestion', function (newquestionobj) {
            if (boxid === newquestionobj.boxid) {
                var newquestion = new Question(newquestionobj.question);
                self.questionList.unshift(newquestion);
            }
        });
        cd.pubsub.subscribe('addAnswer', function (newAnswerObj) {
            var questionId = newAnswerObj.questionid;
            var question = ko.utils.arrayFirst(self.questionList(), function (q) {
                return q.id === questionId;
            });
            if (question) {
                var newAnswer = new Answer(newAnswerObj.answer);
                question.answers.push(newAnswer);
            }
            // self.postAnswer
        });

        cd.pubsub.subscribe('removeAnswer', function (d) {
            var questionId = d.questionid;
            var question = ko.utils.arrayFirst(self.questionList(), function (q) {
                return q.id === questionId;
            });
            question.answers.remove(function (a) {
                /// <summary></summary>
                /// <param name="a" type="Answer"></param>
                return a.id === d.answerid;

            });
        });

        cd.pubsub.subscribe('removeQuestion', function (deletedQuestionid) {
            self.questionList.remove(function (q) {
                /// <summary></summary>
                /// <param name="q" type="Question"></param>

                return q.id === deletedQuestionid;
            });
        });
        self.postQuestion = function (f) {
            if (!cd.register()) {
                cd.unregisterAction();
                return;
            }
            if (self.permission() === 'none' || self.permission() === 'invite') {
                cd.notification(ZboxResources.NeedToFollowBox);
                return;
            }
            var $f = $(f),
            data = $f.serializeArray(), textArea = $f.find('[name="Content"]');
            data.push({ name: 'BoxUid', value: boxid });
            $f.find('button').attr('disabled', 'disabled');
            analytics.trackEvent('Question', 'Add a question', 'The number of question added by users');
            cd.pubsub.publish('addPoints', { type: 'question' });
            dataContext.addQuestion({
                data: data,
                success: function (d) {
                    var obj = {
                        id: d,
                        userName: cd.userDetail().name,
                        userImage: cd.userDetail().img,
                        userUid: cd.userDetail().nId,
                        userUrl: cd.userDetail().url,
                        content: extractUrls($.trim(textArea.val())),
                        answers: [],
                        files: getFiles($f)
                    };
                    var newquestion = new Question(obj);
                    cd.pubsub.publish('addQuestionNoti', { boxid: boxid, question: obj });
                    $f.find('button').removeAttr('disabled').hide();
                    $f.find('textarea').css('height', '');
                    self.questionList.unshift(newquestion);
                    textArea.val('');
                    $f.find('.attachedList').empty();
                    self.state(state.question);
                }
            });
        };
        function getFiles(f) {
            var files = [];
            $(f).find('.attachedItem').each(function (i, e) {
                var input = e.querySelector('input');
                files.push({
                    uid: input.value,
                    thumbnail: input.getAttribute('data-thumb'),
                    type: input.getAttribute('data-type'),
                    name: e.querySelector('.attachName').innerHTML,
                    ownerId: cd.userDetail().nId,
                    url: input.getAttribute('data-url')
                });
            });
            return files;
        }
        function extractUrls(d) {
            /// <summary></summary>
            /// <param name="d" type="String"></param>

            var urlex = /\b((?:https?:\/\/|www\d{0,3}[.]|[a-z0-9.\-]+[.][a-z]{2,4}\/)(?:[^\s()<>]+|\(([^\s()<>]+|(\([^\s()<>]+\)))*\))+(?:\(([^\s()<>]+|(\([^\s()<>]+\)))*\)|[^\s`!()\[\]{};:'"".,<>?«»“”‘’]))/i;
            var matches = ko.utils.arrayGetDistinctValues(d.match(urlex));
            for (var i = 0; i < matches.length; i++) {
                var url = matches[i];
                if (!url) {
                    continue;
                }
                if (url.indexOf('http') !== 0) {
                    url = 'http://' + url;
                }
                d = d.replace(matches[i], "<a target=\"_Blank\" href=\"" + url + "\">" + matches[i] + "</a>");
            }

            return d;
        }
        self.postAnswer = function (f) {
            if (!cd.register()) {
                cd.unregisterAction();
                return;
            }
            if (self.permission() === 'none' || self.permission() === 'invite') {
                cd.notification(ZboxResources.NeedToFollowBox);
                return;
            }
            analytics.trackEvent('Answer', 'Give answer', 'Providing answer ');
            var $f = $(f),
            data = $(f).serializeArray(), textArea = $f.find('[name="Content"]');
            data.push({ name: 'BoxUid', value: boxid });
            var question = ko.dataFor(f);
            data.push({ name: 'QuestionId', value: question.id });
            $f.find('button').attr('disabled', 'disabled');
            $f.find('textarea').css('height', '');
            cd.pubsub.publish('addPoints', { type: 'answer' });
            dataContext.addAnswer({
                data: data,
                success: function (d) {
                    var answerObj = {
                        id: d,
                        userId: cd.userDetail().nId,
                        userName: cd.userDetail().name,
                        userImage: cd.userDetail().img,
                        userUrl : cd.userDetail().url,
                        content: extractUrls($.trim(textArea.val())),
                        rating: 0,
                        creationTime: new Date(),
                        iRate: false,
                        answer: false,
                        files: getFiles($f)
                    };
                    var newanswer = new Answer(answerObj);
                    $f.find('button').removeAttr('disabled').hide();
                    question.answers.push(newanswer);
                    cd.pubsub.publish('addAnswerNoti', { boxid: boxid, questionId: question.id, answer: answerObj });
                    textArea.val('');
                    $f.find('.attachedList').empty();
                }
            });

        };
        //self.markAnswer = function (f, e) {
        //    /// <summary></summary>
        //    /// <param name="f" type="Object"></param>
        //    /// <param name="e" type="Event"></param>

        //    if (!cd.register()) {
        //        cd.unregisterAction();
        //    }
        //    var bestElement = document.querySelector('.best');
        //    if (bestElement) {
        //        var oldAnswer = ko.dataFor(bestElement);
        //        oldAnswer.isAnswer(false);
        //    }

        //    f.isAnswer(true);
        //    var question = ko.contextFor(e.target).$parent;
        //    question.answers.sort(question.sortQuestion);
        //    dataContext.markAnswer({
        //        data: { answerId: f.id }
        //    });
        //}

        //self.rateAnswer = function (f, e) {
        //    if (!cd.register()) {
        //        cd.unregisterAction();
        //    }
        //    var cDisabled = 'disabled';
        //    e.target.setAttribute(cDisabled, cDisabled);
        //    if (!f.irate) {
        //        f.rating(f.rating() + 1);
        //    }
        //    else {
        //        f.rating(f.rating() - 1);
        //    }
        //    f.irate = !f.irate;


        //    dataContext.rateQuestion({
        //        data: { answerId: f.id },
        //        success: function () {
        //            e.target.removeAttribute(cDisabled);

        //        }
        //    });

        //}


        self.deleteQuestion = function (q) {
            /// <summary></summary>
            /// <param name="q" type="Question"></param>

            self.questionList.remove(q);
            dataContext.deleteQuestion({
                data: { questionId: q.id },
                success: function () {
                    cd.pubsub.publish('removeQuestionNoti', { boxid: boxid, question: q.id });
                }
            });

            self.showAllQuestion();


        };

        self.checkOverFlow = function (element) {
            if (element.nodeType !== Node.ELEMENT_NODE) {
                return;
            }
            // var answerelem = $(element).find('.AContent');
            //if (x.find('.annotationText').height() < x.find('.annotationTextWpr').height()) {
            //    x.find('.show-more').remove();
            //}
            //if (answerelem.height() > 55) {
            //    answer.addClass('AOverflow');
            //}
            //    x.find('.show-more').remove();
            //}
        };
        function getQuestions() {
            dataContext.getQuestions({
                data: {
                    boxId: boxid,
                    uniName: cd.getParameterFromUrl(1),
                    boxName: cd.getParameterFromUrl(3)
                },
                success: function (data) {
                    /// <summary></summary>
                    /// <param name="data" type="Array"></param>
                    if (!data.length) {
                        self.state(state.emptyState);
                        return;
                    }
                    self.questionList($.map(data, function (i) { return new Question(i); }));
                    //cd.updateTimeActions();
                    self.state(state.question);
                }
            });
        }

        registerEvents();

        var $qaContent = $('.QAContent'), heightFromTop = $('#box_QA').position().top;
        function applyScroll() {
            heightFromTop = heightFromTop || $('#box_QA').position().top;
            cd.innerScroll($('#Questions'), $(window).height() - ($('#Questions').offset().top - $(window).scrollTop()));

            var extraHeight = 0;
            if ($('.enterAnswer').hasClass('QAfloatButton')) {
                extraHeight += $('.enterAnswer').outerHeight(true);
            }


            cd.innerScroll($('#Answers'),
                $(window).height() - ($('#Answers').offset().top - $(window).scrollTop()) - extraHeight);

            //  var extraHeight = $('.QATop').outerHeight(true);
            //  if (self.state() === state.answers) {
            //      extraHeight = $('#qa_Back').height();
            //  }
            //  if (self.state() === state.emptyState) {
            //      extraHeight = 0;
            //  }
            ////  $('.QAContent').offset()
            //  cd.innerScroll($qaContent, $(window).height() - heightFromTop - extraHeight);
        }
        //function scroll(location, elem) {
        //    location = location || 0;
        //    $(elem).slimScroll({ scrollTo: location });
        //}
        function registerEvents() {
            //$('img[data-type=user]').attr('src', cd.userDetail().img);

            //$qaContent = $('.QAContent');
            cd.pubsub.subscribe('windowChanged', function () {
                if (document.getElementById('box').style.display === 'block') {
                    applyScroll();
                }
            });
            cd.pubsub.subscribe('box_show', function () {
                applyScroll();
            });
            //var pos = 0;
            //$('#Questions').slimScroll().bind('slimscrolling', function (e, position) {
            //    pos = position;
            //});

            $('#box_QA').on('click','.attachDownload',function (e) {
                if (!cd.register()) {
                    e.preventDefault();
                    cd.pubsub.publish('register');
                    return;
                }
            });


            var animationEvents = ['webkitTransitionEnd', 'transitionend', 'MSTransitionEnd'];

            $('.QForm').bind(animationEvents.join(' '), function (e) {
                if (self.state() === state.question) {
                    $('.QA').removeClass('flipTransition');
                    // scroll(pos, $('#Questions'));
                }
                if (self.state() === state.answers) {
                    $('.QA').addClass('flipTransition');
                    //applyScroll();
                }

            });

            $('.AWpr').bind(animationEvents.join(' '), function (e) {

                //if we do not hide this the scroll is for the height of the bigger then Question and answers
                if (self.state() === state.question) {
                    window.setTimeout(function () {
                        $('.QItem').removeClass('selected');
                    }, 1000);
                    if (!$('.QA').hasClass('flipTransition')) {
                        // scroll(pos);
                    }
                }
                else {
                    //  scroll(0,$('#Answers'));
                    var $enterAnswer = $('.enterAnswer'), $answers = $('#Answers');
                    if ($(window).height() < $enterAnswer.offset().top) {
                        $enterAnswer.addClass('QAfloatButton');
                        //$('.AWpr').css('paddingBottom',$enterAnswer.outerHeight(true) + 'px');//.height($answers.height() + $enterAnswer.outerHeight(true));
                    }

                }
                applyScroll();

            });

            var elem;
            $(document).on('click', '[data-upload]', function () {
                elem = this;
                cd.pubsub.publish('upload', { question: true });
            }).on('click', '.removeAttach', function () {
                $(this).parent().remove();
            }).on('click', '.attachDelete', function () {
                var file = ko.dataFor(this);

                var context = ko.contextFor(this);
                context.$parent.files.remove(file);
                dataContext.removefileQnA({
                    data: { itemId: file.id }
                });

            });

            $('#box_QA').on('paste', '[name="Content"]', function () {
                var $this = $(this);
                //need to have time to paste to popluate the val
                window.setTimeout(function () {
                    checkInputValue($this);
                }, 10);
            });
            $('#box_QA').on('keyup keydown', '[name="Content"]', function () {
                var $this = $(this);
                //need to have time to paste to popluate the val
                checkInputValue($this);
            })
            .on('focus', '[name="Content"]', function () {
                if (!cd.register()) {
                    cd.unregisterAction();
                    return;
                }

                if (self.permission() === 'none' || self.permission() === 'invite') {
                    $(this).blur();
                    cd.notification(ZboxResources.NeedToFollowBox);
                    return;
                }
                if (this.value === this.getAttribute('placeholder')) {
                    this.value = '';
                }

                if (this.value === '') {
                    $(this).parents('form').find('button').show().attr('disabled', 'disabled');
                }
                // $(this).elastic();
            });

            cd.pubsub.subscribe('qnaAttacment', function (d) {

                var ul = $(elem).parents('form').find('.attachedList');
                ul.append(cd.attachTemplateToData('attachTemplate', d));
                //BoxUid: "lzodJqaBLAE"
                //Name: "http://wingkaiwan.com/2012/12/28/replacing-mvc-javascriptserializer-with-json-net-jsonserializer/"
                //Owner: "ramyv"
                //OwnerId: "TsKl7pMWvWx"
                //TabId: ""
                //Thumbnail: "http://127.0.0.1:10000/devstoreaccount1/zboxthumbnail/linkv1.jpg"
                //Type: "Link"
                //Uid: "pD0nrbAtsFh"
            });
            function checkInputValue($this) {
                if ($this.val()) {
                    $this.parents('form').find('button').removeAttr('disabled');
                }
                else {
                    $this.parents('form').find('button').attr('disabled', 'disabled');
                }
            }
        }        
    }



})(jQuery, ko, cd, cd.data, ZboxResources, cd.analytics);