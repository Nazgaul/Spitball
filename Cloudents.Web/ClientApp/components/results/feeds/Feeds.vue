<template>
    <general-page :mdAndDown="$vuetify.breakpoint.mdAndDown" :breakPointSideBar="$vuetify.breakpoint.lgAndUp || $vuetify.breakpoint.mdOnly" :name="$route.name">
        <div slot="main" class="feedWrap">
            <template v-if="showRequestBox">
                <request-box class="request-box mb-0"/>
            </template>
            <v-flex xs12 class="analyticWrapper" v-if="showAnalyticStats">
                <analyticOverview />
            </v-flex>

            <div class="feed_filters pa-3 pa-sm-0 my-4">
                <v-flex xs2 sm4 class="pr-0 pr-sm-4 d-flex d-sm-block" justify-end>
                    <v-menu offset-y sel="filter_type">
                        <template v-slot:activator="{ on }">
                            <v-btn icon v-on="on" class="filters_menu_btn d-block d-sm-none">
                                <v-icon class="icon">sbf-sort</v-icon>
                            </v-btn>
                        </template>
                        <v-list class="px-2">
                            <v-list-item v-for="(item, index) in filters" :key="index" @click="menuSelect(item.value)">
                                <v-list-item-title>{{dictionary[item.key]}}</v-list-item-title>
                            </v-list-item>
                        </v-list>
                    </v-menu>

                    <v-select class="profileItemsBox_filters_select d-none d-sm-flex"
                        sel="feed_filter"
                        :append-icon="'sbf-arrow-fill'"
                        v-model="query.filter"
                        :value="Feeds_getCurrentQuery.filter"
                        :items="filters"
                        :item-text="getSelectedName"
                        @change="handleSelects()"
                        :height="$vuetify.breakpoint.xsOnly ? 42 : 36" hide-details solo>
                    </v-select>
                </v-flex>
                <v-flex xs10 sm9 class="pr-4 pr-sm-0" v-if="!getUserLoggedInStatus">
                    <v-select class="profileItemsBox_filters_select"
                        sel="filter_course"
                        :append-icon="'sbf-arrow-fill'"
                        clearable
                        :disabled="!getUserLoggedInStatus"
                        :clear-icon="'sbf-close'"
                        v-model="query.course"
                        :value="Feeds_getCurrentQuery.course"
                        :items="courses"
                        @change="handleSelects()"
                        :placeholder="selectCoursePlaceholder" :height="$vuetify.breakpoint.xsOnly? 42 : 36" solo>
                    </v-select>
                </v-flex>
            </div>

            <div class="results-section mt-3 mt-sm-5" v-if="items">
                <scrollList v-if="items.length" :scrollFunc="scrollFunc" :isLoading="scrollBehaviour.isLoading" :isComplete="scrollBehaviour.isComplete">
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
                                <suggestCard/>   
                            </v-flex>
                        </v-layout>
                    </v-container>
                </scrollList>
                <emptyStateCard v-else/>
            </div>
            <feedSkeleton v-else v-for="n in 5" :key="n"/>
        </div>
        <template slot="rightSide">
            <div :class="['feed-sticky',{'feed-sticky_bannerActive':getBannerParams}]">
                <feedFaqBlock v-if="showAdBlock" class="mb-4"/>
                <buyPointsLayout v-if="getUserLoggedInStatus" class="buyPointsFeed"/>
            </div>
        </template>
    </general-page>
</template>

<script src="./Feeds.js"></script>

<style lang="less">
@import "../../../styles/mixin.less";

.feed-sticky{
    position: sticky;
    height: fit-content;
    top: 80px;
    &.feed-sticky_bannerActive{
        top: 150px;
    }
    .buyPointsFeed{
        max-width: 304px;
        .buyPointsLayout{
            max-width: 100%;
            @media (max-width: @screen-md-plus) {
                max-width: 100%;
                max-height: inherit;
                padding: 10px; 
                margin: inherit;
            }
            .buyPointsLayout_img{
                @media (max-width: @screen-md-plus) {
                    max-width: inherit;
                    height: 90px;
                }
            }
            .buyPointsLayout_action{
                @media (max-width: @screen-md-plus) {
                    margin-left: inherit;
                    width: 100%;
                }
                .buyPointsLayout_title{
                    @media (max-width: @screen-md-plus) {
                        font-size: 16px;
                    }
                }
                .buyPointsLayout_btn{
                    @media (max-width: @screen-md-plus) {
                        margin-top: 10px;
                        min-width: inherit;
                        max-width: inherit;
                    }
                }
            }
        }
    }
}
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
    .analyticWrapper {
        .analyticOverview {
            max-width: 720px;
            margin: auto;
        }   
    }
    .select-feed{
      @media (max-width: @screen-xs) {
         padding: 8px 12px 8px 14px;
         background: white;
      }
      .filters_select{
         color: #4d4b69;
         .responsive-property(height, 36px, null, 42px);
         .v-input__control{
            min-height: auto !important;
            display: unset;
            .responsive-property(height, 36px, null, 42px);
            .v-input__slot{
               border-radius: 8px;
               box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.15);
               margin: 0;
               @media (max-width: @screen-xs) {
                  box-shadow: none;
                  border: solid 1px #ced0dc;
               }
               .v-select__slot{
                  font-size: 14px;
                  .v-select__selections{
                     ::placeholder{
                        font-size: 14px;
                        color: #4d4b69;
                     }
                  }
                  .v-input__append-inner{
                     .v-input__icon{
                        &.v-input__icon--append{
                           i{
                              font-size: 6px;
                              color: #43425d;
                           }
                        }
                        &.v-input__icon--clear{
                           i{
                              font-size: 10px;
                              color: #43425d;
                           }
                        }
                     }
                  }
               }
            }
         }
      }
    }

    .feed_filters{
        display: flex;
        justify-content: space-between;
         @media (max-width: @screen-xs) {
            flex-direction: row-reverse;
            background: white;
        }
        .filtercourse{
            max-width: 100%;
            flex-grow: 1;
        }
        .filters_menu_btn{
            max-width: 44px;
            max-height: 42px;
            width: 44px;
            height: 42px;
            border-radius: 8px;
            border: solid 1px #ced0dc;
            background-color: white;
            color: #6f6e82;
            .icon{
                font-size: 16px;
            }
        }
        .profileItemsBox_filters_select{
            color: #4d4b69;
            .responsive-property(height, 36px, null, 42px);
            .v-input__control{
                min-height: auto !important;
                display: unset;
                .responsive-property(height, 36px, null, 42px);
                .v-input__slot{
                border-radius: 8px;
                box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.15);
                margin: 0;
                @media (max-width: @screen-xs) {
                    box-shadow: none;
                    border: solid 1px #ced0dc;
                }
                .v-select__slot{
                    font-size: 14px;
                    .v-select__selections{
                        ::placeholder{
                            font-size: 14px;
                            color: #4d4b69;
                        }
                    }
                    .v-input__append-inner{
                        .v-input__icon{
                            &.v-input__icon--append{
                            i{
                                font-size: 6px;
                                color: #43425d;
                            }
                            }
                            &.v-input__icon--clear{
                            button{
                                font-size: 10px;
                                color: #43425d;
                            }
                            }
                        }
                    }
                }
                }
            }
        }
    }
}
</style>