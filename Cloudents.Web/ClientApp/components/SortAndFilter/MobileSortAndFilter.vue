<template>
    <v-dialog v-model="value" fullscreen content-class="filter-dialog" persistent>
        <dialog-toolbar :app="false" height="48" toolbarTitle="Filter & Sort" :backAction="$_backAction">
            <v-btn slot="rightElement" flat class="clear-btn" @click="resetFilters">Clear all</v-btn>
        </dialog-toolbar>
        <div class="content-container">
            <div class="sort-wrap" v-if="sortOptions && sortOptions.length">
              <template>
                <h3>Sort by</h3>
                <div class="sort-switch">
                <template v-for="(singleSort, index) in sortOptions">
                <input type="radio" :id="`option${index}`" v-model="sort" :key="`option${index}`"
                name="switch" :value="singleSort">
                <label :for="`option${index}`" :key="index">{{singleSort}}</label>
                </template>
                </div>
                </template>
            </div>
            <div class="filter-wrap px-3" v-if="filterOptions && filterOptions.length">
                <h3 class="" >Filter By</h3>
                <div class="filter-sections">
                    <div class="filter-section" v-for="(singleFilter) in filterOptions" :key="singleFilter.id" :value="true">
                        <v-layout class="filter-header" slot="header">
                            <v-layout row align-center>
                                <div class="icon-wrapper">
                                    <slot :name="`${singleFilter.title}TitlePrefix`"></slot>
                                </div>
                                <slot name="headerTitle" :title="singleFilter.title">
                                    <div>{{singleFilter.title}}</div>
                                </slot>
                            </v-layout>
                        </v-layout>
                        <div class="filter-list">
                            <v-btn-toggle  v-model="filtersSelected" multiple>
                                <v-btn v-for="filterOption in singleFilter.data"
                                       :key="(filterOption.id ? filterOption.id : filterOption)"
                                       :active="filterOption"
                                       :id="filterOption"
                                       :value="`${singleFilter.id}_${filterOption}`">
                                    {{filterOption ? filterOption:filterOption | capitalize}}
                                </v-btn>
                            </v-btn-toggle>
                            <slot :name="`${singleFilter.modelId}EmptyState`" v-if="singleFilter.data && singleFilter.data.length===0"></slot>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <v-btn class="apply elevation-0" fixed  @click="applyFilters">Apply Filters</v-btn>

    </v-dialog>
</template>

<script src="./MobileSortAndFilter.js">
</script>
<style src="./MobileSortAndFilter.less" lang="less"></style>