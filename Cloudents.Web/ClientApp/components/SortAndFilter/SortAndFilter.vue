<template>
    <div class="sort-filter-wrap" :class="{'position-static': isEdgeRtl}">
        <template v-if="sortOptions &&  sortOptions.length">
            <h3 v-language:inner>sortAndFilter_sort</h3>
            <div class="sort-switch">
                <template v-for="(singleSort, index) in sortOptions">
                    <input type="radio" :id="`option${index}`"
                           @click="updateSort(singleSort.key)"
                           :key="`option${index}`"
                           name="switch"
                           :value="singleSort"
                           :checked="isRadioChecked(singleSort, index)">
                    <label :for="`option${index}`"
                           :key="index">{{singleSort.value}}</label>
                </template>
            </div>
        </template>
        <div v-if="filterList && filterList.length">
            <h3 v-language:inner>sortAndFilter_filter</h3>
            <div class="filter-switch">

                <!-- <v-expansion-panel expand v-model="panelList"> -->
                <v-expansion-panel expand readonly v-for="(singleFilter, index) in filterList" :key="index" 
                     v-model="panelList[index]">
                    <v-expansion-panel-content expand expand-icon="">
                        <!-- <v-icon slot="actions" class="hidden-xs-only">sbf-chevron-down</v-icon> -->

                        <template slot="header">
                            <div class="icon-wrapper">
                                <slot :name="`${singleFilter.title}TitlePrefix`"></slot>
                            </div>

                            <slot name="headerTitle" :title="singleFilter.title">
                                <div>{{singleFilter.title}} <span class="change-course-container" @click="openEditClass()" v-if="singleFilter.id.toLowerCase() === 'course'"><span class="edit-after-icon" v-language:inner>menuList_Change</span><v-icon class="edit-after-icon">sbf-edit-icon</v-icon></span></div>
                            </slot>
                        </template>
                        <div :class="['sort-filter']">
                            <div v-for="filterItem in singleFilter.data"
                                 :key="(filterItem.key ? filterItem.key : filterItem.value)" class="filter">
                                <input type="checkbox" :id="(filterItem.key ? filterItem.key : filterItem.value)"
                                       :checked="isChecked(singleFilter, filterItem)"
                                       @change="updateFilter({
                                       id : singleFilter.id,
                                       val: filterItem.key,
                                       name: filterItem.value,
                                       event : $event} )"/>

                                <label class="checkmark" :for="(filterItem.key ? filterItem.key : filterItem.value)"></label>
                                <label class="title-label" :title="filterItem.value ? filterItem.value : filterItem.key | capitalize"
                                       :for="(filterItem.key ? filterItem.key : filterItem.value)">
                                    {{filterItem.value ? filterItem.value : '' | capitalize}}
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


