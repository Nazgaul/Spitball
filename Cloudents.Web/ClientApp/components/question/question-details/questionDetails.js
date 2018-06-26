import questionThread from "./questionThread.vue";
import extendedTextArea from "../helpers/extended-text-area/extendedTextArea.vue";
import questionService from "../../../services/questionService";
import {mapGetters, mapMutations, mapActions} from 'vuex'
import questionCard from "./../helpers/question-card/question-card.vue";
import disableForm from "../../mixins/submitDisableMixin.js"

export default {
    mixins: [disableForm],
    components: {questionThread, questionCard, extendedTextArea},
    props: {
        id: {Number} // got it from route
    },
    data() {
        return {
            textAreaValue: "",
            answerFiles: [],
            questionData: null,
            showForm: false,
        };
    },
    beforeRouteLeave(to, from, next) {
        this.resetQuestion();
        next()
    },
    methods: {
        ...mapActions(["resetQuestion", "removeDeletedAnswer", "updateToasterParams"]),
        ...mapMutations({updateLoading: "UPDATE_LOADING"}),
        submitAnswer() {
            this.updateLoading(true);
            var self = this;
            if (this.submitForm()) {
                this.removeDeletedAnswer();
                questionService.answerQuestion(this.id, this.textAreaValue, this.answerFiles)
                    .then(function () {
                        //TODO: do this on client side (render data inserted by user without calling server) - see commented out below - all that's left is asking ram to return the answerId in response
                        // var creationTime = new Date();
                        // self.questionData.answers.push({
                        //     create: creationTime.toISOString(),
                        //     files: self.answerFiles.map(fileName => "https://spitballdev.blob.core.windows.net/spitball-files/question/"+self.id+"/answer/"+response.data.answerId+"/"+fileName), //this will work only if answerid returns from server
                        //     id: response.data.answerId,
                        //     text: self.textAreaValue,
                        //     user: self.accountUser
                        // });
                        self.textAreaValue = "";
                        self.answerFiles = [];
                        self.updateLoading(false);
                        self.getData();//TODO: remove this line when doing the client side data rendering (make sure to handle delete as well)
                    }, () => {
                        self.submitForm(false);
                        self.updateLoading(true);
                    });
            }
        },
        addFile(filename) {
            this.answerFiles.push(...filename.split(','));
        },
        removeFile(index) {
            this.answerFiles.splice(index, 1);
        },
        getData() {
            var self = this;
            questionService.getQuestion(this.id).then(function (response) {
                self.questionData = response.data;
                if (self.accountUser) {
                    self.questionData.cardOwner = self.accountUser.id === response.data.user.id;
                } else {
                    self.questionData.cardOwner = false; // if accountUser is null the chat shouldn't appear
                }
                self.buildChat();
            }, (error) => {
                if (error.response.status === 404) {
                    window.location = "/error/notfound"
                    return;
                }
                console.error(error);
            });
        },
        buildChat() {
            if (this.talkSession && this.questionData) {
                const otherUser = this.questionData.user;
                var other1 = new Talk.User(otherUser.id);

                //_${this.accountUser.id}_${otherUser.id}
                var conversation = this.talkSession.getOrCreateConversation(
                    `question_${this.id}`
                );
                conversation.setParticipant(this.chatAccount);
                conversation.setParticipant(other1);
                //this.talkSession.syncThemeForLocalDev("/Content/talkjs-theme.css");
                var chatbox = this.talkSession.createChatbox(conversation);
                this.$nextTick(() => {
                    chatbox.mount(this.$refs["chat-area"]);
                });
            }
        },
        showAnswerField() {
            if (this.accountUser) {
                this.showForm = true
            }
            else {
                this.updateToasterParams({
                    toasterText: 'Please <a href="/signin">Login</a> to answer',
                    showToaster: true
                });
            }
        }
    },
    watch: {
        talkSession: function (newVal, oldVal) {
            if (newVal) {
                this.buildChat();
            }
        }
    },
    computed: {
        ...mapGetters(["talkSession", "accountUser", "chatAccount", "getCorrectAnswer", "isDeletedAnswer"]),
        userNotAnswered() {
            this.isDeletedAnswer ? this.submitForm(false) : "";
            return !this.questionData.answers.length || (!this.questionData.answers.filter(i => i.user.id === this.accountUser.id).length || this.isDeletedAnswer);
        },
        enableAnswer() {
            let val = !this.questionData.cardOwner && (!this.accountUser || this.userNotAnswered);
            this.showForm = (val && !this.questionData.answers.length);
            return val;
        }
        // isMobile() {
        //     return this.$vuetify.breakpoint.smAndDown;
        // },
    },
    created() {
        this.getData();
    }
}