import VueResource from 'vue-resource' 
export default {
    data() {
        return {
            msg: "hello vue.js!",
            search: () => {

                this.$http.post("/ai",
                    {
                       sentence: this.msg
                    }).then((response) => {
                    console.log(response.body);
                });

                console.log("hello");
            }
        }
    }
}
