import { connectivityModule } from "./connectivity.module"


function FileData(ObjInit){
        this.blobName = ObjInit.blobName || '';
        this.name= ObjInit.name || '';
        this.type= ObjInit.type || '';
        this.courses= ObjInit.courses || '';
        this.tags = ObjInit.tags || [];
        this.proffesorName= ObjInit.proffesorName || '';
}

function createFileData(ObjInit){
    return new FileData(ObjInit)
}

export default {
    uploadDropbox: (file) => connectivityModule.http.post("/upload/dropbox", file),
    createFileData
}