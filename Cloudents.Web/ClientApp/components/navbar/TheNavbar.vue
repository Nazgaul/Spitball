<template>
<div class="sb-menu">    <v-tabs v-model="currentPage" >
        <v-tabs-bar class="cyan" dark >
               <v-tabs-item v-for="tab in verticals" :key="tab.id" :href="tab.id" :id="tab.id" @click.once="$_updateType(tab.id)"  :class="['bg-'+tab.id,tab.id==currentPage?'tabs__item--active':'']"
                            class="ml-1 mr-1 vertical"
            >
                {{tab.name}}
            </v-tabs-item>
            <v-tabs-slider color="yellow" :class="`bg-${currentPage}`"></v-tabs-slider>
        </v-tabs-bar>
    </v-tabs></div>
</template>


<script>
    import { verticalsNavbar as verticals } from '../data.js';
    import { mapMutations } from 'vuex'

    export default {
        data() {
            return {
                verticals: verticals
            }
        },

        methods: {
            ...mapMutations({ 'changeFlow': 'ADD' }),
            $_currentTerm(type) {
                let term = type.includes('food') ? this.$route.meta.foodTerm : type.includes('job') ? this.$route.meta.jobTerm : this.$route.meta.term;
                return term || {};
            },
            $_updateType(result) {
                this.changeFlow({ result });
                this.$router.push({ path: '/' + result, query: { q: this.$_currentTerm(result).term } })
            }
        },
        computed: {
            currentPage: { get(){
                return this.$route.path.slice(1)},set(val){} }
        }
    };
</script>
<style src="./TheNavbar.less" lang="less"></style>

