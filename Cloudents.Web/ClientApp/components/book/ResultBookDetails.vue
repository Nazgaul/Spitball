<template>
    <general-page v-if="pageData.details" 
                  :title="(pageData.details?pageData.details.title:'')">
        <template slot="sideBar" v-if="page">
            <component :is="($vuetify.breakpoint.xsOnly?'mobile-':'')+'sort-and-filter'"
                       :sortOptions="sortOptions" :sortCallback="$_updateSort" :sortVal="sortVal"
                       v-model="showFilters"
                       :filterOptions="filterOptions"
                       :filterCallback="$_updateFilter"
                       :filterVal="[filter]">
                </component>
                <!--<sort-and-filter :sortOptions="sortOptions" :sortCallback="$_updateSort"
                                 :sortVal="sortVal" :filterOptions="filterOptions"
                                 :filterCallback="$_updateFilter" :filterVal="[filter]"></sort-and-filter>-->
        </template>
        <div slot="main" class="book-detail">
            <div class="d-cell elevation-1 pa-2">
                <result-book :item="pageData.details" :isDetails="true"></result-book>
            </div>
            <div class="d-flex mobile-filter hidden-sm-and-up">
                <v-btn icon flat color="color-book" slot="mobileFilter" @click="showFilters=true" class="text-xs-right mb-2">
                    <v-icon>sbf-filter</v-icon>
                </v-btn>
            </div>
            <div class="book-sources pa-2 elevation-1">
                <a :href="item.link" :target="$vuetify.breakpoint.xsOnly?'_self':'_blank'" class="price-line" v-for="(item,index) in filteredList" :key="index">
                    <v-layout row justify-space-between class="price-line-content">
                        <v-flex class="image text-xs-left">
                            <img v-if="item.image" :src="item.image" />
                            <span v-else>{{item.name}}</span>
                        </v-flex>
                        <v-flex class="text-xs-center">
                            {{item.condition}}
                        </v-flex>
                        <v-flex class="text-xs-right">
                            <div>
                                ${{item.price}}
                            </div>
                        </v-flex>
                    </v-layout>
                    <v-divider></v-divider>
                </a>
            </div>
        </div>
    </general-page>
</template>
<script src="./ResultBookDetails.js"></script>
<style src="./ResultBookDetail.less" lang="less"></style>