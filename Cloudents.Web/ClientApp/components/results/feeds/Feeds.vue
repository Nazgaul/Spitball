<template>
    <general-page :mdAndDown="$vuetify.breakpoint.mdAndDown" :breakPointSideBar="$vuetify.breakpoint.lgAndUp || $vuetify.breakpoint.mdOnly" :name="name">
        <div slot="main" class="feedWrap">
            <coursesTab/>
            <div class="request-box mb-0">
                <request-box></request-box>
            </div>
             <v-flex v-if="filterCondition" class="filter-container"></v-flex>
            <v-snackbar class="question-toaster" @click="loadNewQuestions()" :top="true" :timeout="0" :value="showQuestionToaster">
                <div class="text-wrap">
                    <v-icon class="refresh-style">sbf-arrow-upward</v-icon> &nbsp;&nbsp; <span v-language:inner>result_new_questions</span>
                </div>
            </v-snackbar>
            <div class="results-section mt-5" v-if="items">
                <scroll-list v-if="items.length" :scrollFunc="scrollFunc" :isLoading="scrollBehaviour.isLoading" :isComplete="scrollBehaviour.isComplete">
                    <v-container class="ma-0 results-wrapper pa-0">
                        <v-layout column>
                            <slot name="resultData" :items="items">                                
                                <v-flex class="result-cell" xs-12 v-for="(item,index) in items" :key="index"
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
                            </slot>
                        </v-layout>
                    </v-container>
                </scroll-list>
                <div v-else>
                    <empty-state-card :userText="userText" :helpAction="goToAskQuestion"></empty-state-card>
                </div>
            </div>
            <v-sheet color="#fff" class="mt-5 skeletonWarp" v-else v-for="n in 5" :key="n">
                <v-skeleton-loader type="list-item-avatar-two-line" max-width="250"></v-skeleton-loader>
                <v-skeleton-loader type="list-item-three-line"></v-skeleton-loader>
                <v-skeleton-loader type="actions"></v-skeleton-loader>
            </v-sheet>
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
        <template slot="rightSide" v-if="showAdBlock">
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

<style lang="less">
@import "../../../styles/mixin.less";

.feedWrap {
    .results-section{
        .results-wrapper{
            .suggestCard{
                cursor: pointer;
                box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.24);
                .responsive-property(margin-bottom, 16px, null, 8px);
            }
        }
    }
    .skeletonWarp {
        border-radius: 12px;
        .v-skeleton-loader__avatar {
            border-radius: 50%;
        }
    }
    .question-toaster{
        margin-top: 122px;
        cursor: pointer;
        z-index:122;
        .v-snack__wrapper{
            box-shadow: none;
            background-color: transparent;
            margin: 0 auto;
            width: unset;
            .v-snack__content{
                width: unset;
                border-radius: 50px;
                height: 32px;
                background-color: #0091ca;
                padding: 14px 20px;
                box-shadow: 0 4px 11px 0 rgba(0, 0, 0, 0.26);
            }
        }
        .refresh-style {
            color: #fff;
            width: 0px;
            height: 19px;
            font-size: 14px;
        }
        @media (max-width: @screen-xs) {
            margin-top: 25px;
        }
    }
}
</style>