import { mapGetters } from 'vuex';
//import * as dialogNames from './dialogNames.js'
// const exitRegister = () => import('../../authenticationPage/login/exitRegisterDialog.vue');
// const createCoupon = () => import('../../dashboardPage/dashboardDialog/createCouponDialog.vue');
// const login = () => import('./globalDialogs/login/login.vue');
// const buyPoints = () => import('./globalDialogs/buyPoints/buyPointsWrapper.vue');
// const teacherApproval = () => import('./globalDialogs/teacherApproval/teacherApproval.vue');

// const createStudyRoom = () => import('../../dashboardPage/myStudyRooms/createStudyRoomDialog.vue');


export default {
    // components: {
    //     exitRegister,
    //     createCoupon,
    //     login,
    //     buyPoints,
    //     teacherApproval,
    //     createStudyRoom
    // },
    data() {
        return {
            dialogsPremissions: {
                login: ["notAuth"],
                exitRegister: [],
                createCoupon: ["auth","tutor"],
                buyPoints:["auth"],
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
            if(!this.getUserLoggedInStatus){
                this.component = dialogNames.Login;
            }
        },
        // check_notAuth(){
        //     if(this.getUserLoggedInStatus && global.isAuth){
        //         this.component = '';
        //         this.$closeDialog()
        //         return 'break'
        //     }
        // },
        // check_tutor(){
        //     if(!this.accountUser.isTutor){
        //         this.component = '';
        //         this.$closeDialog()
        //         return 'break'
        //     }
        // },
        // check_params() {
        //     if(!Object.keys(this.$route.params).length) {
        //         this.component = '';
        //         this.$closeDialog()
        //         return 'break'
        //     }
        // },
    },
  
}