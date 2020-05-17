<template>
    <component :is="component"></component>
</template>

<script>
//import { mapGetters } from 'vuex';
//import * as dialogNames from './dialogNames.js'
const becomeTutor = () => import('../../../becomeTutor/becomeTutor.vue');
const exitRegister = () => import('../../authenticationPage/login/exitRegisterDialog.vue');
const upload = () => import('../../../uploadFilesDialog/uploadMultipleFiles.vue');
const createCoupon = () => import('../../dashboardPage/dashboardDialog/createCouponDialog.vue');
const login = () => import('./globalDialogs/login/login.vue');
const buyPoints = () => import('./globalDialogs/buyPoints/buyPointsWrapper.vue');
const teacherApproval = () => import('./globalDialogs/teacherApproval/teacherApproval.vue');

const payment = () => import('./globalDialogs/payment/paymentWrapper.vue');
const createStudyRoom = () => import('../../dashboardPage/myStudyRooms/createStudyRoomDialog.vue');

import dialogMixin from './dialogMixin.js'
export default {
    mixins: [dialogMixin],
     components: {
        becomeTutor,
        exitRegister,
        upload,
        createCoupon,
        login,
        payment,
        buyPoints,
        teacherApproval,
        createStudyRoom
    },
    data: () => ({
        component: ''
    }),
    watch: {
        "$route.query.dialog": "openDialog",
        component(val){
            if (val) {
                if (this.$vuetify.breakpoint.xs) {
                    document.getElementsByTagName("body")[0].className = "noscroll";
                }
            } else {
                document.body.removeAttribute("class", "noscroll");
            }
        }
    },
    methods: {
        openDialog(dialogNameFromRoute){
            if(!dialogNameFromRoute){
                this.component = ''
                this.$closeDialog()
                return;
            }else{
                let currentDialogPremissionList = this.dialogsPremissions[dialogNameFromRoute];
                let self = this;
                this.$nextTick(()=>{
                    for(let premissionType of currentDialogPremissionList){
                        let result = self.dialogHandlerByType(premissionType,dialogNameFromRoute)                        
                        if(result === 'break'){
                            self.component = 'break';
                            break;
                        }
                        if(self.component){
                            break;
                        }
                    }

                    if(self.component === 'break'){
                        self.component = ''
                        return
                    }
                    if(self.component){
                        return
                    }else{
                        self.component = dialogNameFromRoute
                        return
                    }
                })
            }   
        }
    }
}
</script>