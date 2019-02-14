<template>
    <div>
    <v-flex class="information-box" v-show="showInformationBlock">
        <v-icon v-show="$vuetify.breakpoint.xsOnly" class="gamburger-icon" @click="setNavigationDrawerState()">sbf-menu</v-icon>
        <span class="information-box-text">{{informationBlockText}}</span>    
    </v-flex>    
    <v-flex class="line verticals static-card-what-is-hw-question">
        <v-layout row >
            <v-tabs v-model="currentVertical" :value="currentVertical" :scrollable="true">
                    <v-tab v-for="tab in verticals" :ripple="false" :key="tab.id" :href="tab.id" :id="tab.id"
                                 @click.prevent="$_updateType(tab.id)"
                                 :active-class="'v-tabs__item--active header-tab-active'"
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
                },
            }
        },
        computed: {
            ...mapGetters(['getVerticalData', 'accountUser']),
            isMobile(){
                return this.$vuetify.breakpoint.xsOnly
            },
            isInSearchMode(){
                return !!this.$route.query.term;
            },
            isLogedIn(){
                return this.accountUser
            },
            showInformationBlock(){
                if(this.isMobile){
                    return !!this.$route.query && !!this.$route.query.Course;
                }else{
                    if(this.isInSearchMode){
                        return false;
                    }else{
                        return !!this.$route.query && !!this.$route.query.Course;
                    }
                }
                
            },
            informationBlockText(){
                if(this.showInformationBlock){
                    return this.$route.query.Course
                }else{
                    return '';
                }
            }
        },
        watch: {
            currentSelection(val) {
                this.currentVertical = val;
            },
            '$route'(val){
            }
        },
        methods: {
            ...mapMutations(['UPDATE_SEARCH_LOADING']),
            ...mapActions(['setCurrentVertical', 'updateLoginDialogState', 'updateUserProfileData', 'updateNewQuestionDialogState','toggleShowSchoolBlock']),
            setNavigationDrawerState(){
                if(this.$vuetify.breakpoint.xsOnly){
                   this.toggleShowSchoolBlock(false);
                   setTimeout(()=>{
                        this.toggleShowSchoolBlock(true);
                   }, 200)
                }else{
                    this.toggleShowSchoolBlock()
                }
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
                // let tabs = this.$el.querySelector('.v-tabs__wrapper');
                // let currentItem = this.$el.querySelector(`#${result}`);
                // if (currentItem) {
                //     tabs.scrollLeft = currentItem.offsetLeft - (tabs.clientWidth / 2);
                // }
                this.setCurrentVertical(result);
                
                //if same tab do not do UPDATE_SEARCH_LOADING
                if(this.$route.path !== `/${result}`){
                    this.UPDATE_SEARCH_LOADING(true);
                }
                
                this.$router.push({path: '/' + result, query: this.$route.query});
            },
        },
        mounted(){
            if(this.supressVerticalDesign[this.$route.name]){
                // setTimeout(()=>{
                //     this.cleanVerticalDesign();
                // }, 300)
            }
        }
    }
</script>

<style src="./verticalTabs.less" lang="less"></style>