import { connectivityModule } from "./connectivity.module";

function TutorItem(objInit) {
    this.userId = objInit.userId;
    this.name = objInit.name;
    this.image = objInit.image;
    this.courses = objInit.courses || '';
    this.price = objInit.price || 50;
    this.score = objInit.score;
    this.rating = objInit.rate || null;
}


export default {
    requestTutor: (objReq) => {
        return connectivityModule.http.get(`tutor`)
                                 .then((resp) => {
                                     return resp.data;
                                 }, (error) => {
                                     console.log('Error request tutor', error);
                                 });
    }

};