<template>
    <v-dialog v-model="value" fullscreen content-class="white filter-dialog">
        <v-toolbar fixed class="elevation-1" height="48">
            <v-btn icon class="back" @click="$emit('input',false)">
                <i class="sbf icon sbf-arrow-button"></i></v-btn>
            <v-toolbar-title class="toolbar-title">Filter & Sort</v-toolbar-title>
            <v-spacer></v-spacer>
            <v-btn flat class="clear-btn" @click="$_resetFilters">Clear all</v-btn>
        </v-toolbar>

        <v-btn class="apply elevation-0" fixed color="color-blue" @click="$_applayFilters">Apply Filters</v-btn>

        <sort-and-filter :sortOptions="sortOptions" :sortCallback="$_updateSortMobile" :sortVal="sortVal"
                         :filterOptions="filterOptions"
                         :filterCallback="$_updateFilterMobile" :filterVal="selectedFilters">
            
            <!--</template>-->
            <template slot="courseTitlePrefix">
                <slot name="courseTitlePrefix"></slot>
            </template>
            <template slot="courseMobileExtraState">
                <slot name="courseExtraState"></slot>
                <slot name="courseEmptyState"></slot>
            </template>
        </sort-and-filter>
    </v-dialog>
</template>

<script>
    import SortAndFilter from './SortAndFilter.vue'
    export default {
        model: {
            prop: "value",
            event: "input"
        },
        data() {
            return { filters: {}, sort: "",selectedFilters:[]}
        },
        components: { SortAndFilter },
        props: { value: { type: Boolean }, sortOptions: {}, filterOptions: {}, filterVal: {}, sortVal: {} },
        methods: {
            $_applayFilters() {
                if (this.$route.path.includes('note') || this.$route.path.includes('flashcard'))
                    this.$route.meta.myClasses=this.filters.course;
                this.$router.push({ query: { q: this.$route.query.q, sort: this.sort, ...this.filters } });
                this.$emit('input', false);
            },
            $_resetFilters() {
                if (this.$route.path.includes('note') || this.$route.path.includes('flashcard'))
                    this.$route.meta.myClasses = [];
                this.$router.push({ query: { q: this.$route.query.q } });
                this.$emit('input', false);
            },
            $_updateSortMobile(val) {
                this.sort = val;
            },
            $_updateFilterMobile({ id, val, type }) {
                let currentFilter = this.filters[id] || [];
                let listo = [val, ...currentFilter];
                if (!type.target.checked) {
                    listo = currentFilter.filter(i => i.toString() !== val.toString());
                    // this.selectedFilters=this.selectedFilters.filter(i => i!==val);
                }else{
                    // this.selectedFilters.push(val);
                }
                // if (id === 'course') {
                //     this.$route.meta.myClasses = listo;
                // }
                this.filters[id] = [...new Set(listo)];
                if (val === 'inPerson' && type) this.sort = "price";
            }
        },
        watch:{
            filterVal(val){
                // this.filters=val;
                this.selectedFilters=val;
            }
        },
        created(){
            this.selectedFilters=this.filterVal;
        }
    }
</script>