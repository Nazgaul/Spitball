<template>
    <general-page :filterSelection="filterSelection">
        <app-menu slot="verticalNavbar" :$_calcTerm="$_calcTerm"></app-menu>
        <v-chip color="color-light-gray" label slot="selectedFilters" slot-scope="props" class="filter-chip" @click="$_removeFilter(props.item)">
            {{$_showSelectedFilter(props.item)}}
            <v-icon right>sbf-close</v-icon>
            <!--<span class="chip chip--removable" >
                 <close-btn></close-btn>
            </span>-->
        </v-chip>
        <template slot="options" v-if="page">
            <sort-and-filter :sortOptions="page.sort" :sortCallback="$_updateSort" :sortVal="sort"
                             :filterOptions="filterObject" :filterCallback="$_changeSubFilter" :filterVal="filterSelection"
                             :version="$version">
                <div slot="courseEmptyState" v-if="!myCourses.length">
                    Add your school
                    and courses for better results <v-btn @click="$_openPersonalize">Personalize</v-btn>
                </div>
                <template slot="courseExtraState" v-else>
                    <button class="add-course"  @click="$_openPersonalize" type="button">
                        <plus-btn></plus-btn> Add Course
                    </button>
                </template>
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
        mixins: [pageMixin],
       // components: { plusBtn,closeBtn }
    }
</script>
<style src="./Result.less" lang="less">
</style>