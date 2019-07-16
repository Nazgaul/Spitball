<template>
    <div class="dialog-wrap pa-3">
        <div>
            <v-select
                v-model="status"
                :items="items"
                class="mb-2 top-card-select"
                height="40px"
                hide-details
                dense
                box
                menu-props="lazy"
                round
                outline
                label="Status"
                @change="chooseStatus"
              ></v-select>
              <v-select
                v-model="subStatus"
                :items="statusItems"
                class="top-card-select"
                height="40px"
                hide-details
                dense
                box
                menu-props="lazy"
                round
                outline
                label="Default"
                @change="handleFilter"
              ></v-select>
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
            status: '',
            subStatus: '',
            items: [],
            statusItems: []
        }
    },
    methods: {
        chooseStatus() {
            this.statusItems = this.statusFilters[this.status].map(item => item.name);
        },
        handleFilter() {
            let item = this.statusFilters[this.status].filter(el => el.name === this.subStatus)[0];
            this.setStatusFilter(item.id, item.name);
        }
    },
    created() {
        this.items = Object.keys(this.statusFilters).map(element => element);
    }
}
</script>
<style lang="scss">
    .dialog-wrap {
        background: #fff;
    }
</style>