<template>
    <div>
    <v-flex class="line verticals static-card-what-is-hw-question">
        <v-layout row >
            <v-tabs v-model="currentVertical" :value="currentVertical" :scrollable="true">
                    <v-tab v-for="tab in verticals" :ripple="false" :key="tab.id" :href="tab.id" :id="tab.id"
                                 @click.prevent="$_updateType(tab.id)"
                                 :class="['spitball-text-'+tab.id,tab.id===currentVertical?'v-tabs__item--active header-tab-active':'']"
                                 class="mr-3 vertical">
                        {{tab.name}}
                    </v-tab>
                    <v-tabs-slider :color="`color-${currentVertical}`"></v-tabs-slider>
            </v-tabs>
        </v-layout>
    </v-flex>

    </div>
</template>

<script>
    import {mapActions, mapGetters, mapMutations} from 'vuex'
    import {verticalsNavbar as verticals}  from "../../services/navigation/vertical-navigation/nav";

    export default {
        name: "verticals-tabs",

        props: {currentSelection: {}},
        data() {
            return {
                verticals,
                currentVertical: this.currentSelection,
                isRtl: global.isRtl,
                supressVerticalDesign:{
                    wallet: true,
                    profile: true,
                    conversations: true
                }
            }
        },
        computed: {
            ...mapGetters(['getVerticalData', 'accountUser']),
            isLogedIn(){
                return this.accountUser
            }
        },
        watch: {
            currentSelection(val) {
                this.currentVertical = val;
            },
            '$route'(val){
                setTimeout(()=>{
                    if(this.supressVerticalDesign[val.name]){
                        this.cleanVerticalDesign();
                    }else{
                        this.restoreVerticalDesign();
                    }
                }, 300)
            }
        },
        methods: {
            ...mapMutations(['UPDATE_SEARCH_LOADING']),
            ...mapActions(['setCurrentVertical', 'updateLoginDialogState', 'updateUserProfileData', 'updateNewQuestionDialogState']),
            cleanVerticalDesign(){
                //remove selected tab design
                let elmActiveParent = document.getElementsByClassName('header-tab-active')[0];
                elmActiveParent.firstChild.classList.remove('v-tabs__item--active');

                //slider remove
                let elmContainer = elmActiveParent.parentElement;
                let sliderContainer = elmContainer.firstChild;
                sliderContainer.firstChild.classList.remove('v-tabs__slider');
            },
            restoreVerticalDesign(){
                //remove selected tab design
                let elmActiveParent = document.getElementsByClassName('header-tab-active')[0];
                elmActiveParent.firstChild.classList.add('v-tabs__item--active');

                //slider remove
                let elmContainer = elmActiveParent.parentElement;
                let sliderContainer = elmContainer.firstChild;
                sliderContainer.firstChild.classList.add('v-tabs__slider');
            },
            $_updateType(result) {
                this.currentVertical = result;
                this.$ga.event("Vertical_Tab", result);
                let tabs = this.$el.querySelector('.v-tabs__wrapper');
                let currentItem = this.$el.querySelector(`#${result}`);
                if (currentItem) {
                    tabs.scrollLeft = currentItem.offsetLeft - (tabs.clientWidth / 2);
                }
                this.setCurrentVertical(result);
                let query = {};
                if (this.$route.query.hasOwnProperty("promo")) {
                    query = {promo: this.$route.query.promo}
                }
                let {text = "", course} = this.getVerticalData(result);
                if ((result == 'flashcard' && this.$route.path.includes('note') || result == 'note' && this.$route.path.includes('flashcard')) && this.$route.query.course) {
                    course = this.$route.query.course;
                }
                //if same tab do not do UPDATE_SEARCH_LOADING
                if(this.$route.path !== `/${result}`){
                    this.UPDATE_SEARCH_LOADING(true);
                }else{
                    query = this.$route.query;
                }
                this.$router.push({path: '/' + result, query: query});
                // this.$router.push({path: '/' + result, query: {...query, q: text, course}});
            },
        },
        mounted(){
            if(this.supressVerticalDesign[this.$route.name]){
                setTimeout(()=>{
                    this.cleanVerticalDesign();
                }, 300)
            }
        }
    }
</script>

<style src="./verticalTabs.less" lang="less"></style>