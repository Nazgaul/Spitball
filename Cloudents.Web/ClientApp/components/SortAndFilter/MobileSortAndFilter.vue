<template>
    <v-dialog v-model="value" fullscreen content-class="white filter-dialog">
        <dialog-toolbar height="48" toolbarTitle="Filter & Sort" :backAction="$_backAction">
            <v-btn slot="rightElement" flat class="clear-btn" @click="resetFilters">Clear all</v-btn>
        </dialog-toolbar>
 
        <v-btn class="apply elevation-0" fixed color="color-blue" @click="applyFilters">Apply Filters</v-btn>
        <div class="sort-wrap">
            <template v-if="sortOptions.length">
                <h3>Sort</h3>
                <div class="sort-switch">
                    <template v-for="(o,index) in sortOptions">
                        <input type="radio" :id="`option${index}`" v-model="sort"
                               name="switch" :value="o.id">
                        <label :for="`option${index}`">{{o.name}}</label>
                    </template>
                </div>
            </template>
        </div>
        <div class="filter-wrap" v-if="filterOptions && filterOptions.length">
            <h3 class="px-3">Filter</h3>
            <div class="filter-sections">
                <div class="filter-section" v-for="k in filterOptions" :key="k.modelId" :value="true">
                    <v-layout class="filter-header" slot="header">
                        <v-layout row align-center>
                            <div class="icon-wrapper">
                                <slot :name="`${k.modelId}TitlePrefix`"></slot>
                            </div>
                            <slot name="headerTitle" :title="k.title">
                                <div>{{k.title}}</div>
                            </slot>
                        </v-layout>
                        <slot :name="`${k.modelId}MobileExtraState`"></slot>
                    </v-layout>
                    <div class="filter-list">
                        <div v-for="s in k.data" :key="(s.id?s.id:s)" class="filter pl-3">
                            <input type="checkbox" :id="(s.id?s.id:s)" v-model="filters[k.modelId]" :value="(s.id?s.id:s)"/>

                            <span class="checkmark"></span>
                            <label :title="s.name?s.name:s" :for="(s.id?s.id:s)" class="py-2">
                                {{s.name?s.name:s | capitalize}}
                            </label>
                        </div>
                        <slot :name="`${k.modelId}EmptyState`" v-if="k.data&&k.data.length===0"></slot>
                    </div>
                </div>
            </div>
        </div>
    </v-dialog>
</template>

<script src="./MobileSortAndFilter.js">
</script>
<style src="./MobileSortAndFilter.less" lang="less"></style>