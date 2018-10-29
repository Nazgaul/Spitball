import { connectivityModule } from "./connectivity.module"


export default {
    uploadDropbox: (file) => connectivityModule.http.post("/upload/dropbox", {file}),

}