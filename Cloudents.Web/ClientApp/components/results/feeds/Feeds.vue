<template>
    <general-page class="feed" :mdAndDown="$vuetify.breakpoint.mdAndDown" :breakPointSideBar="$vuetify.breakpoint.lgAndUp || $vuetify.breakpoint.mdOnly" :name="name">
        <div slot="main" class="feed__header">
            <div class="d-flex mobile-filter">
                  <askQuestionBtn :class="[!filterCondition ? 'no-filter-btn' : 'with-filter-btn', 'ask-question-mob', 'hidden-md-and-up'] "></askQuestionBtn>
            </div>
            <div class="request-box mb-0" :class="[$vuetify.breakpoint.xsOnly ? 'pt-4' : '']">
                <request-box></request-box>
            </div>
             <v-flex v-if="filterCondition" class="filter-container mb-4">
            </v-flex>
            <v-snackbar class="question-toaster" @click="loadNewQuestions()" :top="true" :timeout="0" :value="showQuestionToaster">
                <div class="text-wrap">
                    <v-icon class="refresh-style">sbf-arrow-upward</v-icon> &nbsp;&nbsp; <span v-language:inner>result_new_questions</span>
                </div>
            </v-snackbar>
            <div class="results-section" :class="{'loading-skeleton mt-5': showSkelaton}">
                <scroll-list v-if="items.length" :scrollFunc="scrollFunc" :isLoading="scrollBehaviour.isLoading" :isComplete="scrollBehaviour.isComplete">
                    <v-container class="ma-0 results-wrapper" :class="$vuetify.breakpoint.mdAndDown ? 'pa-2' : 'pa-0'">
                        <v-layout column>
                            <slot name="resultData" :items="items">                                
                                <v-flex class="result-cell mb-4" xs-12 v-for="(item,index) in items" :key="index"
                                    :class="(index>6?'order-xs6': index>2 ? 'order-xs3' : 'order-xs2')">
                                        <component 
                                            :id="index == 1 ? 'tour_vote' : ''"
                                            :is="setTemplate(item.template)"
                                            :item="item" 
                                            :key="index"
                                            :index="index"
                                            :tutorData="item"
                                            class="cell">
                                        </component>
                                </v-flex>
                                <v-flex class="suggestCard result-cell mb-4 xs-12 order-xs4">
                                    <suggest-card :name="currentSuggest" @click.native="openRequestTutor()"></suggest-card>   
                                </v-flex>
                                <!-- <v-flex 
                                    v-for="(tutor, index) in getTutorList" 
                                    :key="tutor.id" 
                                    class="result-cell"
                                    :class="(index > 6 ? 'order-xs6' : index > 2 ? 'order-xs3' : 'order-xs2')"
                                    >
                                    <tutor-result-card v-if="$vuetify.breakpoint.smAndUp" class="mb-4" :tutorData="tutor"></tutor-result-card>
                                    <tutor-result-card-mobile v-else class="mb-4" :tutorData="tutor"/>
                                </v-flex> -->
                            </slot>
                        </v-layout>
                    </v-container>
                </scroll-list>
                <div v-else>
                    <empty-state-card :userText="userText" :helpAction="goToAskQuestion"></empty-state-card>
                </div>
            </div>
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