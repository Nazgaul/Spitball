import VueResource from 'vue-resource' 
export default {
    data() {
        return {
            msg: "hello vue.js!",
            search: () => {

                this.$http.get("/home/GetMeAjaxBicth").then((response) => {
                    console.log(response.body);
                });

                console.log("hello");
            }
        }
    }
}
