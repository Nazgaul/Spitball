<template>
    <general-page :mdAndDown="$vuetify.breakpoint.mdAndDown" :breakPointSideBar="$vuetify.breakpoint.lgAndUp || $vuetify.breakpoint.mdOnly" :name="name">
        <div slot="main" class="feedWrap">
            <coursesTab/>
            <request-box class="request-box mb-0"/>
            <!-- <feedItemList class="mt-5" v-if="items"/> -->
            <div class="results-section mt-5" v-if="items">
                <scroll-list v-if="items.length" :scrollFunc="scrollFunc" :isLoading="scrollBehaviour.isLoading" :isComplete="scrollBehaviour.isComplete">
                    <v-container class="ma-0 results-wrapper pa-0">
                        <v-layout column>              
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
                        </v-layout>
                    </v-container>
                </scroll-list>
                <empty-state-card v-else :userText="userText" :helpAction="goToAskQuestion"/>
            </div>
            <feedSkeleton v-else v-for="n in 5" :key="n"/>
        </div>
        <feedFaqBlock slot="rightSide" v-if="showAdBlock"/>
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
}
</style>