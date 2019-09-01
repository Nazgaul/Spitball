import searchService from './searchService'
export default {
    getTutorList: (objReq) =>{
        return searchService.getTutorsByCourse(objReq.courseName).then(({data})=> {
            let result = [];
            if(!!data && data.length > 0){
                data.forEach((tutorItem)=>{
                    result.push(searchService.createTutorItem(tutorItem));
                });
            }
            return result;
        });
    }

};