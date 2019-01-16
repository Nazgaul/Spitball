import {mapActions, mapGetters} from 'vuex'
import UserAvatar from '../../helpers/UserAvatar/UserAvatar.vue'
import questionService from '../../../services/questionService'
export default {
    data(){
        return {
            subjectList: [],
        }
    },
    components:{
        UserAvatar
    },
    computed:{
        ...mapGetters(['getSelectedClasses', 'newQuestionDialogSate'])
    },
    watch:{
        // if question dialog state is false reset question form data to default
        newQuestionDialogSate: {
            immediate: true,
            handler(val) {
                if (val) {
                    // get subject if questionDialog state is true(happens only if accountUser is true)
                    questionService.getSubjects().then((response) => {
                        this.subjectList = response.data
                    });
                }
            },
        },
    },
    methods:{
        ...mapActions(['updateNewQuestionDialogState']),
        requestNewQuestionDialogClose() {
            this.updateNewQuestionDialogState(false)
        },
    }
}