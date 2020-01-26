import {connectivityModule} from '../../../../services/connectivity.module'


export const deleteQuestion =(ids)=>{
    let idstoDelete = {'id': ids};
    let url = 'AdminQuestion'
    return connectivityModule.http.delete(url, idstoDelete)
    .then((resp)=>{
        console.log(resp, 'success deleted');
        return Promise.resolve(resp);
    }, (error)=>{
        console.log(error, 'error deleted');
        return Promise.reject(error);
    });
};
