<template>
    <general-page :breakPointSideBar="$vuetify.breakpoint.lgAndUp || $vuetify.breakpoint.mdOnly" :name="name">
        <div slot="main">
            <div class="d-flex mobile-filter">
                  <askQuestionBtn :class="[!filterCondition ? 'no-filter-btn' : 'with-filter-btn', 'ask-question-mob', 'hidden-md-and-up'] "></askQuestionBtn>
            </div>
            <div class="request-box mb-0" :class="[$vuetify.breakpoint.xsOnly ? 'pt-3' : '']">
                <request-box></request-box>
            </div>
             <v-flex v-if="filterCondition" class="filter-container">
                <result-filter></result-filter>
                <div class="filter-button-container">
                    <v-btn 
                        icon 
                        :color="`color-ask`" 
                        flat 
                        slot="mobileFilter" 
                        @click="showFilters=true"
                        class="mobile-filter-icon-btn text-xs-right" 
                        v-if="filterCondition">
                        <v-icon>sbf-filter</v-icon>
                        <div :class="'counter fixedLocation color-ask'" v-if="this.filterSelection.length">
                            {{this.filterSelection.length}}
                        </div>
                    </v-btn>
                </div>
            </v-flex>
            <v-snackbar class="question-toaster" @click="loadNewQuestions()" :top="true" :timeout="0" :value="showQuestionToaster">
                <div class="text-wrap">
                    <v-icon class="refresh-style">sbf-arrow-upward</v-icon> &nbsp;&nbsp; <span v-language:inner>result_new_questions</span>
                </div>
            </v-snackbar>
            <div class="results-section" :class="{'loading-skeleton mt-5': showSkelaton}">
                <v-container v-if="items.length" class="ma-0 results-wrapper" :class="$vuetify.breakpoint.mdAndDown ? 'pa-2' : 'pa-0'">
                    <v-layout column>
                        <v-flex class="empty-filter-cell mb-2 elevation-1" order-xs1 v-if="showFilterNotApplied">
                            <v-layout row align-center justify-space-between>
                                <img src="../img/emptyFilter.png" alt=""/>
                                <p class="mb-0" v-language:inner>result_filter_not_applied</p>
                            </v-layout>
                            <button @click="showFilterNotApplied=false" v-language:inner>result_ok</button>
                        </v-flex>
                        <slot name="resultData" :items="items">
                            <v-flex
                                class="result-cell mb-3" xs-12 v-for="(item,index) in items" :key="index"
                                :class="(index > 6 ? 'order-xs6' : index > 2 ? 'order-xs3' : 'order-xs2')">
                                <component 
                                    :id="index == 1 ? 'tour_vote' : ''"
                                    :is="`result-${item.template}`"
                                    :item="item" :key="index"
                                    :index="index"
                                    class="cell">
                                </component>
                            </v-flex>
                            <v-flex class="suggestCard result-cell mb-3 xs-12 order-xs4">
                                <suggest-card :name="currentSuggest" @click.native="openRequestTutor()"></suggest-card>   
                            </v-flex>
                        </slot>
                    </v-layout>
                </v-container>
                <div v-else>
                    <empty-state-card :userText="userText" :helpAction="goToAskQuestion"></empty-state-card>
                </div>
            </div>
            <v-btn flat block color="#43425D" class="results-loader-btn" @click="loadMoreData" :loading="btnLoading">
                <span v-language:inner="'result_load_documents'"></span>
            </v-btn>
        </div>
        <template slot="sideBar" v-if="filterCondition">
            <component 
                :is="'mobile-sort-and-filter'"
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
                <faq-block :isAsk="true" :isNotes="false" :name="currentSuggest" :text="userText"></faq-block>
            </slot>
        </template>
            <v-flex class="result-cell mb-2 xs-12 order-xs3">
                <suggest-card :name="currentSuggest" @click.native="openRequestTutor()"></suggest-card>   
            </v-flex>
    </general-page>
</template>

<script src="./Feeds.js"></script>

<style src="../Result.less" lang="less"></style>