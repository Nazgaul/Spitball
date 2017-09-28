import verticalCollection1 from './../general/vertical-collection.vue';
import search from "./../../api/ai";

export default {
    components: {
        'vertical-collection': verticalCollection1
    },
    data() {
        var prefix = "";
        return {
            msg: this.$route.meta.userText,
            placeholder: "",

            changeSection: (vertical) => {
                this.placeholder = vertical.placeholder;
                prefix = vertical.prefix;
            },
            search: () => {
                this.$store.dispatch('updateSearchText', this.msg);
                search.interpet(prefix,
                    this.msg,
                    (response) => {
                        this.result = response;
                        this.$store.commit("ADD", response);
                    });
            }
        };
    }
};
