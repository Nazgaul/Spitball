import { connectivityModule } from "./connectivity.module"


function FileData(ObjInit){
        this.id = ObjInit.id || '';
        this.blobName = ObjInit.blobName || '';
        this.name= ObjInit.name || '';
        this.type= ObjInit.type || '';
        this.course= ObjInit.course || '';
        this.tags = ObjInit.tags || [];
        this.professor= ObjInit.professor || '';
        this.price = ObjInit.price || '';
        this.progress = ObjInit.progress || 100;
        this.link  = ObjInit.link || '';
        this.size  = ObjInit.bytes || 0;
        this.error = ObjInit.error || false;
        this.errorText = ObjInit.errorText || ''
}


function  ServerFormatFileData(ObjInit) {
    this.id = ObjInit.id || '';
    this.blobName = ObjInit.blobName || '';
    this.name= ObjInit.name || '';
    this.type= ObjInit.type || '';
    this.course= ObjInit.course || '';
    this.tags = ObjInit.tags || [];
    this.professor= ObjInit.professor || '';
    this.price = ObjInit.price || '';
    this.link  = ObjInit.link || '';
    this.size  = ObjInit.bytes || 0;

}


function createServerFileData(ObjInit){
    return new ServerFormatFileData(ObjInit)
}


function createFileData(ObjInit){
    return new FileData(ObjInit)
}


export default {
    uploadDropbox: (file) => connectivityModule.http.post("/Document/dropbox", file),
    createFileData,
    createServerFileData
}