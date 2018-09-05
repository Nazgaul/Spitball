import questionThread from "./questionThread.vue";
import extendedTextArea from "../helpers/extended-text-area/extendedTextArea.vue";
import questionService from "../../../services/questionService";
import { mapGetters, mapMutations, mapActions } from 'vuex'
import questionCard from "./../helpers/question-card/question-card.vue";
import disableForm from "../../mixins/submitDisableMixin.js"
import QuestionSuggestPopUp from "../../questionsSuggestPopUp/questionSuggestPopUp.vue";
import sbDialog from '../../wrappers/sb-dialog/sb-dialog.vue'
import loginToAnswer from '../../question/helpers/loginToAnswer/login-answer.vue'
import { sendEventList } from '../../../services/signalR/signalREventSender'
import { LanguageService } from "../../../services/language/languageService";

export default {
    mixins: [disableForm],
    components: {questionThread, questionCard, extendedTextArea, QuestionSuggestPopUp, sbDialog, loginToAnswer},
    props: {
        id: {Number}, // got it from route
        questionId: {Number}
    },
    data() {
        return {
            textAreaValue: "",
            errorTextArea: {},
            answerFiles: [],
            questionData: null,
            cardList: [],
            showForm: false,
            showDialogSuggestQuestion: false,
            showDialogLogin: false,
            build: null,
        };
    },
    beforeRouteLeave(to, from, next) {
        this.resetQuestion();
        next()
    },
    methods: {
        ...mapActions(["resetQuestion", "removeDeletedAnswer", "updateToasterParams", "updateLoginDialogState", 'updateUserProfileData']),
        ...mapMutations({updateLoading: "UPDATE_LOADING"}),
        submitAnswer() {
            if (!this.textAreaValue || this.textAreaValue.trim().length < 15) {
                this.errorTextArea = {
                    errorText: LanguageService.getValueByKey("question_questionDetails_error_minChar"),
                    errorClass: true
                };
                return
            }
            this.updateLoading(true);
            var self = this;
            if (self.submitForm()) {
                this.removeDeletedAnswer();
                self.textAreaValue = self.textAreaValue.trim();
                questionService.answerQuestion(self.id, self.textAreaValue, self.answerFiles)
                    .then(function (resp) {
                        self.$ga.event("Submit_answer", "Homwork help");
                        self.textAreaValue = "";
                        self.answerFiles = [];
                        self.updateLoading(false);
                        self.cardList = resp.data;
                        self.getData(true);//TODO: remove this line when doing the client side data rendering (make sure to handle delete as well)
                        self.showDialogSuggestQuestion = true; // question suggest popup dialog
                    }, () => {
                        self.submitForm(false);
                        self.updateLoading(true);
                    })
            }
        },

        addFile(filename) {
            this.answerFiles.push(...filename.split(','));
        },
        removeFile(index) {
            this.answerFiles.splice(index, 1);
        },
        getData(skipViewerUpdate) {
            let updateViewer = skipViewerUpdate ? false : true;
            //enable submit btn
            this.$data.submitted = false;
            questionService.getQuestion(this.id)
                .then((response) => {
                    this.questionData = response;

                    if (updateViewer) {
                        sendEventList.question.addViewr(this.questionData);
                    }

                    if (this.accountUser) {
                        this.questionData.cardOwner = this.accountUser.id === response.user.id;
                    } else {
                        this.questionData.cardOwner = false; // if accountUser is null the chat shouldn't appear
                    }
                    this.buildChat();
                }, (error) => {
                    if (error.response.status === 404) {
                        window.location = "/error/notfound";
                        return;
                    }
                    console.error(error);
                });
        },

        buildChat() {
            if (this.talkSession && this.questionData) {
                const otherUser = this.questionData.user;
                var other1 = new Talk.User(otherUser.id);
                var conversation = this.talkSession.getOrCreateConversation(
                    `question_${this.id}`
                );
                //conversation
                conversation.setParticipant(this.chatAccount, {notify: false});
                conversation.setParticipant(other1);
                conversation.setAttributes({
                    photoUrl: `${location.origin}/images/conversation.png`,
                    subject: `<${location.href}|${this.questionData.text}>`
                })
                    var chatbox = this.talkSession.createChatbox(conversation, {
                    showChatHeader: false
                    });
                chatbox.on("sendMessage", (t) => {
                    conversation.setParticipant(this.chatAccount, {notify: true})
                });
                this.$nextTick(() => {
                    chatbox.mount(this.$refs["chat-area"]);
                });
            }
        },
        showAnswerField() {
            if (this.accountUser) {
                this.showForm = true;
            }
            else {
                this.updateUserProfileData('profileMakeMoney');
                this.dialogType = ''
                this.updateLoginDialogState(true);
            }
        },

    },
    watch: {
        talkSession: function (newVal, oldVal) {
            if (newVal) {
                this.buildChat();
            }
        },
        //watch route(url query) update, and het question data from server
        '$route': 'getData'
    },
    computed: {
        ...mapGetters(["talkSession", "accountUser", "chatAccount", "getCorrectAnswer", "isDeletedAnswer", "loginDialogState"]),

        userNotAnswered() {
            this.isDeletedAnswer ? this.submitForm(false) : "";
            return !this.questionData.answers.length || (!this.questionData.answers.filter(i => i.user.id === this.accountUser.id).length || this.isDeletedAnswer);
        },
        enableAnswer() {
            let hasCorrectAnswer = !!this.questionData.correctAnswerId;
            let val = !this.questionData.cardOwner && (!this.accountUser || this.userNotAnswered) && !hasCorrectAnswer;
            this.showForm = (val && !this.questionData.answers.length);
            return val;
        },
        removeViewer() {
            console.log("leaving question");
            sendEventList.question.removeViewer(this.questionData);
        },
    },
    created() {
        global.addEventListener('beforeunload', () => {
            this.removeViewer();
        })

        this.getData();
        // to do may be to consider change to State Store VueX
        this.$root.$on('deleteAnswer', (id) => {
            this.questionData.answers = this.questionData.answers.filter(item => item.id !== id)
        });
        this.$root.$on('closePopUp', (name) => {
            if (name === 'suggestions') {
                this.showDialogSuggestQuestion = false;
            } else {
                this.updateLoginDialogState(false);
            }
        })
    },
    destroyed() {
        if (this.removeViewer) {
            this.removeViewer();
        }
    }
}