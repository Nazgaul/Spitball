<template>
    <div>
        <component v-for="(item, index) in componentsList" :key="index" :is="item.name" :params="item.params"></component>
    </div>
</template>

<script>
import * as componentConsts from './componentConsts.js';

const PAYMENT_DIALOG = () => import('../dialogInjection/globalDialogs/payment/paymentWrapper.vue');


const auth = () => import('../../global/dialogInjection/globalDialogs/auth/auth.vue')

const simpleToaster = () => import('./simpleToaster.vue');
const simpleErrorToaster = () => import('./simpleErrorToaster.vue')
const errorLinkToaster = () => import('./errorLinkToaster.vue')

const upload = () => import('../../../uploadFilesDialog/uploadMultipleFiles.vue')
const createCoupon = () => import('../../dashboardPage/dashboardDialog/createCouponDialog.vue');

const verifyPhone = () => import('../dialogInjection/globalDialogs/auth/register/verifyPhone.vue')

const applyCoupon = () => import('./applyCoupon.vue')
const studRoomSettings = () => import('../../../studyroom/tutorHelpers/studyRoomSettingsDialog/studyRoomSettingsDialog.vue')
const createStudyRoomDialog = () => import('../../dashboardPage/myStudyRooms/createStudyRoomDialog.vue')
const teacherBillOfflineDialog = () => import('../dialogInjection/globalDialogs/teacherApproval/teacherBillOffline.vue');
export default {
    components: {
        PAYMENT_DIALOG,
        
        auth,
        simpleToaster,
        simpleErrorToaster,
        errorLinkToaster,
        upload,
        createCoupon,
        verifyPhone,
        applyCoupon,
        studRoomSettings,
        createStudyRoomDialog,
        teacherBillOfflineDialog
    },
    data() {
        return {
            component: {},
            componentObj: {
                [componentConsts.PAYMENT_DIALOG]:{
                    name: componentConsts.PAYMENT_DIALOG,
                },
                teacherBillOfflineDialog:{
                    name:'teacherBillOfflineDialog'
                },
                errorToaster_notBrowser:{
                    name:'simpleErrorToaster',
                    params: {
                        text: this.$t("studyRoom_not_browser"),
                    }
                },
                errorToaster_notScreen:{
                    name:'simpleErrorToaster',
                    params: {
                        text: this.$t("studyRoom_not_screen"),
                    }
                },
                errorToaster_sessionEnded:{
                    name:'simpleErrorToaster',
                    params: {
                        text: this.$t("studyRoom_already_ended"),
                    }
                },
                errorToaster_permissionDenied:{
                    name:'errorLinkToaster',
                    params: {
                        text: this.$t('studyRoom_premission_denied',['https://support.apple.com/en-il/guide/mac-help/mchld6aa7d23/mac']),
                        timeout: 30000,
                    }
                },
                simpleToaster_userConnected:{
                    name:'simpleToaster',
                    params: {
                        text: this.$t('studyRoom_toaster_added_to_room'),
                    }
                },
                simpleToaster_userLeft:{
                    name:'simpleToaster',
                    params: {
                        text: this.$t('studyRoom_toaster_left_the_room'),
                    }
                },
                login: {
                    name: 'auth',
                    params: {
                        component: 'login',
                    }
                },
                register: {
                    name: 'auth',
                    params: {
                        component: 'register'
                    }
                },
                registerTeacher: {
                    name: 'auth',
                    params: {
                        component: 'register',
                        teacher: true
                    }
                },
                registerType: {
                    name: 'auth',
                    params: {
                        component: 'registerType'
                    }
                },
                studyRoomSettings: {
                    name: 'studRoomSettings',
                },
                createPrivateSession: {
                    name: 'createStudyRoomDialog',
                    params: {
                        type: 'private'
                    }
                },
                createLiveSession: {
                    name: 'createStudyRoomDialog',
                    params: {
                        type: 'broadcast'
                    }
                },
                upload: {
                    name: 'upload',
                },
                createCoupon: {
                    name: 'createCoupon'
                },
                verifyPhone: {
                    name: 'auth',
                    params: {
                        component: 'register',
                        goTo: 'verifyPhone'
                    }
                },
                applyCoupon: {
                    name: 'applyCoupon'
                }
            }
        }
    },
    watch: {
        componentsList:{
            deep:true,
            immediate:true,
            handler(){}
        },
    },
    computed: {
        componentsList(){
            return this.$store.getters.getComponent.map(cmp=>{
                return this.componentObj[cmp] || {name: '', params: ''};
            })
        }
    }
}
</script>