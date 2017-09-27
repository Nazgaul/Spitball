import * as types from '../../store/mutation-types';
import temp from '../../api/search';

export default {
    name: "app",
    data() {
        this.$store.subscribe((mutation, state) => {
            if (mutation.type === types.ADD) {
                this.$router.push({ name: state.state.node.model.name });
            }
        });
        temp.getShortAnswer(null,
            (response) => {
                console.log(response);
            }
        );
        return {
            };
    }
};
