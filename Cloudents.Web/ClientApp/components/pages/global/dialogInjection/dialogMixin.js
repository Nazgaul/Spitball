import { mapGetters } from 'vuex';

const becomeTutor = () => import('../../../becomeTutor/becomeTutor.vue');
const exitRegisterDialog = () => import('../../authenticationPage/login/exitRegisterDialog.vue');
const upload = () => import('../../../uploadFilesDialog/uploadMultipleFiles.vue');
const createCoupon = () => import('../../dashboardPage/dashboardDialog/createCouponDialog.vue');
const login = () => import('../../authenticationPage/dialogs/loginToAnswer/login-answer.vue');
const payment = () => import('./globalDialogs/payment/payment.vue');


export default {
    components: {
        becomeTutor,
        exitRegisterDialog,
        upload,
        createCoupon,
        login,
        payment
    },
    data() {
        return {
            dialogsPremissions: {
                login: [],
                exitRegisterDialog: [],
                becomeTutor: [],
                upload: ["auth","courses"],
                createCoupon: ["auth","tutor"],
                payment:["auth","notPayment"]
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
            if(!this.getUserLoggedInStatus && !global.isAuth){
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
        },
        // check_payment(){
        //     if(!this.accountUser.needPayment){
        //         this.component = 'payment';
        //         // TODO: do something
        //     }
        // },
        check_notPayment(){
            if(this.accountUser.needPayment){
                // TODO: do something
                return 'break'
            }
        }
    },
    watch: {
        '$route.query.dialog':function(val){
            if(val === 'payment'){
                setTimeout(function() {
                    document.querySelector(".payme-popup").parentNode.style.zIndex = 999;
                }, 1000);
            }
        }
    },
}