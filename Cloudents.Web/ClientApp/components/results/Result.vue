<template>
    <general-page :filterSelection="filterSelection">
        <app-menu slot="verticalNavbar" :$_calcTerm="$_calcTerm"></app-menu>
        <v-chip slot="selectedFilters" slot-scope="props" label>
            <strong>{{$_showSelectedFilter(props.item)}}</strong> <v-btn @click="$_removeFilter(props.item)">X</v-btn>
        </v-chip>
        <result-personalize v-if="isfirst"></result-personalize>
        <template slot="options" v-if="page">
            <div class="sort-filter">
                <slot v-if="page.sort">
                    <h3>Sort by</h3>
                    <sort-switch :options="page.sort" :callback="$_updateSort" :val="sort"></sort-switch>
                </slot>
                <slot v-if="page.filter">
                    <h3>filter by</h3>
                    <div class="sort-filter" v-if="page.filter">
                        <radio-list class="search" :callback="$_changeSubFilter" :values="filterObject" :checkesVals="filterSelection">
                            <div slot="courseEmptyState" v-if="!myCourses.length">
                                Add your school
                                and courses for better results <v-btn @click="$_openPersonalize">Personalize</v-btn>
                            </div>
                            <div slot="courseExtraState" v-else>
                                <v-btn @click="$_openPersonalize">Add course</v-btn>
                            </div>
                        </radio-list>
                    </div>
                </slot>
                <v-flex class="text-xs-center pt-2"> {{version}}</v-flex>
            </div>
        </template>
        <scroll-list slot="data" v-if="page&&items" @scroll="value => {items=items.concat(value) }" :token="pageData.token">
            <v-container class="pa-0">
                <v-layout column>
                    <v-flex class="elevation-1 mb-2" xs-12 v-for="(item,index) in items" :key="index" @click="(hasExtra?selectedItem=item.placeId:'')" :class="(index>6?'order-xs3':'order-xs1')">
                        <component :is="'result-'+item.template" :item="item" :key="index" class="cell"></component>
                    </v-flex>
                    <v-flex class="elevation-1 mb-2" xs-12 v-if="flowNode" v-for="(child,index) in flowNode.children" :key="index" @click="$_updateCurrentFlow(index)" order-xs2>
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