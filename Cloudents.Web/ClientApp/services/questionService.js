import axios from "axios";
import qs from "query-string";

axios.defaults.paramsSerializer = params => qs.stringify(params, {indices: false});
axios.defaults.responseType = "json";

export default {
    getSubjects: () => axios.get("/Question/subject")
}