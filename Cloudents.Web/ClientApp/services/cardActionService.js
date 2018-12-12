import { connectivityModule } from "./connectivity.module"

const report = (itemId, reason) => {
    console.log('got to send report stuff service')
    let data = {
        "id": itemId,
        "reason": reason
    };
    return connectivityModule.http.post("Report/", data)
};

export default {
    reportItem(itemId, reason) {
        return report(itemId, reason)
    },

}