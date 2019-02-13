<template>
    <general-page :breakPointSideBar="$vuetify.breakpoint.lgAndUp || $vuetify.breakpoint.mdOnly" :name="name">
        <soon-component v-show="currentNavData.soon" slot="soonComponent"></soon-component>
        
        <div slot="main">
            <div class="d-flex mobile-filter">
                  <upload-files-btn class="upload-card hidden-md-and-up"></upload-files-btn>
            </div>
            <v-flex v-if="filterCondition" class="filter-container">
                <div>
                    Results From Harvard University
                </div>
                <div>
                    <v-btn icon :color="`color-note`" flat slot="mobileFilter" @click="showFilters=true"
                       class="mobile-filter-icon-btn text-xs-right" v-if="filterCondition">
                    <v-icon>sbf-filter</v-icon>
                    <div :class="'counter fixedLocation color-note'"
                         v-if="this.filterSelection.length">{{this.filterSelection.length}}
                    </div>
                </v-btn>
                </div>
            </v-flex>
            
            <div class="results-section" :class="{'loading-skeleton': showSkelaton}">
                <scroll-list v-if="items.length" :scrollFunc="scrollFunc" :isLoading="scrollBehaviour.isLoading" :isComplete="scrollBehaviour.isComplete">
                    <v-container class="pa-0 ma-0 results-wrapper">
                        <v-layout column>
                            <v-flex class="empty-filter-cell mb-2 elevation-1" order-xs1 v-if="showFilterNotApplied">
                                <v-layout row align-center justify-space-between>
                                    <img src="../img/emptyFilter.png" alt=""/>
                                    <p class="mb-0" v-language:inner>result_filter_not_applied</p>
                                </v-layout>
                                <button @click="showFilterNotApplied=false" v-language:inner>result_ok</button>
                            </v-flex>
                            <slot name="resultData" :items="items">
                                <v-flex v-show="!showSkelaton && showSelectUni" class="result-cell mb-2" xs-12>
                                    <set-uni-class class="cell"></set-uni-class>
                                </v-flex>
                                
                                <v-flex class="result-cell mb-2" xs-12 v-for="(item,index) in items" :key="index"
                                        :class="(index>6?'order-xs6': index>2 ? 'order-xs3' : 'order-xs2')">
                                    <component :id="index == 1 ? 'tour_vote' : ''" :is="'result-'+item.template" :item="item" :key="index" :index="index" class="cell"></component>
                                </v-flex>
                                <router-link tag="v-flex"
                                             class="result-cell hidden-lg-and-up elevation-1 mb-2 xs-12 order-xs4 "
                                             :to="{path:'/'+currentSuggest,query:{term:this.userText}}">
                                    <suggest-card :name="currentSuggest"></suggest-card>
                                </router-link>
                            </slot>
                        </v-layout>
                    </v-container>
                </scroll-list>
                <div v-else>
                    <div class="result-cell elevation-1 mb-2 empty-state" xs-12>
                        <v-layout row class="pa-3">
                            <v-flex>
                                <h6 class="mb-3"><span v-language:inner>result_your_search</span> - <span
                                        class="user-search-text">{{userText}}</span> - <span v-language:inner>result_record_not_match</span>
                                </h6>
                                <div class="sug mb-2" v-language:inner>result_suggestions</div>
                                <ul>
                                    <li v-language:inner>result_spelling</li>
                                    <li v-language:inner>result_different_keywords</li>
                                    <li v-language:inner>result_general_keywords</li>
                                    <li v-language:inner>result_fewer_keywords</li>
                                </ul>
                            </v-flex>
                        </v-layout>
                    </div>
                </div>
            </div>
        </div>
        <template slot="sideBar" v-if="filterCondition">
            <component :is="'mobile-sort-and-filter'"
                       :sortOptions="page.sort"
                       :sortVal="sort"
                       v-model="showFilters"
                       :filterOptions="getFilters"
                       :filterVal="filterSelection">
                <img :src="universityImage" slot="courseTitlePrefix" width="24" height="24" v-if="universityImage"/>
            </component>
        </template>

        <template slot="rightSide">
            <slot name="rightSide">
                <faq-block :isAsk="false" :isNotes="true" :name="currentSuggest" :text="userText"></faq-block>

            </slot>
        </template>
        <slot name="suggestCell">
            <router-link slot="suggestCell" tag="v-flex"
                         class="result-cell hidden-md-and-down elevation-1 mb-2 xs-12 order-xs3 "
                         :to="{path:'/'+currentSuggest,query:{term:this.query.term}}">

                <suggest-card :name="currentSuggest"></suggest-card>
            </router-link>
        </slot>
    </general-page>
</template>

<script src="./StudyDocuments.js"></script>
<style src="../Result.less" lang="less">
</style>