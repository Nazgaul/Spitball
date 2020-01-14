
import {connectivityModule} from '../../services/connectivity.module';
import searchService from '../../services/searchService';


function createTutorList(objInit){
    return new TutorList(objInit);
}

function TutorList(objInit){
    this.count = objInit.count;
    this.nextPageLink = objInit.nextPageLink;
    this.result = objInit.result.map(searchService.createTutorItem);
}
const getTutorList = (params) => {
    return connectivityModule.http.get("tutor/search", { params }).then(({data})=>{
        return createTutorList(data);
    });
};

export default {
    getTutorList
}