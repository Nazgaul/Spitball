<template>
   <general-page :isLoading="isLoading" :title="titleText">
       <result-personalize v-show="isfirst" :show="showSearch" v-if="isfirst||showCourses"></result-personalize>
       <!--<h5 slot="title">-->
           <!--<span v-if="isEmpty" class="empty" v-html="page.emptyText.replace('$subject', term)"></span>-->
           <!--<span v-else v-html="titleText"></span> {{dynamicHeader}}-->
       <!--</h5>-->
       <v-container class="pa-0 mb-3" slot="options">
           <v-layout row>
               <radio-list class="search" :values="page.filter" @click="$_changeFilter" model="filter" :value="filterOptions"></radio-list>
               <div class="s-divider"></div>
               <radio-list v-if="page.sort" :values="page.sort" @click="$_updateSort" model="sort" class="search sort" :value="$_defaultSort(page.sort[0].id)"></radio-list>
           </v-layout>
           <radio-list :values="subFilters" @click="$_changeSubFilter" class="sub-search" model="subFilter" :value="subFilter"></radio-list>
       </v-container>
       <scroll-list slot="data" :loadMore="!isEmpty" v-if="items" @scroll="value => {items=items.concat(value) }" :token="pageData.token">
           <v-container fluid grid-list-sm v-for="(item,index) in items" :key="index" @click="(hasExtra?selectedItem=item.placeId:'')" :class="(index>6?'order-xs3':'order-xs1')">
               <component :is="'result-'+item.template" :item="item" :key="index" class="cell"></component>
           </v-container>
           <div v-if="flowNode" v-for="(child,index) in flowNode.children" class="suggest" @click="$_updateCurrentFlow(index)" order-xs2>
               <suggest-card :name="child.name"></suggest-card>
           </div>
       </scroll-list>
       <component slot="adsense"  v-if="hasExtra&&!isEmpty" :is="name+'-extra'" :place="selectedItem"></component>

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