import Vue from 'vue'
import { Provide } from 'vue-property-decorator'
import axios from 'axios'

export interface IConnectivityModule{
    get: any
    post: any
    put: any
    delete: any
}

//const api = window.location.origin + '/api';
const api = 'https://localhost:44384/api';

class ConnectivityModule extends Vue implements IConnectivityModule{
    get: any;    
    post: any;
    put: any;
    delete: any;
    
    constructor(){
        super();

        this.get = function(path: string) {
            return axios.get(`${api}/${path}`);
        }
        this.post = function(path: string, data: any) {
            return axios.post(`${api}/${path}`, data);
        }
        this.put = function(path: string, data: any) {
            return axios.put(`${api}/${path}`, data);
        }
        this.delete = function(path: string) {
            return axios.delete(`${api}/${path}`);
        }
    }
}
    
export const connectivityModule = new ConnectivityModule();