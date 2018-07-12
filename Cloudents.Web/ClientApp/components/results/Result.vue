﻿
<template>
    <general-page :breakPointSideBar="$vuetify.breakpoint.lgAndUp" :name="name">
        <signup-banner  slot="signupBanner" v-scroll="hideBanner" v-if="!accountUser && showRegistrationBanner && isBannerVisible"></signup-banner>
        <div slot="main" >
         <div class="d-flex mobile-filter" >
                <router-link v-if="$route.path.slice(1)==='ask' " class="ask-question-mob hidden-md-and-up"  :to="{path:'/newquestion/'}">Ask Your Question</router-link>
                <v-btn icon :color="`color-${name}`" flat slot="mobileFilter" @click="showFilters=true" class="text-xs-right hidden-sm-and-up" v-if="filterCondition">
                    <v-icon>sbf-filter</v-icon>
                    <div :class="'counter color-'+$route.path.slice(1)" v-if="this.filterSelection.length">{{this.filterSelection.length}}</div>
                </v-btn>
            </div>
            <div v-if="filterSelection.length" class="pb-3 hidden-sm-and-down">
                <template v-for="item in filterSelection">
                    <v-chip label class="filter-chip elevation-1">
                        {{$_showSelectedFilter(item) | capitalize}}
                        <v-icon right @click="$_removeFilter(item)">sbf-close</v-icon>
                    </v-chip>
                </template>
            </div>
            <div class="results-section" :class="{'loading-skeleton': showSkelaton}">
                <scroll-list v-if="items.length" @scroll="value => {items=items.concat(value) }" :url="pageData.nextPage" :vertical="pageData.vertical">
                    <v-container class="pa-0 ma-0 results-wrapper">
                        <v-layout column>
                            <v-flex class="promo-cell mb-4 elevation-1" order-xs1 v-if="showPromo">
                                <button class="close-btn pa-2" @click="showPromo=false">
                                    <v-icon right>sbf-close</v-icon>
                                </button>
                                <div>
                                    <div class="promo-title">Simplify School with Spitball</div>
                                    <p class="mt-1">The one-stop-shop for all your school needs.
                                        From class notes to tutors, textbooks and more, we bring the best that the internet has to offer students, all together in one place.</p>
                                </div>
                            </v-flex>
                            <v-flex class="empty-filter-cell mb-2 elevation-1" order-xs1 v-if="showFilterNotApplied">
                                <v-layout row align-center justify-space-between>
                                    <img src="./img/emptyFilter.png" alt=""/>
                                    <p class="mb-0">Filter was not applied, because it will give you no results.</p>
                                </v-layout>
                                <button @click="showFilterNotApplied=false">OK</button>
                            </v-flex>
                            <slot name="resultData" :items="items">
                                <!-- && $vuetify.breakpoint.xsOnly -->
                                <router-link v-if="$route.path.slice(1)==='ask' " class="ask-question-mob  hidden-sm-and-down"  :to="{path:'/newquestion/'}">Ask Your Question</router-link>
                                <v-flex order-xs1 v-if="isAcademic&&showPersonalizeField&&!university && !loading" class="personalize-wrapper pa-3 mb-3 elevation-1">
                                    <v-text-field class="elevation-0" type="search" solo prepend-icon="sbf-search" placeholder="Where do you go to school?" @click="$_openPersonalize"></v-text-field>
                                </v-flex>
                                <v-flex class="result-cell  mb-3" xs-12 v-for="(item,index) in items" :key="index" :class="(index>6?'order-xs6': index>2 ? 'order-xs3' : 'order-xs2')">
                                    <component v-if="item.template!=='ask'" :is="'result-'+item.template" :item="item" :key="index" :index="index" class="cell" ></component>
                                    <router-link v-else :to="{path:'/question/'+item.id}" class="mb-5">
                                        <question-card :cardData="item"></question-card>
                                    </router-link>

                                    <div class="show-btn" v-if="item &&  item.user && accountUser && accountUser.id !== item.user.id || name!=='ask'" :class="'color-'+$route.path.slice(1)">{{name==='ask'?'Answer':'Show Me'}}</div>
                                </v-flex>
                                <router-link tag="v-flex" class="result-cell hidden-lg-and-up elevation-1 mb-3 xs-12 order-xs4 " :to="{path:'/'+currentSuggest,query:{q:this.userText}}">
                                    <suggest-card :name="currentSuggest"></suggest-card>
                                </router-link>
                            </slot>
                        </v-layout>
                    </v-container>
                </scroll-list>
                <div v-else>
                    <div class="result-cell elevation-1 mb-3 empty-state" xs-12>
                        <v-layout row class="pa-3">
                            <v-flex class="img-wrap mr-3">
                                <empty-state></empty-state>
                            </v-flex>
                            <v-flex>
                                <h6 class="mb-3">Your search - {{userText}} - did not match any records.</h6>
                                <div class="sug mb-2">Suggestions:</div>
                                <ul>
                                    <li>Check your spelling.</li>
                                    <li>Try different keywords.</li>
                                    <li>Try more general keywords.</li>
                                    <li>Try fewer keywords.</li>
                                </ul>
                            </v-flex>
                        </v-layout>
                    </div>
                </div>
            </div>
            <!--<div v-else class="skeleton"></div>-->
        </div>
        <template slot="sideBar" v-if="filterCondition">
            <component :is="($vuetify.breakpoint.xsOnly?'mobile-':'')+'sort-and-filter'"
                       :sortOptions="page.sort"
                       :sortVal="sort"
                       v-model="showFilters"
                       :filterOptions="filterObject"
                       :filterVal="filterSelection">
                <img :src="universityImage" slot="courseTitlePrefix" width="24" height="24" v-if="universityImage" />
                <template slot="courseEmptyState" v-if="!myCourses.length">
                    <div class="course-empty-state">
                        <div class="text">Add your school and courses for better results</div>
                        <button class="mobile-button" v-if="$vuetify.breakpoint.xsOnly" @click="$_openPersonalize">
                            <v-icon class="hidden-sm-and-up">sbf-search</v-icon>
                            <span class="hidden-sm-and-up" v-if="!university">Where do you go to school?</span>
                            <span class="hidden-sm-and-up" v-else>What class are you taking</span>
                        </button>
                        <v-btn v-else @click="$_openPersonalize">Personalize</v-btn>
                    </div>
                </template>
                <template slot="courseExtraState" v-else>
                    <button class="add-course" @click="$_openPersonalize" type="button">
                        <plus-btn></plus-btn><span>Add Course</span>
                    </button>
                </template>
                <button v-if="$vuetify.breakpoint.xsOnly" slot="courseMobileExtraState" class="edit-list" @click.stop.prevent="$_openPersonalize()" type="button">Edit List</button>
            </component>
        </template>

        <template slot="rightSide">
            <slot name="rightSide">
                <faq-block :isAsk="name==='ask'" :name="currentSuggest" :text="userText"></faq-block>
            </slot>

        </template>
        <!--<component slot="rightSide" v-if="hasExtra&&!isEmpty" :is="name+'-extra'" :place="selectedItem"></component>-->
        <slot name="suggestCell">
            <router-link slot="suggestCell" tag="v-flex" class="result-cell hidden-md-and-down elevation-1 mb-2 xs-12 order-xs3 " :to="{path:'/'+currentSuggest,query:{q:this.query.q}}">
                <suggest-card :name="currentSuggest"></suggest-card>
            </router-link>
        </slot>
    </general-page>
</template>
<script>
    import { pageMixin } from './Result'
    import signupBanner from './../helpers/signup-banner/signup-banner.vue'
    import QuestionCard from "../question/helpers/question-card/question-card";
    import {mapGetters} from 'vuex';
    export default {
        components: {QuestionCard, signupBanner},
        mixins: [pageMixin],
        computed:{
            ...mapGetters(["accountUser"])
        }
    }
</script>
<style src="./Result.less" lang="less">
</style>