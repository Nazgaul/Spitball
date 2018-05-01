<template>
    <general-page :title="(pageData&&pageData.details?pageData.details.title:'')">
        <template slot="sideBar">
            <component :is="($vuetify.breakpoint.xsOnly?'mobile-':'')+'sort-and-filter'"
                       :sortOptions="$vuetify.breakpoint.xsOnly?null:sortOptions" 
                       :sortVal="sortVal" v-model="showFilters"
                       :filterOptions="filterOptions" :filterVal="selectedFilter">
            </component>
        </template>
        {{selectedFilter}}
        <div slot="main" class="book-detail">
            <div class="d-cell elevation-1 pa-2">
                <result-book :item="pageData.details" :isDetails="true"></result-book>
            </div>
            <div class="d-flex mobile-filter" :class="sortVal==='buy'?'pb-2':'pb-3'">
                <v-btn class="hidden-sm-and-up text-xs-right" v-if="sortVal==='buy'" icon flat color="color-book" slot="mobileFilter" @click="showFilters=true">
                    <v-icon>sbf-filter</v-icon>
                    <div class="counter color-book" v-if="this.filterSelection.length">{{this.filterSelection.length}}</div>
                </v-btn>
            </div>
            <div class="book-sources pa-2 elevation-1" v-if="filteredList.length&&!isLoad">
                <a :href="item.link" :target="$vuetify.breakpoint.xsOnly?'_self':'_blank'" class="price-line" v-for="(item,index) in filteredList" :key="index">
                    <v-layout row justify-space-between class="price-line-content" @click="$ga.event('Search_Results', `Books_Details_${currentType}`,`#${index+1}_${item.name}`)">
                        <v-flex class="image text-xs-left">
                            <img v-if="item.image" :src="item.image" />
                            <span v-else>{{item.name}}</span>
                        </v-flex>
                        <v-flex class="text-xs-center">
                            {{item.condition}}
                        </v-flex>
                        <v-flex class="text-xs-right">
                            <div>
                                ${{item.price|floatDot(2)}}
                            </div>
                        </v-flex>
                    </v-layout>
                    <v-divider></v-divider>
                </a>
            </div>
            <div class="loader" v-else-if="isLoad">
                <v-progress-circular indeterminate v-bind:size="50"></v-progress-circular>
            </div>
        </div>
    </general-page>
</template>
<script src="./ResultBookDetails.js"></script>
<style src="./ResultBookDetail.less" lang="less"></style>