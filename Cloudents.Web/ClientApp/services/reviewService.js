import { connectivityModule } from "./connectivity.module";

const sendReview = (review)=>{
    return connectivityModule.http.post("studyRoom/review", review);
};
export default {
    sendReview
}
