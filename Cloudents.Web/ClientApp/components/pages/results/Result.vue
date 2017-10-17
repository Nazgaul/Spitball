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
            
            
            <!--<v-container grid-list-md>
                <v-layout row wrap>
                    <v-flex>
                        <radio-list class="search" :values="page.filter" @click="$_changeFilter" model="filter" :value="filterOptions"></radio-list>
                    </v-flex>
                    <v-flex>
                        <radio-list v-if="page.sort" :values="page.sort" @click="$_updateSort" model="sort" class="search sort" :value="$_defaultSort(page.sort[0].id)"></radio-list>
                    </v-flex>

                </v-layout>
                <v-flex xs12>
                    <radio-list :values="subFilters" @click="$_changeSubFilter" class="sub-search" model="subFilter" :value="subFilter"></radio-list>
                </v-flex>
            </v-container>-->
        </slot>
        <slot name="data">
            <scroll-list :loadMore="!isEmpty" v-if="pageData.data" @scroll="value => {pageData.data=pageData.data.concat(value) }">
                <v-container fluid grid-list-md v-for="(item,index) in pageData.data" >
                    <component :is="'result-'+item.template" :item="item" :key="index" class="cell"></component>
                </v-container>
            </scroll-list>
        </slot>

        <!--<slot name="emptyState" v-show="isEmpty"></slot>-->
        <!--<v-flex xs4>
            <img src="http://lorempixel.com/246/205/sports/" />
            <img src="http://lorempixel.com/246/205/sports/" />
        </v-flex>-->
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