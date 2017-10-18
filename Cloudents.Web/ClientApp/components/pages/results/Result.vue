<template>
    <div class="sec-result">
        <h5>
            <span v-if="isEmpty" class="empty" v-html="page.emptyText.replace('$subject', term)"></span>
            <span v-else v-html="page.title"></span>{{dynamicHeader}}
        </h5>
        <slot name="options">
            <v-container>
                <v-layout row>
                    <radio-list class="search" :values="page.filter" @click="$_changeFilter" model="filter" :value="filterOptions"></radio-list>
                    <div class="s-divider"></div>
                    <radio-list v-if="page.sort" :values="page.sort" @click="$_updateSort" model="sort" class="search sort" :value="$_defaultSort(page.sort[0].id)"></radio-list>
                </v-layout>
                <radio-list :values="subFilters" @click="$_changeSubFilter" class="sub-search" model="subFilter" :value="subFilter"></radio-list>
            </v-container>
        </slot>
        <v-container>
            <v-layout row>
                <slot name="data" >
                    <scroll-list :loadMore="!isEmpty" v-if="pageData.data" @scroll="value => {pageData.data=pageData.data.concat(value) }">
                        <v-container fluid grid-list-md v-for="(item,index) in pageData.data">
                            <component :is="'result-'+item.template" :item="item" :key="index" class="cell"></component>
                        </v-container>
                    </scroll-list>
                </slot>
                <div class="pa-2">
                    <img src="http://lorempixel.com/320/240/" />
                    <img src="http://lorempixel.com/320/240/" />
                </div>
            </v-layout>
        </v-container>

        <!--<slot name="emptyState" v-show="isEmpty"></slot>-->
        
    </div>
</template>
<script>
    import { pageMixin } from './mixins'
    export default {
        mixins: [pageMixin]
    }
</script>
<style src="./Result.less" lang="less">
    
</style>