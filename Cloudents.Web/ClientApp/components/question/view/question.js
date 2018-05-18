import questionTextArea from "./../helpers/question-text-area/questionTextArea.vue";
import questionCard from "./../helpers/question-card/question-card.vue";
import questionService from '../../../services/questionService';
import miniChat from "./../../chat/private-chat/private-chat.vue";

export default {
    components: {questionTextArea, questionCard, miniChat},
    data() {
        return {
            questionText: '',
            files: []
        }
    },
    methods:{
        answer(){
            var self = this;
            questionService.answerQuestion(this.questionId,this.questionText,this.files)
                .then(function () {
                    self.$router.push({path: '/note', query: {q: ''}});
                });
        }
    },
    computed:{
        
        isMobile(){return this.$vuetify.breakpoint.xsOnly;},    
        validForm() {
            return this.questionText.length
        }
    },
    created() {
        this.questionId = this.$route.query.id;
    }
}