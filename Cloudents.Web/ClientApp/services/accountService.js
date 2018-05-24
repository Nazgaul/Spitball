import axios from "axios";

export default {
    getAccount:() => axios.get("/Account"),
    setUserName: (data) => axios.post("/Account/userName", {name: data}),
    getAccountNum: () => axios.post("/Account/password"),
    setUniversity: (universityId) => axios.post("/Account/university", {universityId}),
}