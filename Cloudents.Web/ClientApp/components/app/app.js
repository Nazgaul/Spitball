import * as types from '../../store/mutation-types'

export default {
    name: "app",
    data() {
        this.$store.subscribe((mutation, state) => {
            if (mutation.type === types.ADD) {
                this.$router.push(state.state.node.model.name);
            }
        });
        return {
            //msg: "hello vue.js! Ram is the king",
            //search: () => {
            //    console.log("hello");
            //}
        }
    }
}
