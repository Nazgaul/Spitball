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
    }
}