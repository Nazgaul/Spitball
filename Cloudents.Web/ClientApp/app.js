import { mapGetters } from 'vuex'
import App from './app.vue'
export function createApp () {
    const app = new Vue({
        // the root instance simply renders the App component.
        computed: {
            ...mapGetters(['loading']),
        },
        render: h => h(App)
    });
    return { app }
}