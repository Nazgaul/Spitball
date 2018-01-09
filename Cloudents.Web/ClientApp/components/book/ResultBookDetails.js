import { sortAndFilterMixin } from '../results/mixins'
import ResultBook from './bookCell.vue';
import TabsSort from "./bookDetailsSort"
const all = "all";
export default {
    mixins: [sortAndFilterMixin, TabsSort],
    created() {
        this.filter = all;
        this.UPDATE_LOADING(true);
        this.$store.dispatch("bookDetails", { pageName: "bookDetails", isbn13: this.params.id, type: "buy" }).then(({ data }) => {
            this.pageData = data;
            this.UPDATE_LOADING(false);
        });
    },
    data() {
        return {
            pageData: '',
            sortVal: "buy",
            showFilters: false
        };
    },
    components: { ResultBook },
    computed: {
        filteredList: function () {
            return !this.pageData.data ? [] : this.filter === all ? this.pageData.data.sort((a, b) => a.price - b.price) : this.pageData.data.filter(item => [].concat(this.filter).includes(item.condition));
        }
    },

    filters: {
        floatDot: function (value,a) {
            return value.toFixed(a)
        }
    },

    methods: {
        $_updateFilter({ val, type }) {
            if(this.filter===all){
                this.filter=[];
            }
            if(!type.target.checked){
                this.filter=this.filter.filter(i=>i!==val);
            }else{
               this.filter.push(val);
            }
            if(!this.filter.length){this.filter=all}
        },
        $_updateSort(val) {
            this.sortVal = val;
            this.$_changeTab(val);
        },
        submitMobile({filters}){
            if(filters&&filters.filter&&filters.filter.length){this.filter=filters.filter}
            else{this.filter=all}
        }
    },
    props: { filterOptions: { type: Array } }
}