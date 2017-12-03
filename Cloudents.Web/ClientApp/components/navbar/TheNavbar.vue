<template>
<div class="sb-menu">
    <v-dialog width="500" v-model="changeTerm">
        <form @submit.prevent="$_changeTerm">
            <v-text-field  light solo class="search-b" :placeholder="`Please enter the search term for ${this.newVertical}`" v-model="newTerm" ></v-text-field>
            <v-btn :disabled="!newTerm" @click="$_changeTerm">Submit</v-btn>
        </form>
    </v-dialog>
    <v-tabs v-model="currentPage" >
        <v-tabs-bar class="cyan" dark >
               <v-tabs-item v-for="tab in verticals" :key="tab.id" :href="tab.id" :id="tab.id" @click="$_updateType(tab.id)"  :class="['bg-'+tab.id,tab.id==currentPage?'tabs__item--active':'']"
                            class="ml-1 mr-1 vertical">
                {{tab.name}}
            </v-tabs-item>
            <v-tabs-slider color="yellow" :class="`border-${currentPage}`"></v-tabs-slider>
        </v-tabs-bar>
    </v-tabs></div>
</template>


<script>
    import { verticalsNavbar as verticals } from '../data.js';
    import { mapMutations } from 'vuex'

    export default {
        data() {
            return {
                verticals: verticals,
                changeTerm:false,
                newTerm:"",
                newVertical:""
            }
        },

        props:{$_calcTerm:{type:Function}},

        methods: {
            ...mapMutations({ 'changeFlow': 'ADD' }),
            $_currentTerm(type) {
                let term = type.includes('food') ? this.$route.meta.foodTerm : type.includes('job') ? this.$route.meta.jobTerm : this.$route.meta.term;
                return term || {};
            },
            $_updateType(result) {
                if(this.$route.meta[this.$_calcTerm(result)]){
                    this.changeFlow({ result });
                this.$router.push({ path: '/' + result, query: {...this.$route.query,q: this.$_currentTerm(result).term } })}else{
                    this.changeTerm=true;
                    this.$nextTick(()=>{
                        this.$el.querySelector(`[href=${this.currentPage}]`).click();
                        this.newVertical=result;
                    });
                }
            },
            $_changeTerm(){
                let term=this.newTerm;
                this.changeFlow({ result:this.newVertical });
                this.$route.meta[this.$_calcTerm(this.newVertical)] = {term: term, luisTerm: term};
                this.$nextTick(()=>{
                    this.changeTerm=false;
                    this.newTerm="";
                    this.$router.push({ path: '/' + this.newVertical, query: {q: term } });
                })
            }
        },
        computed: {
            currentPage: { get(){
                return this.$route.path.slice(1)},set(val){} }
        }
    };
</script>
<style src="./TheNavbar.less" lang="less"></style>

