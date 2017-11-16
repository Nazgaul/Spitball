import { sortAndFilterMixin } from './mixins'
//const ResultBook = () => import('./ResultBook.vue');
import ResultBook from './ResultBook.vue';
export default {
    mixins: [sortAndFilterMixin],
    created() {
        this.filter = 'all';
        this.UPDATE_LOADING(true);
        this.$store.dispatch("bookDetails", { pageName: this.name, params: this.params }).then((res) => {
            this.pageData = res;
            this.UPDATE_LOADING(false);
        });
    },
    data() {
        return { pageData: '' };
    },
    components: {  ResultBook },
    computed: {
        filteredList: function () {
            return this.filter === 'all' ? this.pageData.data.sort((a, b) => a.price - b.price) : this.pageData.data.filter(item => item.condition === this.filter);
        }
    }
    // mixins: [detailsMixin]
}