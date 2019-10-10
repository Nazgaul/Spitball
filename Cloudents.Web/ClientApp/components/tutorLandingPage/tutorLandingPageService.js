
import {connectivityModule} from '../../services/connectivity.module';
import searchService from '../../services/searchService';

const getTutorList = (params) => {
    return connectivityModule.http.get("tutor/search", { params }).then(({data})=>{
        return data.result.map(searchService.createTutorItem);
    });
};

export default{
    getTutorList
}
