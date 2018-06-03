import questionThread from "./questionThread.vue";
import extendedTextArea from "../helpers/extended-text-area/extendedTextArea.vue";
import questionService from "../../../services/questionService";
import {mapGetters} from "vuex";
import questionCard from "./../helpers/question-card/question-card.vue";
import disableForm from "../../mixins/submitDisableMixin.js"

export default {
    mixins:[disableForm],
    components: {questionThread, questionCard, extendedTextArea},
    props: {
        id: {Number} // got it from route
    },
    data() {
        return {
            textAreaValue: "",
            answerFiles: [],
            questionData: null,
            showForm: false
        };
    },
    methods: {
        submitAnswer() {
            var self = this;
            if(!this.submitted) {
                this.submitForm();
                questionService.answerQuestion(this.id, this.textAreaValue, this.answerFiles)
                    .then(function () {
                        self.textAreaValue = "";
                        self.answerFiles = [];
                        //TODO: do this on client side (render data inserted by user without calling server)
                        self.getData();
                    });
            }
        },
        addFile(filename) {
            this.answerFiles.push(filename);
        },
        removeFile(index){
            this.answerFiles.splice(index,1);
        },
        getData() {
            var self = this;
            questionService.getQuestion(this.id).then(function (response) {
                self.questionData = response.data;
                if(self.accountUser) {
                    self.questionData.myQuestion = self.accountUser.id === response.data.user.id;
                }
                // self.questionData.correctAnswer = '1A5B19B2-573D-44EC-9486-A8E900D811F5';//TODO: remove when ram adds it to the api
                self.buildChat();
            });
        },
        buildChat() {
            if (this.talkSession && this.questionData) {
                const otherUser = this.questionData.user;
                if (this.accountUser.id === otherUser.id) {
                    return;
                }
                var other1 = new Talk.User(otherUser.id);

                var conversation = this.talkSession.getOrCreateConversation(
                    `question_${this.id}_${this.accountUser.id}_${otherUser.id}`
                );
                conversation.setParticipant(this.chatAccount);
                conversation.setParticipant(other1);
                var chatbox = this.talkSession.createChatbox(conversation);
                this.$nextTick(() => {
                    chatbox.mount(this.$refs["chat-area"]);
                });
            }
        },
        markAsCorrect(answerId){
            var self = this;
            questionService.markAsCorrectAnswer(answerId).then(function () {
                self.questionData.correctAnswer = answerId;
            })
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
        ...mapGetters(["talkSession", "accountUser", "chatAccount"]),
        // isMobile() {
        //     return this.$vuetify.breakpoint.smAndDown;
        // },
    },
    created() {
        this.getData();
    }
}