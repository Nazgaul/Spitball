import { connectivityModule } from "./connectivity.module";
import searchService from './searchService'
export default {
    getTutorList: (objReq) =>{
        let params = {page :objReq.page};
        if (objReq.courseName) { // we need to check this because we dont want empty query string
            params.courseName = objReq.courseName;
        }
        return connectivityModule.http.get("tutor",{
            params: params
        }).then(({data})=> {
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