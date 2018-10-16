import userAvatar from "../../../helpers/UserAvatar/UserAvatar.vue";
import askQuestionBtn from '../askQuestionBtn/askQuestionBtn.vue';
import {mapActions, mapGetters} from 'vuex';

export default {
    components: {
        askQuestionBtn,
        userAvatar
    },
    data() {
        return {
            offsetTop: 0,
            offsetTop2: 0,
            hideABTestBlock: true,

        }
    },
    props: {
        userName: {}
    },
    computed: {
        ...mapGetters(['accountUser', 'loginDialogState' ]),
        isHiddenBlock(){
            ///if (this.$vuetify.breakpoint.name === 'xs' || 'sm') {
            return this.offsetTop >= 75 && (this.$vuetify.breakpoint.name === 'xs' && 'sm')
            //  }
        },
        isFloatingBtn(){
            return this.offsetTop2 >= 150 && (this.$vuetify.breakpoint.name === 'xs' && 'sm')

        }
    },
    methods: {
        ...mapActions(["updateLoginDialogState", 'updateUserProfileData', 'updateNewQuestionDialogState']),
        goToAskQuestion(from) {
           // console.log(this.accountUser);
            if (this.accountUser == null) {
                this.updateLoginDialogState(true);
                //set user profile
                this.updateUserProfileData('profileHWH')
            } else {
                let Obj = {
                    status:true,
                    from: from
                }
                this.updateNewQuestionDialogState(Obj);
            }
        },
        hideOnMobileScroll(e) {
                this.offsetTop = window.pageYOffset || document.documentElement.scrollTop;
        },
        transformToBtn(){
            this.offsetTop2 = window.pageYOffset || document.documentElement.scrollTop;

        },
    },


    created() {
        console.log(...mapGetters);
    }
}
