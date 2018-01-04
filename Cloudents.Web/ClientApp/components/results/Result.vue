﻿
<template>
    <general-page :breakPointSideBar="$vuetify.breakpoint.lgAndUp">
        <div slot="main">
            <div class="d-flex mobile-filter hidden-sm-and-up">
                <v-btn icon :color="`color-${name}`" flat slot="mobileFilter" @click="showFilters=true" class="text-xs-right" v-if="filterObject">
                    <v-icon>sbf-filter</v-icon>
                </v-btn>
            </div>
            <div v-if="filterSelection.length" class="pb-3">
                <template v-for="item in filterSelection">
                    <v-chip label class="filter-chip elevation-1" @click="$_removeFilter(item)">
                        {{$_showSelectedFilter(item) | capitalize}}
                        <v-icon right>sbf-close</v-icon>
                    </v-chip>
                </template>
            </div>
            <scroll-list v-if="items.length" @scroll="value => {items=items.concat(value) }" :token="pageData.token">
                <v-container class="pa-0">
                    <v-layout column>
                        <v-flex order-xs1 v-if="showPersonalizeField&&!university" class="personalize-wrapper pa-3 mb-2 elevation-1">
                            <v-text-field class="elevation-0" type="search" solo prepend-icon="sbf-search" placeholder="Where do you go to school?" @click="$_openPersonalize"></v-text-field>
                        </v-flex>
                        <v-flex class="result-cell elevation-1 mb-2" xs-12 v-for="(item,index) in items" :key="index" @click="(hasExtra?selectedItem=item.placeId:'')" :class="(index>6?'order-xs4':'order-xs2')">
                            <component :is="'result-'+item.template" :item="item" :key="index" class="cell"></component>
                        </v-flex>
                        <router-link v-if="!hasExtra" tag="v-flex" class="result-cell hidden-lg-and-up elevation-1 mb-2 xs-12 order-xs3 " :to="{path:'/'+currentSuggest,query:{q:this.query.q}}">
                            <suggest-card :name="currentSuggest"></suggest-card>
                        </router-link>
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
                            <h6 class="mb-3">Your search - {{term}} - did not match any records.</h6>
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
        <template slot="sideBar" v-if="page">
            <component :is="($vuetify.breakpoint.xsOnly?'mobile-':'')+'sort-and-filter'"
                       :sortOptions="page.sort"
                       :sortCallback="$_updateSort"
                       :sortVal="sort"
                       v-model="showFilters"
                       :filterOptions="filterObject"
                       :filterCallback="$_updateFilter"
                       :filterVal="filterSelection">
                <img :src="universityImage" slot="courseTitlePrefix" width="24" height="24" v-if="universityImage" />
                <template slot="courseEmptyState" v-if="!myCourses.length">
                    <button v-if="$vuetify.breakpoint.xsOnly" class="edit-list hidden-sm-and-up" @click="$_openPersonalize()">Empty state mobile</button>
                    <div class="course-empty-state" v-else>
                        <div>Add your school and courses for better results</div>
                        <v-btn @click="$_openPersonalize">Personalize</v-btn>
                    </div>
                </template>
                <template slot="courseExtraState" v-else>
                    <button class="add-course" @click="$_openPersonalize" type="button">
                        <plus-btn></plus-btn><span>Add Course</span>
                    </button>
                </template>
                <button v-if="$vuetify.breakpoint.xsOnly" slot="mobileExtra" class="edit-list" @click.stop.prevent="$_openPersonalize()" type="button">Edit List</button>
            </component>
        </template>


        <component slot="rightSide" v-if="hasExtra&&!isEmpty" :is="name+'-extra'" :place="selectedItem"></component>
        <router-link slot="suggestCell" v-if="!hasExtra" tag="v-flex" class="result-cell hidden-md-and-down elevation-1 mb-2 xs-12 order-xs3 " :to="{path:'/'+currentSuggest,query:{q:this.query.q}}">
            <suggest-card :name="currentSuggest"></suggest-card>
        </router-link>
    </general-page>
</template>
<script>
    import { pageMixin } from './mixins'

    export default {
        mixins: [pageMixin],
        // components: { plusBtn,closeBtn }
    }
</script>
<style src="./Result.less" lang="less">
</style>
<style lang="less" src="../settings/ResultPersonalize.less">
</style>