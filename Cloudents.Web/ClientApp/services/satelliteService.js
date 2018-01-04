import { help } from './resources';
export default {
    getFaq() {
        return help.getFaq();
    },
    getBlog(id) {
        return help.getUniData(id).then(f=> f.data);
    }
}