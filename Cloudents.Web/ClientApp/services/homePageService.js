import { connectivityModule } from "./connectivity.module";
import { HomePage } from './constructors.js'
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
    return new HomePage.Stats(objInit)
}
function createHomePageReviews(objInit){
    return new HomePage.Review(objInit)
}
export default {
    getHomePageTutors,
    getHomePageItems,
    getHomePageSubjects,
    getHomePageStats,
    getHomePageReviews
}


