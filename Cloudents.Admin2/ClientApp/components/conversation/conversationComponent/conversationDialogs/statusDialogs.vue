<template>
    <div class="dialog-wrap">
        <div>
            <v-card v-for="(group, key, index) in statusFilters" :key="index">
                <v-card-title primary-title>
                    <div class="title">{{key}}</div>
                </v-card-title>

                <v-card-actions>
                  <v-spacer></v-spacer>
                  <v-btn icon @click="toggleStatusCard(index)">
                    <v-icon>{{ show && currentDialogShow === index ? 'keyboard_arrow_down' : 'keyboard_arrow_up' }}</v-icon>
                  </v-btn>
                </v-card-actions>
                <v-slide-y-transition v-for="(items, id) in group" :key="id">
                    <v-card-text v-show="show && currentDialogShow === index">
                        <v-btn round @click="chooseFilter(items.id, items.name)">{{items.name}}</v-btn>
                    </v-card-text>
                </v-slide-y-transition>
            </v-card>
        </div>
    </div>
</template>
<script>
export default {
    props: {
        statusFilters: {
            type: Object
        },
        setStatusFilter: {
            type: Function
        }
    },
    data() {
        return {
            show: false,
            currentFilter: '',
        }
    },
    methods: {
        toggleStatusCard(index) {
            this.show = !this.show;
            this.currentDialogShow = index;
        },
        chooseFilter(id, name) {
            this.setStatusFilter(id, name);
        },
    }
}
</script>
<style lang="scss">
    .dialog-wrap {
        background: #fff;
    }
</style>