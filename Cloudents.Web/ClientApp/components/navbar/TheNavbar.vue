<template>
    <!---->
    <div class="sb-menu" :class="offsetTop > 0 ? 'scrolled':''" v-scroll="$_onScroll">
        <v-tabs :value="currentSelection">
            <v-tabs-bar class="cyan" dark>
                <v-tabs-item v-for="tab in verticals" :key="tab.id" :href="tab.id" :id="tab.id" @click="$_updateType(tab.id)" :class="['spitball-bg-'+tab.id,tab.id==currentSelection?'tabs__item--active':'']"
                             class="mr-2 vertical">
                    {{tab.name}}
                </v-tabs-item>
                <v-tabs-slider color="yellow" :class="`spitball-border-${currentSelection}`"></v-tabs-slider>
            </v-tabs-bar>
        </v-tabs>
    </div>
</template>


<script>
    import { mapMutations, mapGetters } from 'vuex'

    export default {
        data() {
            return {
                newVertical: "",
                offsetTop: 0
            }
        },

        props: { $_calcTerm: { type: Function }, verticals: { type: Array },callbackFunc:{type:Function} ,currentSelection:{type:String}},

        methods: {
            $_currentTerm(type) {
                let term = type.includes('food') ? this.$route.meta.foodTerm : type.includes('job') ? this.$route.meta.jobTerm : this.$route.meta.term;
                return term || {};
            },
            $_updateType(result) {
                if (this.$route.name !== "result") {
                    if(this.callbackFunc){
                        this.callbackFunc.call(this,result);
                    }else {
                        this.$router.push({path: '/' + result, query: {q: this.$route.query.q}});
                    }
                }
                else if (this.$route.meta[this.$_calcTerm(result)]) {
                    let query = { q: this.$_currentTerm(result).term };
                    if (this.currentPage === result) query = { ...this.$route.query, ...query };
                    if(this.$route.meta.myClasses&&(result.includes('note')||result.includes('flashcard')))query.course=this.$route.meta.myClasses;
                    this.$router.push({ path: '/' + result, query })
                } else {

                    if (!this.getUniversityName && (result !== 'food' && result !== 'job')) {
                        this.$root.$children[0].$refs.personalize.showDialog=true;
                        return;
                    }
                    this.$router.push({ path: '/' + result });
                }
            },
            $_onScroll: function () {
                this.offsetTop = window.pageYOffset || document.documentElement.scrollTop;
            }


        },
        computed: {
            ...mapGetters(["getUniversityName"])
        }
    };
</script>
<style src="./TheNavbar.less" lang="less"></style>

