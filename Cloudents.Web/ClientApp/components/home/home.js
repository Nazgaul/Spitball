//import VueResource from 'vue-resource'
import verticalCollection from './../general/vertical-collection.vue'


export default {
    components: {
        verticalCollection
    },
    data() {
        return {
            msg: "",
            result: "",

            search: () => {
                this.$http.get("api/AI",
                    {
                        params:
                        {
                            sentence: this.msg
                        }
                    }).then((response) => {
                    this.result = response.body;
                });

                console.log("hello");
            }
        }
    }
}
