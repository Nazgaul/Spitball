<template>
    <div>
        <component v-for="(item, index) in componentsList" :key="index" :is="item.name" :params="item.params"></component>
    </div>
</template>

<script>
import * as componentConsts from './componentConsts.js';

const PAYMENT_DIALOG = () => import('../dialogInjection/globalDialogs/payment/paymentWrapper.vue');
// const TUTOR_EDIT_PROFILE = () => import('../../../new_profile/profileHelpers/userInfoEdit/tutorInfoEdit.vue');

const auth = () => import('../../global/dialogInjection/globalDialogs/auth/auth.vue')

const simpleToaster = () => import('./simpleToaster.vue');
const simpleErrorToaster = () => import('./simpleErrorToaster.vue')
const errorLinkToaster = () => import('./errorLinkToaster.vue')
const PURCHASE_TRANSACTION = () => import('./buyPointsTransaction.vue')

const createCoupon = () => import('../../dashboardPage/dashboardDialog/createCouponDialog.vue');
const applyCoupon = () => import('./applyCoupon.vue')

const editStudentInfo = () => import('../../../new_profile/profileHelpers/userInfoEdit/userInfoEdit.vue')

const verifyPhone = () => import('../dialogInjection/globalDialogs/auth/register/verifyPhone.vue')
const studRoomSettings = () => import('../../../studyroom/tutorHelpers/studyRoomSettingsDialog/studyRoomSettingsDialog.vue')
const SESSION_CREATE_DIALOG = () => import('../../dashboardPage/myStudyRooms/createStudyRoomDialog.vue')

const teacherBillOfflineDialog = () => import('../dialogInjection/globalDialogs/teacherApproval/teacherBillOffline.vue');
const ITEM_DIALOG = () => import('../itemDialog/itemDialog.vue');

const paymentDialog = () => import('../../../pages/coursePage/courseDialog.vue');
export default {
    components: {
        PAYMENT_DIALOG,
        // TUTOR_EDIT_PROFILE,
        auth,
        simpleToaster,
        simpleErrorToaster,
        errorLinkToaster,
        PURCHASE_TRANSACTION,
        createCoupon,
        verifyPhone,
        editStudentInfo,
        applyCoupon,
        studRoomSettings,
        SESSION_CREATE_DIALOG,
        teacherBillOfflineDialog,
        ITEM_DIALOG,
        paymentDialog
    },
    data() {
        return {
            component: {},
            componentObj: {
                // DIALOGS:
                [componentConsts.COURSE_PAYMENT_DIALOG]:{
                    name: 'paymentDialog'
                },
                [componentConsts.ITEM_DIALOG]:{
                    name:componentConsts.ITEM_DIALOG,
                    params:{
                        name: componentConsts.ITEM_DIALOG
                    }
                },
                [componentConsts.PAYMENT_DIALOG]:{
                    name: componentConsts.PAYMENT_DIALOG,
                },
                // [componentConsts.TUTOR_EDIT_PROFILE]: {
                //     name: componentConsts.TUTOR_EDIT_PROFILE 
                // },
                // TOASTERS:ERRORS
                [componentConsts.BOOK_FAILED]:{
                    name:'simpleErrorToaster',
                    params:{
                        text: this.$t("calendar_error_create_event"),
                        name: componentConsts.BOOK_FAILED
                    }
                },
                [componentConsts.CREATE_BROADCAST_ERROR]:{
                    name:'simpleErrorToaster',
                    params:{
                        text: this.$t("create_broadcast_error"),
                        name: componentConsts.CREATE_BROADCAST_ERROR
                    }
                },
                [componentConsts.WENT_WRONG]:{
                    name:'simpleErrorToaster',
                    params:{
                        text: this.$t("tutorRequest_request_error"),
                        name: componentConsts.WENT_WRONG
                    }
                },
                [componentConsts.FILE_NOT_SUPPORTED]:{
                    name:'simpleErrorToaster',
                    params:{
                        text: this.$t("upload_multiple_error_extension_title"),
                        name: componentConsts.FILE_NOT_SUPPORTED
                    }
                },
                [componentConsts.ENROLLED_ERROR]:{
                    name:'simpleErrorToaster',
                    params:{
                        text: this.$t('profile_enroll_error'),
                        name: componentConsts.ENROLLED_ERROR
                    }
                },
                [componentConsts.ENROLLED_ERROR_2]:{
                    name:'simpleErrorToaster',
                    params:{
                        text: this.$t('profile_enroll_error_2'),
                        name: componentConsts.ENROLLED_ERROR_2
                    }
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
                [componentConsts.PURCHASE_TRANSACTION]:{
                    name: componentConsts.PURCHASE_TRANSACTION,
                    params: {
                        text: this.$t('buyTokens_success_transaction'),
                        timeout: 30000,
                        name: componentConsts.PURCHASE_TRANSACTION
                    }
                },
                simpleToaster_tutorConnected:{
                    name:'simpleToaster',
                    params: {
                        text: this.$t('studyRoom_toaster_tutor_added_to_room'),
                    }
                },
                simpleToaster_tutorLeft:{
                    name:'simpleToaster',
                    params: {
                        text: this.$t('studyRoom_toaster_tutor_left_the_room'),
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
                setPhone: {
                    name: 'auth',
                    params: {
                        component: 'setPhone',
                    }
                },
                verifyPhone: {
                    name: 'auth',
                    params: {
                        component: 'register',
                        goTo: 'verifyPhone'
                    }
                },
                studyRoomSettings: {
                    name: 'studRoomSettings',
                },

                [componentConsts.SESSION_CREATE_DIALOG]:{
                    name: componentConsts.SESSION_CREATE_DIALOG,
                    params:{
                        type: 'private',
                        name: componentConsts.SESSION_CREATE_DIALOG
                    }
                },
                createCoupon: {
                    name: 'createCoupon'
                },
                applyCoupon: {
                    name: 'applyCoupon'
                },
                editStudentInfo: {
                    name: 'editStudentInfo'
                }
            }
        }
    },
    watch: {
        componentsList:{
            deep:true,
            immediate:true,
            handler(val){
                if (val.length) {
                    if (this.$vuetify.breakpoint.xs) {
                        document.body.classList.add('noscroll')
                    }
                } else {
                    document.body.classList.remove('noscroll')
                }
            }
        }
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