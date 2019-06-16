import { connectivityModule } from "./connectivity.module";
export default {
    requestTutor: (data) => {
        return connectivityModule.http.post(`tutor/request`, data)
                                 .then((resp) => {
                                     return resp.data;
                                 });
    }
};
