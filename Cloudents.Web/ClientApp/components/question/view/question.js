import questionTextArea from "./../helpers/question-text-area/questionTextArea.vue";
import questionCard from "./../helpers/question-card/question-card.vue";
import questionService from '../../../services/questionService';
import miniChat from "./../../chat/private-chat/private-chat.vue";
import { mapGetters } from "vuex";

export default {
    components: { questionTextArea, questionCard, miniChat },
    props: { id: { Number } },
    data() {
        return {
            textAreaValue: '',
            files: [],
            questionData: null
        }
    },
    methods: {
        answer() {
            var self = this;
            questionService.answerQuestion(this.id, this.textAreaValue, this.files)
                .then(function () {
                    self.$router.push({ path: '/note', query: { q: '' } });
                });
        },
        getData() {
            var self = this;
            questionService.getQuestion(this.id)
                .then(function (response) {
                    self.questionData = response.data
                    self.buildChat();
                });
        },
        buildChat() {
            if (this.talkSession && this.questionData) {
                const otherUser = this.questionData.user;
                if (this.accountUser.id === otherUser.id ) {
                    return;
                }
                var other1 = new Talk.User({
                    id: otherUser.id,
                    name: otherUser.name,
                });
                
                var conversation = this.talkSession.getOrCreateConversation(`question_${this.id}_${this.accountUser.id}_${otherUser.id}`);
                conversation.setParticipant(this.chatAccount);
                conversation.setParticipant(other1);

                var chatbox = this.talkSession.createChatbox(conversation);
                this.$nextTick(()=>{
                chatbox.mount(this.$refs["chat-area"]);
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
        ...mapGetters(["talkSession","accountUser","chatAccount"]),
        isMobile() {
            return this.$vuetify.breakpoint.xsOnly;
        },

        validForm() {
            return this.textAreaValue.length
        }
    },
    created() {
        this.getData();
        // .then(function (response) {
        //     self.questionData = response.data
        // });
    }
}