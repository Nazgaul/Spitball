<template>
    <div class="sort-filter-wrap">
        <template v-if="sortOptions.length">
            <h3>Sort</h3>
            <div class="sort-switch">
                <template v-for="(o, index) in sortOptions">
                    <input type="radio" :id="`option${index}`" @click="updateSort(o.id)" :key="`option${index}`"
                           name="switch" :value="o.id" :checked="sortVal?sortVal===o.id:index===0">
                    <label :for="`option${index}`" :key="index">{{o.name}}</label>
                </template>
            </div>
        </template>
        <div v-if="filterOptions && filterOptions.length">
            <h3>Filter</h3>
            <div class="filter-switch">
                <!--removed :value binding cause of error Vuetify 1.1.1-->
                <v-expansion-panel expand :value="0">
                    <v-expansion-panel-content v-for="(singleFilter, index) in filterList" :key="index">
                        <v-icon slot="actions" class="hidden-xs-only">sbf-chevron-down</v-icon>
                        <template slot="header">
                            <div class="icon-wrapper">
                                <slot :name="`${singleFilter.modelId}TitlePrefix`"></slot>
                            </div>

                            <slot name="headerTitle" :title="singleFilter.title">
                                <div>{{singleFilter.title}}</div>
                            </slot>
                        </template>
                        <div :class="['sort-filter',$route.path==='/ask'?'no-maxHeight':'']">
                            <div v-for="filterItem in singleFilter.data"
                                 :key="(filterItem.id ? filterItem.id : filterItem)" class="filter">
                                <input type="checkbox" :id="(filterItem.id ? filterItem.id : filterItem)"
                                       :checked="filterVal.find(i=>i.key===singleFilter.modelId && i.value===( filterItem.id ? filterItem.id.toString() : filterItem.toString()))"
                                       @change="updateFilter({
                                       id : singleFilter.modelId,
                                       val:(filterItem.id ? filterItem.id.toString() : filterItem),
                                       type : $event} )"/>

                                <label class="checkmark" :for="(filterItem.id ? filterItem.id : filterItem)"></label>
                                <label class="title-label" :title="filterItem.name ? filterItem.name : filterItem" :for="(filterItem.id ? filterItem.id : filterItem)">
                                    {{filterItem.name ? filterItem.name : filterItem | capitalize}}
                                </label>
                            </div>
                            <slot :name="`${singleFilter.modelId}EmptyState`"
                                  v-if="singleFilter.data && singleFilter.data.length===0"></slot>
                            <slot :name="`${singleFilter.modelId}ExtraState`" v-else></slot>
                        </div>
                    </v-expansion-panel-content>
                </v-expansion-panel>
            </div>
        </div>
    </div>
</template>
<script src="./SortAndFilter.js"></script>
<style src="./SortAndFilter.less" lang="less"></style>


