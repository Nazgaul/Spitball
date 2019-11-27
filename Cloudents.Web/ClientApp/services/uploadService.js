import { connectivityModule } from "./connectivity.module"
import { Upload } from './constructors.js'

function createServerFileData(objInit){
    return new Upload.ServerFormatFileData(objInit);
}

function createFileData(objInit){
    return new Upload.FileData(objInit);
}


export default {
    uploadDropbox: (file) => connectivityModule.http.post("/Document/dropbox", file),
    createFileData,
    createServerFileData
}

// REMOVE IT
// function FileData(objInit){
//         this.id = objInit.id || '';
//         this.blobName = objInit.blobName || '';
//         this.name= objInit.name || '';
//         this.course= objInit.course || '';
//         this.price = objInit.price || '';
//         this.progress = objInit.progress || 100;
//         this.link  = objInit.link || '';
//         this.size  = objInit.bytes || 0;
//         this.error = objInit.error || false;
//         this.errorText = objInit.errorText || '';
//         this.description = objInit.description || '';
// }

// REMOVE IT
// function ServerFormatFileData(objInit) {
//     this.id = objInit.id || '';
//     this.blobName = objInit.blobName || '';
//     this.name= objInit.name || '';
//     this.course= objInit.course || '';
//     this.price = objInit.price || '';
//     this.link  = objInit.link || '';
//     this.size  = objInit.bytes || 0;
//     this.description = objInit.description || '';
// }