<template>
    <general-page :breakPointSideBar="$vuetify.breakpoint.lgAndUp || $vuetify.breakpoint.mdOnly" :name="name">
       
        <div slot="main">
            <div class="d-flex mobile-filter">
                  <upload-files-btn class="upload-card hidden-md-and-up"></upload-files-btn>
            </div>
            <div class="request-box" :class="[$vuetify.breakpoint.xsOnly ? 'pt-3' : '']">
                <request-box></request-box>
            </div>
            <v-flex v-if="filterCondition" class="filter-container">
                <result-filter></result-filter>
                <div class="filter-button-container">
                    <v-btn icon :color="`color-note`" flat slot="mobileFilter" @click="showFilters=true"
                       class="mobile-filter-icon-btn text-xs-right" v-if="filterCondition">
                    <v-icon>sbf-filter</v-icon>
                    <div :class="'counter fixedLocation color-note'"
                         v-if="this.filterSelection.length">{{this.filterSelection.length}}
                    </div>
                </v-btn>
                </div>
            </v-flex>

            <div class="results-section" :class="{'loading-skeleton mt-5': showSkeleton}">
                <scroll-list v-if="items.length" :scrollFunc="scrollFunc" :isLoading="scrollBehaviour.isLoading" :isComplete="scrollBehaviour.isComplete">
                    <v-container class="ma-0 results-wrapper" :class="$vuetify.breakpoint.mdAndDown ? 'pa-2' : 'pa-0'">
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
                                <v-flex class="suggestCard result-cell mb-2 xs-12 order-xs4">
                                    <suggest-card :name="currentSuggest" @click.native="openRequestTutor()"></suggest-card>   
                                </v-flex>
                            </slot>
                        </v-layout>
                    </v-container>
                </scroll-list>
                <div v-else>
                    <empty-state-card :userText="userText"></empty-state-card>
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
            <v-flex class="result-cell mb-2 xs-12 order-xs3">
                <suggest-card :name="currentSuggest" @click.native="openRequestTutor()"></suggest-card>   
            </v-flex>
    </general-page>
</template>

<script src="./StudyDocuments.js"></script>
<style src="../Result.less" lang="less">
</style>