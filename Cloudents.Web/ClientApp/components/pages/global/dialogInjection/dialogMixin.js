import { mapGetters } from 'vuex';

const becomeTutor = () => import('../../../becomeTutor/becomeTutor.vue');
const exitRegisterDialog = () => import('../../authenticationPage/login/exitRegisterDialog.vue');
const upload = () => import('../../../uploadFilesDialog/uploadMultipleFiles.vue');
const createCoupon = () => import('../../dashboardPage/dashboardDialog/createCouponDialog.vue');
const login = () => import('../../authenticationPage/dialogs/loginToAnswer/login-answer.vue');


export default {
    components: {
        becomeTutor,
        exitRegisterDialog,
        upload,
        createCoupon,
        login,
    },
    data() {
        return {
            dialogsPremissions: {
                login: [],
                exitRegisterDialog: [],
                becomeTutor: ["auth"],
                upload: ["auth","courses"],
                createCoupon: ["auth","tutor"]
            }
        }
    },
    computed: {
        ...mapGetters([
            'getUserLoggedInStatus',
            'accountUser',
            'getSelectedClasses',
        ])
    },
    methods: {
        dialogHandlerByType(premissionType,dialogNameFromRoute){
            let dialogChekerName = `check_${premissionType}`;
            // TODO make it Promise!!
                return this[dialogChekerName](dialogNameFromRoute);
        },
        check_auth(){
            if(!this.getUserLoggedInStatus){
                this.component = 'login';
            }
        },
        check_tutor(){
            if(!this.accountUser.isTutor){
                this.component = '';
                this.$closeDialog()
                return 'break'
            } 
        },
        check_courses(){
            if(this.getSelectedClasses.length === 0){
                this.$router.push({name: "addCourse"})
                return 'break'
            }
        }
    },
}