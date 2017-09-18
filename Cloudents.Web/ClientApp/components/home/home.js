//import VueResource from 'vue-resource'
import verticalCollection from './../general/vertical-collection.vue'


export default {
    components: {
        verticalCollection
    },
    data() {
        
        return {

            
            msg: "",
            placeholder: "",

            changeSection: (vertical) => {
                this.placeholder = vertical.placeholder;
                //console.log(vertical);
            },
            search: () => {
                this.$http.get("api/AI",
                    {
                        params:
                        {
                            sentence: this.msg
                        }
                    }).then((response) => {
                        this.result = response.body;
                        this.$store.commit("ADD", response.body);
                    });
            }
        }
    }
}
