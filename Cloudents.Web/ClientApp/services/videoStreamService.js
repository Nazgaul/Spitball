import { connectivityModule } from "./connectivity.module";


export default {
    generateRoom: () => {
        return connectivityModule.http.post("tutoring/create");
    },
    getToken: (name, identityName) => {
        let userIdentity = identityName || '';
        return connectivityModule.http.get(`tutoring/join?roomName=${name}&identityName=${userIdentity}`)
            .then((data) => {
                return data.data.token
            });
    }

}