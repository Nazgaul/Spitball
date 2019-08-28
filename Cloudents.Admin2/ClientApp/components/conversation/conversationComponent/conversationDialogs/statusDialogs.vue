<template>
    <div class="dialog-wrap">
        <div class="pa-4">
            <h3>Filter Results</h3>
        </div>
        <div class="pa-4">
            <v-combobox
                v-model="status"
                :items="items"
                class="mb-4 top-card-select"
                height="40px"
                hide-details
                box
                menu-props="lazy"
                round
                outline
                :label="currentStatus.group || 'Status 1'"
                @change="chooseStatus"
              ></v-combobox>
              <v-combobox
                v-model="subStatus"
                :items="statusItems"
                class="top-card-select"
                height="40px"
                hide-details
                box
                menu-props="lazy"
                round
                outline
                :label="currentStatus.name || 'Status Details'"
                @change="handleFilter"
              ></v-combobox>
        </div>
    </div>
</template>
<script>
export default {
    props: {
        statusFilters: {
            type: Object
        },
        currentStatus: {
            type: Object,
            default: {}
        },
        setStatusFilter: {
            type: Function
        },
        changeStatus: {
            type: Function
        },
        isSet: {
            type: Boolean
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
        handleFilter(val) {            
            if(!val) return;

            let item = this.statusFilters[this.status].filter(el => el.name === this.subStatus)[0];

            if(this.isSet) {
                this.changeStatus(item);
                return;
            }

            this.setStatusFilter(item);
        }
    },
    created() {
        this.items = Object.keys(this.statusFilters).map(element => element);
    }
}
</script>
<style lang="less">
    .dialog-wrap {
        background: #fff;
    }
</style>