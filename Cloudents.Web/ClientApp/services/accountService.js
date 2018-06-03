import axios from "axios";

export default {
    getAccount:() => axios.get("/Account"),
    setUserName: (data) => axios.post("/Account/userName", {name: data}),
    getUserName: () => axios.get("/Account/userName"),
    setUniversity: (universityId) => axios.post("/Account/university", {universityId}),
    getProfile:(id) => axios.get("/Profile/" + id),
    logout: () => axios.post("/Account/logout"),

}