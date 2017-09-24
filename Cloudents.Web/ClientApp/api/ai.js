import Vue from 'vue';
export default {
    interpet(prefix, str, cb, errorCb) {
        Vue.http.get("api/AI",
            {
                params:
                {
                    sentence: prefix + " " + str
                }
            }).then((response) => {
            cb(response.body);
            //this.result = response.body;
            //this.$store.commit("ADD", response.body);
        });
    }
};