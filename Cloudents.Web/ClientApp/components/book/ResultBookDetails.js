import { sortAndFilterMixin } from '../results/mixins'
import ResultBook from './bookCell.vue';
import TabsSort from "./bookDetailsSort"
export default {
    mixins: [sortAndFilterMixin, TabsSort],
    created() {
        this.filter = 'all';
        this.UPDATE_LOADING(true);
        this.$store.dispatch("bookDetails", { pageName: "bookDetails", isbn13: this.params.id,type:"buy" }).then(({data}) => {
            this.pageData = data;
            this.UPDATE_LOADING(false);
        });
    },
    data() {
        return {
            pageData: '',
            sortVal: "buy",
            showFilters:false
        };
    },
    components: {  ResultBook },
    computed: {
        filteredList: function () {
            return !this.pageData.data?[]:this.filter === 'all' ? this.pageData.data.sort((a, b) => a.price - b.price) : this.pageData.data.filter(item => item.condition === this.filter);
        }
    },

    methods: {
    $_updateFilter({val,type}){
        this.filter=(type.target.checked||val==='all')?val:'all';
    },
        $_updateSort(val){
            this.sortVal=val;
            this.$_changeTab(val);
        }
    },
    props:{filterOptions:{type:Array}}
}