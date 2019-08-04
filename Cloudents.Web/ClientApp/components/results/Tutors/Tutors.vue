<template>
    <general-page :breakPointSideBar="$vuetify.breakpoint.lgAndUp || $vuetify.breakpoint.mdOnly" :name="name" >
       
        <div slot="main">
            
            <div class="request-box mb-4" style="max-width: 640px" :class="[$vuetify.breakpoint.xsOnly ? 'pt-3' : '']">
                <request-box></request-box>
            </div>
            <v-flex v-if="filterCondition" class="filter-container">
                <result-filter></result-filter>
                <div class="filter-button-container">
                    <v-btn icon :color="`color-tutor`" flat slot="mobileFilter" @click="showFilters=true"
                       class="mobile-filter-icon-btn text-xs-right" v-if="filterCondition">
                    <v-icon>sbf-filter</v-icon>
                    <div :class="'counter fixedLocation color-tutor'"
                         v-if="this.filterSelection.length">{{this.filterSelection.length}}
                    </div>
                </v-btn>
                </div>
            </v-flex>
            
            <div class="results-section" :class="{'loading-skeleton': showSkeleton}">
                <scroll-list v-if="items.length" :scrollFunc="scrollFunc" :isLoading="scrollBehaviour.isLoading" :isComplete="scrollBehaviour.isComplete">
                    <v-container class="ma-0 results-wrapper pa-0">
                        <v-layout column>
                            <v-flex class="empty-filter-cell mb-2 elevation-1" order-xs1 v-if="showFilterNotApplied">
                                <v-layout row align-center justify-space-between>
                                    <img src="../img/emptyFilter.png" alt=""/>
                                    <tutor-resultp class="mb-0" v-language:inner>result_filter_not_applied</tutor-resultp>
                                </v-layout>
                                <button @click="showFilterNotApplied=false" v-language:inner>result_ok</button>
                            </v-flex>
                            <slot name="resultData" :items="items">
                                <!--<v-flex v-show="!showSkelaton && showSelectUni" class="result-cell mb-3 empty-state-tutor" xs-12>-->
                                    <!--<set-uni-class class="cell"></set-uni-class>-->
                                <!--</v-flex>-->
                                
                                <v-flex class="result-cell" xs-12 v-for="(item, index) in items" :key="index"
                                        :class="(index>6?'order-xs6': index>2 ? 'order-xs3' : 'order-xs2')">
                                    <component v-if="showSkeleton" :is="'result-'+item.template" :item="item" :key="index" :index="index" class="cell"></component>
                                    <template v-else>
                                        <tutor-result-card class="mb-3" v-if="$vuetify.breakpoint.mdAndUp" :tutorData="item"></tutor-result-card>
                                        <tutor-result-card-mobile class="mb-2" v-else :tutorData="item"></tutor-result-card-mobile>
                                    </template>
                                </v-flex>
                                <v-flex class="suggestCard result-cell xs-12 order-xs4">
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

        <!--<template slot="rightSide">-->
            <!--<slot name="rightSide">-->
                <!--<faq-block :isAsk="false" :isNotes="false" :name="currentSuggest" :text="userText"></faq-block>-->

            <!--</slot>-->
        <!--</template>-->>
            <v-flex class="result-cell mb-2 xs-12 order-xs3">
                <suggest-card @click.native="openRequestTutor()" :name="currentSuggest"></suggest-card>   
            </v-flex>
    </general-page>
</template>

<script src="./Tutors.js"></script>
<style src="../Result.less" lang="less">
</style>