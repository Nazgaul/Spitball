import { connectivityModule } from "./connectivity.module";


export default {
    generateRoom: () => {
        return connectivityModule.http.post("tutoring/create");
    },
    getToken: (name) => {
        return connectivityModule.http.get(`tutoring/join?roomName=${name}`).then(data => data.data.token);
    }

}