import { connectivityModule } from "./connectivity.module"


export default {
    sendDocumentData: (data) => connectivityModule.http.post("/document", data),
    getDocument : (id) => connectivityModule.http.get("/document", id),
}