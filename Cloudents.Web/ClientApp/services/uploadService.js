import { connectivityModule } from "./connectivity.module"


function FileData(objInit){
        this.id = objInit.id || '';
        this.blobName = objInit.blobName || '';
        this.name= objInit.name || '';
        this.type= objInit.type || '';
        this.course= objInit.course || '';
        this.tags = objInit.tags || [];
        this.professor= objInit.professor || '';
        this.price = objInit.price || '';
        this.progress = objInit.progress || 100;
        this.link  = objInit.link || '';
        this.size  = objInit.bytes || 0;
        this.error = objInit.error || false;
        this.errorText = objInit.errorText || '';
}


function  ServerFormatFileData(objInit) {
    this.id = objInit.id || '';
    this.blobName = objInit.blobName || '';
    this.name= objInit.name || '';
    this.type= objInit.type || '';
    this.course= objInit.course || '';
    this.tags = objInit.tags || [];
    this.professor= objInit.professor || '';
    this.price = objInit.price || '';
    this.link  = objInit.link || '';
    this.size  = objInit.bytes || 0;

}


function createServerFileData(objInit){
    return new ServerFormatFileData(objInit);
}


function createFileData(objInit){
    return new FileData(objInit);
}


export default {
    uploadDropbox: (file) => connectivityModule.http.post("/Document/dropbox", file),
    createFileData,
    createServerFileData
}