import { connectivityModule } from "./connectivity.module";
// TODO: move to maor_studyroom service
const sendReview = (review)=>{
    return connectivityModule.http.post("studyRoom/review", review);
};
export default {
    sendReview
}
