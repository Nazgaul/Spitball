import { connectivityModule } from "./connectivity.module";


export default {
    getToken: (path, name) => {
        return connectivityModule.http.get(`${path}?identity=${name}`)
    }

}