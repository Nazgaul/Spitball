﻿import * as types from '../../store/mutation-types';

export default {
    name: "app",
    data() {
        this.$store.subscribe((mutation, state) => {
            if (mutation.type === types.ADD) {
                this.$router.push({
                    name: state.Flow.node.model.name
                });
            }
        });
        return {}
    }
};
