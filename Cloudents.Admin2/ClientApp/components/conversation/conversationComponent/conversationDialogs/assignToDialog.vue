<template>
    <div class="dialog-wrap">
        <v-card>

            <v-card-title primary-title>
                <div class="title">Assign To</div>
            </v-card-title>

            <v-card-actions>
                <v-spacer></v-spacer>
                <v-btn icon @click="show = !show">
                    <v-icon>{{ show ? 'keyboard_arrow_down' : 'keyboard_arrow_up' }}</v-icon>
                </v-btn>
            </v-card-actions>

            <v-slide-y-transition v-for="(assign, index) in assignToFilters" :key="index">
                <v-card-text v-show="show">
                    <v-btn round @click="chooseFilter(assign)">{{assign}}</v-btn>
                </v-card-text>
            </v-slide-y-transition>

        </v-card>            
    </div>
</template>
<script>
export default {
    props: {
        assignToFilters: {},
        setAssignFilter: {
            type: Function
        }
    },
    data() {
        return {
            show: false
        }
    },
    methods: {
        chooseFilter(id) {
            this.handleFilter('assignTo', id)
        }
    }
}
</script>
<style lang="scss">
    .dialog-wrap {
        background: #fff;
        .v-btn--active {
            background: blue;
        }
    }
</style>