import axios from "axios";

export default {
    getAccount:() => axios.post("/api/Account"),
    setUserName: (data) => axios.post("/Account/userName", {name: data}),
    getAccountNum: () => axios.post("/Account/password"),
    setUniversity: (universityId) => axios.post("/api/Account/university", {universityId}),
}