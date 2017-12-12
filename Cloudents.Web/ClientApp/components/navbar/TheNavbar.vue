<template>
<div class="sb-menu" :class="offsetTop > 0 ? 'scrolled':''" v-scroll="$_onScroll">
    <v-tabs v-model="currentPage" >
        <v-tabs-bar class="cyan" dark>
               <v-tabs-item v-for="tab in verticals" :key="tab.id" :href="tab.id" :id="tab.id" @click="$_updateType(tab.id)"  :class="['spitball-bg-'+tab.id,tab.id==currentPage?'tabs__item--active':'']"
                            class="mr-2 vertical">
                {{tab.name}}
            </v-tabs-item>
            <v-tabs-slider color="yellow" :class="`spitball-border-${currentPage}`"></v-tabs-slider>
        </v-tabs-bar>
    </v-tabs></div>
</template>


<script>
    import { mapMutations,mapGetters } from 'vuex'

    export default {
        data() {
            return {
                newVertical: "",
                offsetTop: 0
            }
        },

        props:{$_calcTerm:{type:Function},verticals:{type:Array}},

        methods: {
            $_currentTerm(type) {
                let term = type.includes('food') ? this.$route.meta.foodTerm : type.includes('job') ? this.$route.meta.jobTerm : this.$route.meta.term;
                return term || {};
            },
            $_updateType(result) {
                if(this.$route.name!=="result"){
                    this.$router.push({ path: '/' + result,query:{q:this.$route.query.q}});
                }
                if(this.$route.meta[this.$_calcTerm(result)]){
                    let query={q: this.$_currentTerm(result).term };
                    if(this.currentPage===result)query={...this.$route.query,...query};
                this.$router.push({ path: '/' + result,query})}else{

                    if(!this.getUniversityName&&(result!=='food'&&result!=='job')){
                        console.log('no university');
                        let header=this.$root.$children[0].$refs.header;
                        header.isfirst=true;
                        header.$refs.personalize.showDialog=true;
                        return;
                    }
                    this.$router.push({ path: '/' + result});
                }
            },
            $_onScroll: function () {
                console.log('woop');
                this.offsetTop = window.pageYOffset || document.documentElement.scrollTop;
            }


        },
        computed: {
            ...mapGetters(["getUniversityName"]),
            currentPage: { get(){
                return this.$route.meta.pageName?this.$route.meta.pageName:this.$route.path.slice(1);
            }, set(val) { }
            }
        }
    };
</script>
<style src="./TheNavbar.less" lang="less"></style>

