import { lazyComponent } from '../../../../routes/routesUtils';
import dialogConfig from './dialogConfig.json';

function initDialogComponents() {
    let dialogs = {}, dialogIndex;

    for (dialogIndex in dialogConfig) {
        dialogs[dialogIndex] = lazyComponent(dialogConfig[dialogIndex].path)
    }
    
    return dialogs;
}

export { dialogConfig }
export default { components: initDialogComponents() }