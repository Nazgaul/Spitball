import DialogToolbar from '../dialog-toolbar/DialogToolbar.vue'
import {mapActions} from 'vuex'

export default {
    model: {
        prop: "value",
        event: "input"
    },
    data() {
        return {
            filters: {
                source: [],
                course: [],
                jobType: [],
                filter: []
            },
            sort: this.sortVal ? this.sortVal : (this.sortOptions && this.sortOptions.length) ? this.sortOptions[0].id : ""
        }
    },
    components: {DialogToolbar},
    props: {
        value: {type: Boolean},
        sortOptions: {type: Array, default: () => []},
        filterOptions: {type: Array, default: () => []},
        filterVal: {type: Array, default: () => []},
        sortVal: {}
    },
    methods: {
        ...mapActions(['setFilteredCourses']),

        initFilters(val = []) {
            this.filters = {source: [], course: [], jobType: [], filter: []};
            [].concat(val).forEach(({key, value}) => {
                this.filters[key] = this.filters[key].concat(value);
            });
        },
        applyFilters() {
            //if filters have courses and courses has been changed save the changes
            let courseBefore = this.filterVal.filter(i => i.key === 'course').map(i => i.value);
            let courseNow = this.filters.course;
            let mergedCourseSet = new Set([...courseNow, ...courseBefore]);
            let isNotEqual = courseNow.length < mergedCourseSet.size;
            console.log(isNotEqual);
            if ((this.filterOptions.find(i => i.modelId === 'course')) &&
                (courseBefore.length !== courseNow.length || isNotEqual)) {
                this.setFilteredCourses(this.filters.course);
            }
            if (this.filters.filter.includes('inPerson')) {
                this.sort = "price"
            }
            let query = {};
            Object.keys(this.filters).forEach(key => {
                let value = this.filters[key];
                if (value.length) query[key] = value;
            });
            if (this.sort) query.sort = this.sort;
            if (this.$route.query.q) query.q = this.$route.query.q;
            this.$router.push({query});
            this.$emit('input', false);
        },

        resetFilters() {
            if (this.filterOptions.find(i => i.modelId === 'course')) {
                this.setFilteredCourses([]);
            }
            this.initFilters();
            if (this.sortOptions.length) {
                this.sort = this.sortOptions[0].id;
            }
            this.$router.push({query: {q: this.$route.query.q}});
            this.applyFilters();
            this.$emit('input', false);
        },

        $_backAction() {
            this.sort = this.sortVal;
            this.initFilters(this.filterVal);
            this.$emit('input', false)
        }
    },
    watch: {
        filterVal(val) {
            this.initFilters(val);
            this.sort = this.sortVal ? this.sortVal : (this.sortOptions && this.sortOptions.length) ? this.sortOptions[0].id : "";
        }
    },

    created() {
        this.initFilters(this.filterVal);
    }
}