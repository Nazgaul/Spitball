<template>
    <v-dialog v-model="value" fullscreen content-class="white filter-dialog">
        <dialog-toolbar height="48" toolbarTitle="Filter & Sort" :backAction="$_backAction">
            <v-btn slot="rightElement" flat class="clear-btn" @click="resetFilters">Clear all</v-btn>
        </dialog-toolbar>
 
        <v-btn class="apply elevation-0" fixed color="color-blue" @click="applyFilters">Apply Filters</v-btn>
        <div class="sort-filter-wrap">
            <template v-if="sortOptions">
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
        <div v-if="filterOptions">
            <h3>Filter</h3>
            <div class="filter-switch" v-if="filterOptions">
                <v-expansion-panel :value="true">
                    <v-expansion-panel-content v-for="k in filterOptions" :key="k.modelId" :value="true">
                        <v-icon slot="actions" class="hidden-xs-only">sbf-chevron-down</v-icon>
                        <template slot="header">
                            <div class="icon-wrapper"><slot :name="`${k.modelId}TitlePrefix`"></slot></div>
                            <slot name="headerTitle" :title="k.title">
                                <div>{{k.title}}</div>
                            </slot>
                            <slot :name="`${k.modelId}MobileExtraState`"></slot>
                        </template>
                        <div class="sort-filter">
                            <div v-for="s in k.data" :key="(s.id?s.id:s)" class="filter">
                                <input type="checkbox" :id="(s.id?s.id:s)" v-model="filters[k.modelId]" :value="(s.id?s.id:s)"/>

                                <span class="checkmark"></span>
                                <label :title="s.name?s.name:s" :for="(s.id?s.id:s)">
                                    {{s.name?s.name:s | capitalize}}
                                </label>
                            </div>
                            <slot :name="`${k.modelId}EmptyState`" v-if="k.data&&k.data.length===0"></slot>
                        </div>
                    </v-expansion-panel-content>
                </v-expansion-panel>
            </div>
        </div>
    </v-dialog>
</template>

<script src="./MobileSortAndFilter.js">
</script>
<style src="./SortAndFilter.less" lang="less"></style>