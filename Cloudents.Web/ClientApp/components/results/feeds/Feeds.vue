<template>
    <general-page :mdAndDown="$vuetify.breakpoint.mdAndDown" :breakPointSideBar="$vuetify.breakpoint.lgAndUp || $vuetify.breakpoint.mdOnly" :name="$route.name">
        <div slot="main" class="feedWrap">
            <v-btn @click="$openDialog('createCoupon')" color="success">text</v-btn>
            <coursesTab/>
            <request-box class="request-box mb-0"/>
            <v-flex xs12 class="mt-3 analyticWrapper" v-if="showAnalyticStats">
                <analyticOverview/>
            </v-flex>
            <v-flex xs12 sm4 class="select-feed mt-3">
                <v-select class=" filters_select"
                    sel="feed_filter"
                    :append-icon="'sbf-arrow-fill'"
                    v-model="query.filter"
                    :value="Feeds_getCurrentQuery.filter"
                    :items="filters"
                    :item-text="getSelectedName"
                    @change="handleSelects()"
                    :height="$vuetify.breakpoint.xsOnly? 42 : 36" hide-details solo>
                    <template v-slot:item="{item}">
                        {{dictionary[item.key]}}
                    </template>
                </v-select>
            </v-flex>
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

}
</style>