
import questionThread from "./questionThread.vue";
import questionTextArea from "./../helpers/question-text-area/questionTextArea.vue";
import questionService from "../../../services/questionService";
import { mapGetters } from "vuex";
import questionCard from "./../helpers/question-card/question-card.vue";

export default {
    components: { questionThread, questionCard, questionTextArea },
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
            questionService.answerQuestion(this.id, this.textAreaValue, this.answerFiles)
                .then(function() {
                    self.textAreaValue = "";
                    //TODO: do this on client side (render data inserted by user without calling server)
                    self.getData();
                });
        },
        addFile(filename){
          this.answerFiles.push(filename);
        },
        getData() {
            var self = this;
            questionService.getQuestion(this.id).then(function(response) {
                self.questionData = response.data;
                self.buildChat();
            });
        },
        buildChat() {
            if (this.talkSession && this.questionData) {
                const otherUser = this.questionData.user;
                if (this.accountUser.id === otherUser.id) {
                    return;
                }
                var other1 = new Talk.User({
                    id: otherUser.id,
                    name: otherUser.name
                });

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
        }
    },
    watch: {
        talkSession: function(newVal, oldVal) {
            if (newVal) {
                this.buildChat();
            }
        }
    },
    computed: {
        ...mapGetters(["talkSession", "accountUser", "chatAccount"]),
        isMobile() {
            return this.$vuetify.breakpoint.xsOnly;
        },
    },
    created() {
        this.getData();
    }
}