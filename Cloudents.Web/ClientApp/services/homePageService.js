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
function createHomePageStats(objInit){
    return new HomePageStats(objInit)
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
    getHomePageStats
}


