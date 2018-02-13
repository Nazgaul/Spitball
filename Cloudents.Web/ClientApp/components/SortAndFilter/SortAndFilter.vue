<template>
    <div class="sort-filter-wrap">
        <template v-if="sortOptions.length">
            <h3>Sort</h3>
            <div class="sort-switch">
                <template v-for="(o,index) in sortOptions">
                    <input type="radio" :id="`option${index}`" @click="updateSort(o.id)"
                           name="switch" :value="o.id" :checked="sortVal?sortVal===o.id:index===0">
                    <label :for="`option${index}`">{{o.name}}</label>
                </template>
            </div>
        </template>
        <div v-if="filterOptions&&filterOptions.length">
            <h3>Filter</h3>
            <div class="filter-switch">
                <v-expansion-panel :value="true" expand>
                    <v-expansion-panel-content v-for="k in filterOptions" :key="k.modelId" :value="true">
                        <v-icon slot="actions" class="hidden-xs-only">sbf-chevron-down</v-icon>
                        <template slot="header">
                            <div class="icon-wrapper"><slot :name="`${k.modelId}TitlePrefix`"></slot></div>
                            <slot name="headerTitle" :title="k.title">
                                <div>{{k.title}}</div>
                            </slot>
                        </template>
                        <div class="sort-filter">
                            <div v-for="s in k.data" :key="(s.id?s.id:s)" class="filter">
                                <input type="checkbox" :id="(s.id?s.id:s)" :checked="filterVal.find(i=>i.key===k.modelId&&i.value===(s.id?s.id.toString():s.toString()))"
                                       @change="updateFilter({id:k.modelId,val:(s.id?s.id.toString():s),type:$event})" />

                                <label class="checkmark" :for="(s.id?s.id:s)"></label>
                                <label class="title-label" :title="s.name?s.name:s" :for="(s.id?s.id:s)">
                                    {{s.name?s.name:s | capitalize}}
                                </label>
                            </div>
                            <slot :name="`${k.modelId}EmptyState`" v-if="k.data&&k.data.length===0"></slot>
                            <slot :name="`${k.modelId}ExtraState`" v-else></slot>
                        </div>
                    </v-expansion-panel-content>
                </v-expansion-panel>
            </div>
        </div>
    </div>
</template>
<script src="./SortAndFilter.js"></script>
<style src="./SortAndFilter.less" lang="less"></style>


