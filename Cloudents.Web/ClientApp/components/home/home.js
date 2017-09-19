import verticalCollection1 from './../general/vertical-collection.vue'
import search from "./../api/search"

export default {
    components: {
        'vertical-collection':verticalCollection1
    },
    data() {
        var prefix = "";
        return {           
            msg: "",
            placeholder: "",

            changeSection: (vertical) => {
                this.placeholder = vertical.placeholder;
                prefix = vertical.prefix;
            },
            search: () => {
                search.getDocument(null,
                    (result) => {
                        console.log(result);
                    });
                //this.$http.get("api/AI",
                //    {
                //        params:
                //        {
                //            sentence: prefix + " " + this.msg
                //        }
                //    }).then((response) => {
                //        this.result = response.body;
                //        this.$store.commit("ADD", response.body);
                //    });
            }
        }
    }
}
