<template>
    <div :class="{'pr-2 pl-2': isMobile, 'hide-header': hideHeader}">
    <!-- <v-flex class="information-box" v-show="showInformationBlock">
        <div :class="{'information-box-mobile-wrap': isMobile}">
            <v-icon v-show="isMobile" class="gamburger-icon" @click="setNavigationDrawerState()">sbf-menu</v-icon>
            <span v-if="accountUser" class="information-box-text" :class="{'mobile': isMobile}">{{informationBlockText}}</span>
        </div>
    </v-flex> -->
    <!-- <v-flex class="line verticals static-card-what-is-hw-question">
        <v-layout row >
            <v-tabs hide-slider :dir="this.$vuetify.breakpoint.xsOnly && isRtl ? `ltr` : ''" v-model="currentVertical" :value="currentVertical" :scrollable="true">
                    <v-tab v-for="tab in verticals" :ripple="false" :key="tab.id" :href="tab.id" :id="tab.id"
                                 @click.prevent="$_updateType(tab.id)"
                                 :active-class="'v-tabs__item--active header-tab-active'"
                                 class="mr-4 vertical">
                        {{tab.name}}
                    </v-tab>
                    <v-tabs-slider :color="`color-${currentVertical}`"></v-tabs-slider>
            </v-tabs>
        </v-layout>
    </v-flex> -->

    </div>
</template>

<script>
    import {mapActions, mapGetters, mapMutations} from 'vuex'
    import {verticalsNavbar as verticals}  from "../../services/navigation/vertical-navigation/nav";
    import {LanguageService} from "../../services/language/languageService"

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
            ...mapGetters(['accountUser', 'showMobileFeed']),
            isMobile(){
                return this.$vuetify.breakpoint.mdAndDown
            },
            isInSearchMode(){
                return !!this.$route.query.term;
            },
            isLogedIn(){
                return this.accountUser
            },
            showInformationBlock(){
                if(this.isMobile){
                    return true;
                }else{
                    if(this.isInSearchMode){
                        return false;
                    }else{
                        return true;
                    }
                }
                
            },
            informationBlockText(){
                if(this.showInformationBlock){
                    if(!!this.$route.query && !!this.$route.query.Course){
                        return this.$route.query.Course
                    }else{
                        return LanguageService.getValueByKey("schoolBlock_information_box_latest");
                    }
                }else{
                    return LanguageService.getValueByKey("schoolBlock_information_box_latest");
                }
            },
            hideHeader(){
                if(this.$vuetify.breakpoint.xsOnly){
                    return this.$route.name === "university" || !this.showMobileFeed;
                }else{
                    return false;
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
            ...mapActions(['updateLoginDialogState', 'updateNewQuestionDialogState','toggleShowSchoolBlock']),
            setNavigationDrawerState(){
                 this.toggleShowSchoolBlock()
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
                // this.setCurrentVertical(result);
                
                //if same tab do not do UPDATE_SEARCH_LOADING
                if(this.$route.path !== `/${result}`){
                    this.UPDATE_SEARCH_LOADING(true);
                }
                
                this.$router.push({ path: '/' + result, query: this.$route.query });
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