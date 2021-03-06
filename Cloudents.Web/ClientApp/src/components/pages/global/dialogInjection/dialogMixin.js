import { mapGetters } from 'vuex';
// const exitRegister = () => import('../../authenticationPage/login/exitRegisterDialog.vue');
// const login = () => import('./globalDialogs/login/login.vue');
// const teacherApproval = () => import('./globalDialogs/teacherApproval/teacherApproval.vue');



export default {
    // components: {
    //     exitRegister,
    //     login,
    //     teacherApproval,
    // },
    data() {
        return {
            dialogsPremissions: {
                login: ["notAuth"],
                exitRegister: [],
                createCoupon: ["auth","tutor"],
                teacherApproval:["auth", "tutor", "params"],
                createStudyRoom:["auth","tutor"],
            }
        }
    },
    computed: {
        ...mapGetters([
            'getUserLoggedInStatus',
            'accountUser',
            'isFrymo'
        ])
    },
    methods: {
        dialogHandlerByType(premissionType,dialogNameFromRoute){
            let dialogChekerName = `check_${premissionType}`;
            // TODO make it Promise!!
                return this[dialogChekerName](dialogNameFromRoute);
        },
        check_auth(){
        },
        check_notAuth(){
            if(this.getUserLoggedInStatus && global.isAuth){
                this.component = '';
                this.$closeDialog()
                return 'break'
            }
        },
        check_tutor(){
            if(!this.$store.getters.getIsTeacher){
                this.component = '';
                this.$closeDialog()
                return 'break'
            }
        },
        check_params() {
            if(!Object.keys(this.$route.params).length) {
                this.component = '';
                this.$closeDialog()
                return 'break'
            }
        },
    },
  
}