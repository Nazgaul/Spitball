// import { help } from './resources';
import { connectivityModule } from "./connectivity.module"
let cacheControl = `?v=${global.version}&l=${global.lang}`;
export default {
    getFaq:() => {
        return connectivityModule.http.get(`help${cacheControl}`);
    },

    getBlog:(id) => {
        connectivityModule.get("blog?id=" + id).then((f)=> {
            return f.data;
        });
    }
}
