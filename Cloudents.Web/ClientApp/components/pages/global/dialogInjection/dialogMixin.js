import { mapGetters } from 'vuex';

const becomeTutor = () => import('../../../becomeTutor/becomeTutor.vue');
const exitRegisterDialog = () => import('../../authenticationPage/login/exitRegisterDialog.vue');
const upload = () => import('../../../uploadFilesDialog/uploadMultipleFiles.vue');
const createCoupon = () => import('../../dashboardPage/dashboardDialog/createCouponDialog.vue');
const login = () => import('./globalDialogs/login/login.vue');


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
                login: ["isNotAuth"],
                exitRegisterDialog: [],
                becomeTutor: [],
                upload: ["isAuth","isCourses"],
                createCoupon: ["isAuth","isTutor"]
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
        check_isAuth(){
            if(!this.getUserLoggedInStatus && !global.isAuth){
                this.component = 'login';
            }
        },
        check_isNotAuth(){
            if(this.getUserLoggedInStatus && global.isAuth){
                this.component = '';
                this.$closeDialog()
                return 'break'
            }
        },
        check_isTutor(){
            if(!this.accountUser.isTutor){
                this.component = '';
                this.$closeDialog()
                return 'break'
            } 
        },
        check_isCourses(){
            if(this.getSelectedClasses.length === 0){
                this.$router.push({name: "addCourse"})
                return 'break'
            }
        }
    },
}