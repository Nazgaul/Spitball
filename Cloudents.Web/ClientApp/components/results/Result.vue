﻿
<template>
    <general-page :breakPointSideBar="$vuetify.breakpoint.lgAndUp" :name="name">
        <div slot="main" >
            <div class="d-flex mobile-filter hidden-sm-and-up">
                <v-btn icon :color="`color-${name}`" flat slot="mobileFilter" @click="showFilters=true" class="text-xs-right" v-if="filterObject">
                    <v-icon>sbf-filter</v-icon>
                </v-btn>
            </div>
            <div v-if="filterSelection.length" class="pb-3">
                <template v-for="item in filterSelection">
                    <v-chip label class="filter-chip elevation-1">
                        {{$_showSelectedFilter(item) | capitalize}}
                        <v-icon right @click="$_removeFilter(item)">sbf-close</v-icon>
                    </v-chip>
                </template>
            </div>
            <div :class="{'loading-skeleton': showSkelaton}">
                <scroll-list v-if="items.length" @scroll="value => {items=items.concat(value) }" :token="pageData.token">
                    <v-container class="pa-0">
                        <v-layout column>
                            <v-flex class="promo-cell mb-2 elevation-1" order-xs1 v-if="showPromo">
                                <button class="close-btn pa-2" @click="showPromo=false">
                                    <v-icon right>sbf-close</v-icon>
                                </button>
                                <div>
                                    <div class="promo-title" :class="`color-${name}--text`">{{currentPromotion.title}}</div>
                                    <p class="mt-1">{{currentPromotion.content}}</p>
                                </div>
                            </v-flex>
                       <slot name="resultData" :items="items">
                           <v-flex order-xs1 v-if="isAcademic&&showPersonalizeField&&!university" class="personalize-wrapper pa-3 mb-2 elevation-1">
                                <v-text-field class="elevation-0" type="search" solo prepend-icon="sbf-search" placeholder="Where do you go to school?" @click="$_openPersonalize"></v-text-field>
                            </v-flex>
                            <v-flex class="result-cell elevation-1 mb-2" xs-12 v-for="(item,index) in items" :key="index" :class="(index>6?'order-xs6': index>2 ? 'order-xs3' : 'order-xs2')">
                                <component :is="'result-'+item.template" :item="item" :key="index" :index="index" class="cell"></component>
                            </v-flex>
                            <router-link tag="v-flex" class="result-cell hidden-lg-and-up elevation-1 mb-2 xs-12 order-xs4 " :to="{path:'/'+currentSuggest,query:{q:this.userText}}">
                                <suggest-card :name="currentSuggest"></suggest-card>
                            </router-link>
                            <v-flex v-if="name==='ask'" class="result-cell elevation-1 mb-2 xs-12 order-xs2">
                                <studyblue-card :searchterm="userText"></studyblue-card>
                            </v-flex>
                       </slot>
                        </v-layout>
                    </v-container>
                </scroll-list>
                <div v-else>
                    <div class="result-cell elevation-1 mb-2 empty-state" xs-12>
                        <v-layout row class="ma-3">
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
        <template slot="sideBar" v-if="page">
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
    <slot name="rightSide"></slot>
</template>
        <!--<component slot="rightSide" v-if="hasExtra&&!isEmpty" :is="name+'-extra'" :place="selectedItem"></component>-->
       <slot name="suggestCell">
           <router-link slot="suggestCell"  tag="v-flex" class="result-cell hidden-md-and-down elevation-1 mb-2 xs-12 order-xs3 " :to="{path:'/'+currentSuggest,query:{q:this.query.q}}">
            <suggest-card :name="currentSuggest"></suggest-card>
        </router-link>
       </slot>
    </general-page>
</template>
<script>
    import { pageMixin } from './Result'
    export default {
        mixins: [pageMixin],
    }
</script>
<style src="./Result.less" lang="less">
</style>