// import { lazyComponent } from '../../../../routes/routesUtils';
import dialogConfig from './dialogConfig.json';

const becomeTutor = () => import('../../../becomeTutor/becomeTutor.vue');
const login = () => import('../../authenticationPage/login/exitRegisterDialog.vue');
const upload = () => import('../../../uploadFilesDialog/uploadMultipleFiles.vue');
const createCoupon = () => import('../../dashboardPage/dashboardDialog/createCouponDialog.vue');

// function initDialogComponents() {
//     let dialogs = {}, dialogIndex;

//     for (dialogIndex in dialogConfig) {
//         dialogs[dialogIndex] = lazyComponent(dialogConfig[dialogIndex].path)
//     }
    
//     return dialogs;
// }

export { dialogConfig }
export default {
    components: {
        becomeTutor,
        login,
        upload,
        createCoupon,
    }
}