<template>
    <general-page :filterSelection="filterSelection">
        <app-menu slot="verticalNavbar" :$_calcTerm="$_calcTerm"></app-menu>
        <v-chip slot="selectedFilters" slot-scope="props" class="chip--removable">
            {{$_showSelectedFilter(props.item)}}
            <span class="chip chip--removable" @click="$_removeFilter(props.item)">
                X
            </span>
            <!--<strong>{{$_showSelectedFilter(props.item)}}</strong> <v-btn @click="$_removeFilter(props.item)">X</v-btn>-->
        </v-chip>
        <template slot="options" v-if="page">
            <sort-and-filter :sortOptions="page.sort" :sortCallback="$_updateSort" :sortVal="sort"
                             :filterOptions="filterObject" :filterCallback="$_changeSubFilter" :filterVal="filterSelection"
                             :version="$version">
                <div slot="courseEmptyState" v-if="!myCourses.length">
                    Add your school
                    and courses for better results <v-btn @click="$_openPersonalize">Personalize</v-btn>
                </div>
                <div slot="courseExtraState" v-else>
                    <v-btn @click="$_openPersonalize">Add course</v-btn>
                </div>
            </sort-and-filter>
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