<template>
    <general-page>
        <app-menu slot="verticalNavbar"></app-menu>
        <result-personalize v-show="isfirst" :show="showSearch" v-if="isfirst||showCourses"></result-personalize>
        <template slot="options" v-if="page">
            <!--<div class="sort-filter" v-if="page.sort || page.filter">
                <radio-list class="search" :values="page.filter" @click="$_changeFilter" model="filter" :value="filterOptions"></radio-list>
                <radio-list v-if="page.sort" :values="page.sort" @click="$_updateSort" model="sort" class="search sort" :value="$_defaultSort(page.sort[0].id)"></radio-list>
            </div>
            <div class="sort-filter">
                <radio-list :values="subFilters" @click="$_changeSubFilter" class="sub-search" model="subFilter" :value="subFilter"></radio-list>
            </div>-->
            <div class="sort-filter">
                <h3>Sort by</h3>
                <h3>filter by</h3>
                <div class="sort-filter" v-if="page.sort || page.filter">
                    <radio-list class="search" :values="[page.filter, pageData]" @click="$_changeSubFilter" model="page.filter" :value="filterOptions"></radio-list>
                </div>
            </div>
</template>
        <scroll-list slot="data" v-if="page&&items" @scroll="value => {items=items.concat(value) }" :token="pageData.token">
            <v-container class="pa-0">
                <v-layout column>
                    <v-flex class="elevation-1 mb-2" xs-12 v-for="(item,index) in items" :key="index" @click="(hasExtra?selectedItem=item.placeId:'')" :class="(index>6?'order-xs3':'order-xs1')">
                        <component :is="'result-'+item.template" :item="item" :key="index" class="cell"></component>
                    </v-flex>
                    <v-flex class="elevation-1 mb-2" xs-12 v-if="flowNode" v-for="(child,index) in flowNode.children" @click="$_updateCurrentFlow(index)" order-xs2>
                        <suggest-card :name="child.name"></suggest-card>
                    </v-flex>
                </v-layout>
            </v-container>
        </scroll-list>
        <component slot="adsense" v-if="hasExtra&&!isEmpty" :is="name+'-extra'" :place="selectedItem"></component>
    </general-page>
</template>
<script>
    import { pageMixin } from './mixins'
    export default {
        mixins: [pageMixin]
    }
</script>
<style src="./Result.less" lang="less">
</style>