import Vue from 'vue';
export default {
    interpetPromise(text) {
       return  Vue.http.get("api/AI",
            {
                params:
                {
                    sentence: text
                }
            })
    }
};