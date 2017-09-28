<template>
    <div class="content">
        <h1 ><span v-if="isEmpty" v-html="page.title"></span>
        <span v-else v-html="page.emptyState"></span>{{dynamicHeader}}</h1>
        <slot name="options">
            <radio-list :values="page.filter" :changeCallback="changeFilter" model="filter"></radio-list>
            sort:<div v-for="s in page.sort">
                     <router-link :to="{query: computedSort(s)}" append>{{s}}</router-link>
            </div>
            <radio-list :values="subFilters" :changeCallback="changeSubFilter" model="subFilter"></radio-list>
        </slot>
        <slot></slot>
       <slot name="emptyState" v-show="isEmpty"></slot>
    </div>
</template>
<script>
    import { page } from './../../data' 
    import RadioList from './../../helpers/radioList.vue'
    export default {
        data() {
            return {
                page: page[this.$route.name],
                filter: '',
                currentQuery: this.$route.query
            }
        }, computed: {
            dynamicHeader: function () { return this.$store.getters.pageTitle },
            isEmpty: function () { return this.$store.getters.isEmpty },
            pageData: function () { return this.$store.getters.pageContent },
            subFilters: function () {
                var subFilters = []
                var list = this.pageData[this.filter];
                if (list) {
                    for (var i = 0; i < list.length; i++) {
                        var item = list[i];
                        subFilters.push({ id: item, name: item });
                    }
                }
                return subFilters;
            }
        },
        components: { RadioList },
        methods: {
            computedSort(sort) { return { ... this.currentQuery,sort}} ,
            changeFilter(e) {
                this.filter = e.target.value;
                if (!this.subFilters.length) {
                    this.$router.push({ query: { ... this.currentQuery,filter:this.filter} });
                }
            },
            changeSubFilter(e) {
                let sub = {};
                sub[this.filter] = e.target.value
                this.$router.push({ query: { ... this.currentQuery,...sub} });
                console.log('change sub filter');          
            }
        }
    }
</script>