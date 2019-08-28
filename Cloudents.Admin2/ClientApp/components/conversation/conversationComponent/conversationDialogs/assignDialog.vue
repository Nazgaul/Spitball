<template>
    <div class="dialog-wrap">
        <div class="pa-4">
            <h3>Assign Results</h3>
        </div>
        <div class="pa-4">
            <v-combobox
                v-model="assign"
                :items="assignFilters"
                class="mb-4 top-card-select"
                height="40px"
                hide-details
                box
                menu-props="lazy"
                round
                outline
                :label="currentAssigned || 'Assigned'"
                @change="chooseAssign"
            ></v-combobox>
        </div>
    </div>
</template>
<script>
export default {
    props: {
        assignFilters: {},
        currentAssigned: {
            type: String
        },
        setAssigned: {
            type: Function
        }
    },
    data() {
        return {
            assign: ''
        }
    },
    methods: {
        chooseAssign() {
            if(!this.assign) return;

            this.setAssigned(this.assign)           
        }
    }
}
</script>
<style lang="less">
    .dialog-wrap {
        background: #fff;
        .v-btn--active {
            background: blue;
        }
    }
</style>