import { connectivityModule } from "./connectivity.module";
// TODO: move to studyroom service
const sendReview = (review)=>{
    return connectivityModule.http.post("studyRoom/review", review);
};
export default {
    sendReview
}
