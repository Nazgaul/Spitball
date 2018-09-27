import {connectivityModule} from '../../../../services/connectivity.module'


    export const deleteQuestion =(ids)=>{
        let url = 'AdminMarkQuestion'
        return connectivityModule.http.delete(`${url}`, ids)
        .then((resp)=>{
            console.log(resp, 'success deleted')
            return Promise.resolve(resp) 
        }, (error)=>{
            console.log(error, 'error deleted')
            return Promise.reject(error) 
        })
    }
