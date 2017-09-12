import Vue from 'vue';
export default class AppComponent extends Vue {
    methods() {
        return {
            changeSection(item: any) {
                console.log("change" + item);
            }
        }
    }
}