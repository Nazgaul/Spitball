<template>
    <div class="sort-filter-wrap">
        <template v-if="sortOptions &&  sortOptions.length">
            <h3 v-language:inner>sortAndFilter_sort</h3>
            <div class="sort-switch">
                <template v-for="(singleSort, index) in sortOptions">
                    <input type="radio" :id="`option${index}`"
                           @click="updateSort(singleSort)"
                           :key="`option${index}`"
                           name="switch"
                           :value="singleSort"
                           :checked="sortVal ? sortVal === singleSort : index===0">
                    <label :for="`option${index}`"
                           :key="index">{{singleSort}}</label>
                </template>
            </div>
        </template>
        <div v-if="filterList && filterList.length">
            <h3 v-language:inner>sortAndFilter_filter</h3>
            <div class="filter-switch">
                <v-expansion-panel expand v-model="panelList">
                    <v-expansion-panel-content v-for="(singleFilter, index) in filterList" :key="index">
                        <v-icon slot="actions" class="hidden-xs-only">sbf-chevron-down</v-icon>
                        <template slot="header">
                            <!--<h3>{{panelList}}</h3>-->
                            <div class="icon-wrapper">
                                <slot :name="`${singleFilter.title}TitlePrefix`"></slot>
                            </div>

                            <slot name="headerTitle" :title="singleFilter.title">
                                <div>{{singleFilter.title}}</div>
                            </slot>
                        </template>
                        <div :class="['sort-filter']">
                            <div v-for="filterItem in singleFilter.data"
                                 :key="(filterItem.id ? filterItem.id : filterItem)" class="filter">
                                <input type="checkbox" :id="(filterItem.id ? filterItem.id : filterItem)"
                                       :checked="isChecked(singleFilter, filterItem)"
                                       @change="updateFilter({
                                       id : singleFilter.id,
                                       val: filterItem,
                                       event : $event} )"/>

                                <label class="checkmark" :for="(filterItem.id ? filterItem.id : filterItem)"></label>
                                <label class="title-label" :title="filterItem.name ? filterItem.name : filterItem | capitalize"
                                       :for="(filterItem.id ? filterItem.id : filterItem)">
                                    {{filterItem.name ? filterItem.name : filterItem | capitalize}}
                                </label>
                            </div>
                            <slot :name="`${singleFilter.id}EmptyState`"
                                  v-if="singleFilter.data && singleFilter.data.length===0"></slot>
                            <slot :name="`${singleFilter.id}ExtraState`" v-else></slot>
                        </div>
                    </v-expansion-panel-content>
                </v-expansion-panel>
            </div>
        </div>
    </div>
</template>
<script src="./SortAndFilter.js"></script>
<style src="./SortAndFilter.less" lang="less"></style>


