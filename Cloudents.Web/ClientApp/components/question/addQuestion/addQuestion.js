import {mapActions} from 'vuex'
import UserAvatar from '../../helpers/UserAvatar/UserAvatar.vue'
export default {
    data(){
        return {

        }
    },
    components:{
        UserAvatar
    },
    methods:{
        ...mapActions(['updateNewQuestionDialogState']),
        requestNewQuestionDialogClose() {
            this.updateNewQuestionDialogState(false)
        },
    }
}