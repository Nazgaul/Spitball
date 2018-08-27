// import { help } from './resources';
import { connectivityModule } from "./connectivity.module"

export default {
    getFaq:() => {
        return connectivityModule.http.get("help")
    },

    getBlog:(id) => {
        connectivityModule.get("blog?id=" + id).then((f)=> {
            return f.data
        });
    }
}
