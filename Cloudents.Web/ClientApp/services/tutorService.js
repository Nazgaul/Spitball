import { connectivityModule } from "./connectivity.module";
export default {
    requestTutor: (data) => {
        return connectivityModule.http.post(`tutor/request`, data)
                                 .then((resp) => {
                                     console.log(':::data returned, tutor request:::', resp.data)
                                     return resp.data;
                                 }, (error) => {
                                     console.log('Error request tutor', error);
                                 });
    },
    requestTutorAnonymous: (data) => {
        return connectivityModule.http.post(`tutor/anonymousRequest`, data)
                                 .then((resp) => {
                                     console.log(':::data returned, tutor request:::', resp.data)
                                     return resp.data;
                                 }, (error) => {
                                     console.log('Error request tutor', error);
                                 });
    }


};