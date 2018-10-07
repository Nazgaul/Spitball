import userAvatar from "../../../helpers/UserAvatar/UserAvatar.vue";
import askQuestionBtn from '../askQuestionBtn/askQuestionBtn.vue';
import { mapGetters, mapActions } from 'vuex';

export default {
    components: {
        askQuestionBtn,
        userAvatar
    },
    data() {
        return {
            offsetTop: 0,
            hideABTestBlock: true
        }
    },
    props: {
        userName: {}
    },
    computed: {
        ...mapGetters({
            accountUser: 'accountUser',
            loginDialogState: 'loginDialogState'
        }),
    },
    methods: {
        ...mapActions(["updateLoginDialogState", 'updateUserProfileData', 'updateNewQuestionDialogState']),
        goToAskQuestion() {
            if (this.accountUser == null) {
                this.updateLoginDialogState(true);
                //set user profile
                this.updateUserProfileData('profileHWH')
            } else {
                this.updateNewQuestionDialogState(true);
            }
        },
        hideOnMobileScroll(e) {
                this.offsetTop = window.pageYOffset || document.documentElement.scrollTop;
                console.log(this.offsetTop)
        },
    },
    computed:{
        isHiddenBlock(){
            ///if (this.$vuetify.breakpoint.name === 'xs' || 'sm') {
                return this.offsetTop >= 75 && (this.$vuetify.breakpoint.name === 'xs' && 'sm')
          //  }
        },

    },

    created() {
    }
}
