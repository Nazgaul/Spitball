<template>
    <v-dialog v-model="value" content-class="filter-dialog"  :max-width="$vuetify.breakpoint.smAndUp ? '720px' : ''" :fullscreen="$vuetify.breakpoint.xsOnly"  persistent>
        <div class="dialog-header">
            <!-- <span class="dialog-title">{{toolBarTitle}}</span> -->
            <v-icon class="dialog-title">sbf-filter</v-icon>
            <v-icon  class="close-icon-filter" @click="resetFilters">sbf-close</v-icon>
        </div>

        <!--<dialog-toolbar height="48" :toolbarTitle="toolBarTitle" :backAction="$_backAction">-->
            <!--<v-btn slot="rightElement" flat class="clear-btn" @click="resetFilters">-->
                <!--<v-icon  slot="rightElement" class="close-icon-filter" @click="resetFilters">sbf-close</v-icon>-->
                <!--<span v-language:inner>mobileSortAndFilter_clearAll</span> -->
            <!--</v-btn>-->
        <!--</dialog-toolbar>-->
        <div class="content-container">
            <div class="sort-wrap" v-if="sortOptions && sortOptions.length">
              <template>
                <h3 v-language:inner>mobileSortAndFilter_sortBy</h3>
                <div class="sort-switch">
                <template v-for="(singleSort, index) in sortOptions">
                    <input type="radio" :id="`option${index}`" v-model="sort" :key="`option${index}`"
                    name="switch" :value="singleSort.key">
                    <label :for="`option${index}`" :key="index">{{singleSort.value}}</label>
                </template>
                </div>
                </template>
            </div>
            <div class="filter-wrap" :class="$vuetify.breakpoint.xsOnly ? 'px-4' : 'px-5'" v-if="filterList && filterList.length">
                <!--<h3 class="" v-language:inner>mobileSortAndFilter_filterBy</h3>-->
                <div class="filter-sections">
                    <div class="filter-section" v-for="(singleFilter) in filterList" :key="singleFilter.id" :value="true">
                        <v-layout class="filter-header" slot="header">
                            <v-layout row align-center>
                                <div class="icon-wrapper">
                                    <slot :name="`${singleFilter.title}TitlePrefix`"></slot>
                                </div>
                                <slot name="headerTitle" :title="singleFilter.title">
                                    <div>
                                        <span class="filter-single-title">{{singleFilter.title}}</span>
                                    </div>
                                </slot>
                            </v-layout>
                        </v-layout>
                        <div class="filter-list">
                            <v-btn-toggle v-model="filtersSelected" multiple>
                                <v-btn v-for="(filterOption, index) in singleFilter.data"
                                       :key="(filterOption.key ? filterOption.key : index)"
                                       :active="filterOption.value"
                                       :id="filterOption.key"
                                       :value="`${filterOption.key}_${filterOption.value}_${singleFilter.id}`">
                                    {{filterOption.value ? filterOption.value : '' | capitalize}}
                                </v-btn>
                            </v-btn-toggle>
                            <slot :name="`${singleFilter.modelId}EmptyState`" v-if="singleFilter.data && singleFilter.data.length===0"></slot>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="bottom-bnt-wrap justify-end align-center">
            <v-btn class="apply elevation-0" @click="applyFilters"><span v-language:inner>mobileSortAndFilter_applyFilterBtn</span></v-btn>
        </div>
    </v-dialog>
</template>

<script src="./MobileSortAndFilter.js">
</script>
<style src="./MobileSortAndFilter.less" lang="less"></style>