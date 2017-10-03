import Vue from 'vue';
export default {
    interpetPromise(prefix, str) {
       return  Vue.http.get("api/AI",
            {
                params:
                {
                    sentence: prefix + " " + str
                }
            })
    }
};