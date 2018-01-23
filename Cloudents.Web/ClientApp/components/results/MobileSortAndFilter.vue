<template>
    <v-dialog v-model="value" fullscreen content-class="white filter-dialog">
        <dialog-toolbar height="48" toolbarTitle="Filter & Sort" :backAction="$_backAction">
            <v-btn slot="rightElement" flat class="clear-btn" @click="$_resetFilters">Clear all</v-btn>
        </dialog-toolbar>
 
        <v-btn class="apply elevation-0" fixed color="color-blue" @click="$_applayFilters">Apply Filters</v-btn>

        <sort-and-filter :sortOptions="sortOptions" :sortCallback="$_updateSortMobile" :sortVal="sortVal"
                         :filterOptions="filterOptions"
                         :filterCallback="$_updateFilterMobile" :filterVal="selectedFilters">
            <div slot="headerTitle" slot-scope="props"  @click.stop.prevent="">
                <span>{{props.title}}</span>
            </div>
            <!--</template>-->
            <template slot="courseTitlePrefix">
                <slot name="courseTitlePrefix"></slot>
            </template>
            <template slot="courseMobileExtraState">
                <slot name="mobileExtra"></slot>
            </template>
            <template slot="courseEmptyState">
                <slot name="courseEmptyState"></slot>
            </template>
        </sort-and-filter>
    </v-dialog>
</template>

<script>
    import SortAndFilter from './SortAndFilter.vue'
    import DialogToolbar from '../dialog-toolbar/DialogToolbar.vue'

    import {mapActions} from 'vuex'
    export default {
        model: {
            prop: "value",
            event: "input"
        },
        data() {
            return { filters: {}, sort: "", selectedFilters: [] }
        },
        components: { SortAndFilter, DialogToolbar },
        props: { value: { type: Boolean }, sortOptions: {}, filterOptions: {}, filterVal: {}, sortVal: {}, submitCallback: { Function } },
        methods: {
            ...mapActions(['setFileredCourses']),
            $_applayFilters() {
                if (this.submitCallback) {
                    this.submitCallback({ filters: this.filters, sort: this.sort });
                } else {
                    if (this.$route.path.includes('note') || this.$route.path.includes('flashcard'))
                        this.setFileredCourses(this.filters.course);
                    this.$router.push({query: {q: this.$route.query.q, sort: this.sort, ...this.filters}});
                }
                this.$emit('input', false);
            },
            $_resetFilters() {
               if(this.submitCallback){this.submitCallback({})}else {
                   if (this.$route.path.includes('note') || this.$route.path.includes('flashcard'))
                       this.setFileredCourses([]);
                   this.$router.push({query: {q: this.$route.query.q}});
               }
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
                }
                this.filters[id] = [...new Set(listo)];
                if (val === 'inPerson' && type) this.sort = "price";
            },
            $_backAction() {
                this.$emit('input', false)
            }
        },
        watch: {
            filterVal(val) {
                Object.entries(this.filters).forEach(([key, currentVal]) => {
                    this.filters[key] = currentVal.concat(val).filter((val, index, self) => self.indexOf(val) !== index);
                });
                this.selectedFilters = val;
            }
        },
        created() {
            this.selectedFilters = this.filterVal;
        }
    }
</script>