import { mapGetters } from 'vuex';
import * as dialogNames from './dialogNames.js'
const becomeTutor = () => import('../../../becomeTutor/becomeTutor.vue');
const exitRegister = () => import('../../authenticationPage/login/exitRegisterDialog.vue');
const upload = () => import('../../../uploadFilesDialog/uploadMultipleFiles.vue');
const createCoupon = () => import('../../dashboardPage/dashboardDialog/createCouponDialog.vue');
const login = () => import('./globalDialogs/login/login.vue');
const buyPoints = () => import('./globalDialogs/buyPoints/buyPointsWrapper.vue');
const teacherApproval = () => import('./globalDialogs/teacherApproval/teacherApproval.vue');

const payment = () => import('./globalDialogs/payment/paymentWrapper.vue');



export default {
    components: {
        becomeTutor,
        exitRegister,
        upload,
        createCoupon,
        login,
        payment,
        buyPoints,
        teacherApproval
    },
    data() {
        return {
            dialogsPremissions: {
                login: ["notAuth"],
                exitRegister: [],
                becomeTutor: ["auth"],
                payment:["auth",'notFrymo'],
                upload: ["auth","courses"],
                createCoupon: ["auth","tutor"],
                buyPoints:["auth"],
                teacherApproval:["auth", "tutor", "params"]
            }
        }
    },
    computed: {
        ...mapGetters([
            'getUserLoggedInStatus',
            'accountUser',
            'getSelectedClasses',
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
            if(!this.getUserLoggedInStatus){
                this.component = dialogNames.Login;
            }
        },
        check_notAuth(){
            if(this.getUserLoggedInStatus && global.isAuth){
                this.component = '';
                this.$closeDialog()
                return 'break'
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
            this.$store.dispatch('getManageCourses').then(courses => {
                if(courses.length === 0){
                    this.$router.push({name: "addCourse"})
                    return 'break'
                }
            })
        },
        check_params() {
            if(!Object.keys(this.$route.params).length) {
                this.component = '';
                this.$closeDialog()
                return 'break'   
            }
        },
        check_notFrymo(){
            if(this.isFrymo){
                this.component = '';
                this.$closeDialog()
                return 'break'
            } 
        },
        // check_payment(){
        //     if(!this.accountUser.needPayment){
        //         this.component = 'payment';
        //         // TODO: do something
        //     }
        // },
        // check_notPayment(){
        //     if(this.accountUser.needPayment){
        //         // TODO: do something
        //         return 'break'
        //     }
        // }
    },
    watch: {
        '$route.query.dialog':function(val){
            if(val === dialogNames.Payment){
                setTimeout(function() {
                    //We need this because we another dialog opened
                    document.querySelector(".payme-popup").parentNode.style.zIndex = 999;
                }, 1000);
            }
        }
    },
}