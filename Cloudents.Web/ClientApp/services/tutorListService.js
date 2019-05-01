import { connectivityModule } from "./connectivity.module";
import searchService from './searchService'
export default {
    getTutorList: (objReq) =>{
        // return connectivityModule.http.get(`tutor?page=${objReq.page}&courseName=${objReq.courseName}`).then(({data})=> {
         return connectivityModule.http.get(`tutor?page=${objReq.page}&courseName=${objReq.courseName}`).then(({data})=> {
            let result = [];
            if(!!data && data.length > 0){
                data.forEach((tutorItem)=>{
                    result.push(searchService.createTutorItem(tutorItem));
                });
            }
            return result;
        })
    }

};