import userAvatar from "../../../helpers/UserAvatar/UserAvatar.vue";
import askQuestionBtn from '../askQuestionBtn/askQuestionBtn.vue';
import {mapGetters, mapActions } from 'vuex';

export default {
    components:{
        askQuestionBtn,
        userAvatar
    },
    data() {
        return {
        }
    },
    props: {
        userName:{}
    },
    computed: {
        ...mapGetters({
            accountUser: 'accountUser',
            loginDialogState: 'loginDialogState'
        }),
    },
    methods:{
        ...mapActions(["updateLoginDialogState", 'updateUserProfileData', 'updateNewQuestionDialogState']),
        goToAskQuestion(){
            if(this.accountUser == null){
                this.updateLoginDialogState(true);
                //set user profile
                this.updateUserProfileData('profileHWH')
            }else{
                this.updateNewQuestionDialogState(true);
            }
        }
    },
    created() {
    }
}
