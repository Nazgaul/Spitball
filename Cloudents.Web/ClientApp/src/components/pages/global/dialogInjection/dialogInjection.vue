<template>
    <component :is="component"></component>
</template>

<script>
const exitRegister = () => import('../../authenticationPage/login/exitRegisterDialog.vue');
const createCoupon = () => import('../../dashboardPage/dashboardDialog/createCouponDialog.vue');
const login = () => import('./globalDialogs/login/login.vue');
const buyPoints = () => import('./globalDialogs/buyPoints/buyPointsWrapper.vue');
const teacherApproval = () => import('./globalDialogs/teacherApproval/teacherApproval.vue');

const createStudyRoom = () => import('../../dashboardPage/myStudyRooms/createStudyRoomDialog.vue');

import dialogMixin from './dialogMixin.js'
export default {
    mixins: [dialogMixin],
     components: {
        exitRegister,
        createCoupon,
        login,
        buyPoints,
        teacherApproval,
        createStudyRoom,
    },
    data: () => ({
        component: ''
    }),
    watch: {
        "$route.query.dialog": "openDialog",
        component(val){
            if (val) {
                if (this.$vuetify.breakpoint.xs) {
                    document.body.classList.add('noscroll')
                }
            } else {
                document.body.classList.remove('noscroll')
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
                    }
                    self.component = dialogNameFromRoute
                    return
                    
                })
            }   
        }
    }
}
</script>