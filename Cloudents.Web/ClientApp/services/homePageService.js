import { connectivityModule } from "./connectivity.module";
import searchService from './searchService.js'

function getHomePageTutors(count = 12){
    let params = {count}
    return connectivityModule.http.get(`HomePage/tutors`,{params}).then(res=>{
        return res.data.map(tutor=>{
            return searchService.createTutorItem(tutor)
        })
    })
}
function getHomePageItems(count = 12){
    let params = {count}
    return connectivityModule.http.get(`HomePage/documents`,{params}).then(res=>{
        return res.data.map(item=>{
            return searchService.createDocumentItem(item)
        })
    })
}
function getHomePageSubjects(count = 12){
    let params = {count}
    return connectivityModule.http.get(`HomePage/subjects`,{params}).then(res=>{
        return res.data.map(subject=>subject)
    })
}
function getHomePageStats(){
    return connectivityModule.http.get(`HomePage`).then(res=>{
        return createHomePageStats(res.data)
    })
}
function getHomePageReviews(count = 3){
    let params = {count}
    return connectivityModule.http.get(`HomePage/reviews`,{params}).then(res=>{
        return res.data.map(review=>{
            return createHomePageReviews(review)
        })
    })
}
function createHomePageStats(objInit){
    return new HomePageStats(objInit)
}
function createHomePageReviews(objInit){
    return new HomePageReview(objInit)
}
function HomePageReview(objInit){
    this.text = objInit.text;
    this.userName = objInit.userName;
    this.tutorImage = objInit.tutorImage;
    this.tutorName = objInit.tutorName;
    this.tutorId = objInit.tutorId;
    this.tutorReviews = objInit.tutorReviews;
}

function HomePageStats(objInit){
    this.documents = objInit.documents;
    this.tutors = objInit.tutors;
    this.students = objInit.students;
    this.reviews = objInit.reviews;
}
export default {
    getHomePageTutors,
    getHomePageItems,
    getHomePageSubjects,
    getHomePageStats,
    getHomePageReviews
}


