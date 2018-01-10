<template functional>
    <div class="sort-filter-wrap">
        <template v-if="props.sortOptions">
            <h3>Sort</h3>
            <div class="sort-switch">
                <template v-for="(o,index) in props.sortOptions">
                    <input type="radio" :id="`option${index}`" @click="props.sortCallback(o.id)"
                           name="switch" :value="o.id" :checked="props.sortVal?props.sortVal===o.id:index===0">
                    <label :for="`option${index}`">{{o.name}}</label>
                </template>
            </div>
        </template>
        <div v-if="props.filterOptions">
            <h3>Filter</h3>
            <div class="filter-switch" v-if="props.filterOptions">
                <v-expansion-panel :value="true" expand>
                    <v-expansion-panel-content v-for="k in props.filterOptions" :key="k.modelId" hide-actions :value="true">
                        <template slot="header">
                            <div class="icon-wrapper"><slot :name="`${k.modelId}TitlePrefix`"></slot></div>
                            <div>{{k.title}}</div>
                            <div class="header__icon hidden-xs-only">
                                <v-icon>sbf-chevron-down</v-icon>
                            </div>
                            <slot :name="`${k.modelId}MobileExtraState`"></slot>
                            <!--<button v-if="k.modelId === 'course'" class="edit-list hidden-sm-and-up" @click="props.$_openPersonalize()" type="button">Edit List</button>-->

                        </template>
                        <div class="sort-filter">
                            <div v-for="s in k.data" :key="(s.id?s.id:s)" class="filter">
                                <!--<v-checkbox :label="s.name?s.name:s" :inputValue="props.filterVal.includes(s.id?s.id:s.toString())"></v-checkbox>-->
                                <!--<sb-checkbox hide-details @change="props.filterCallback({id:k.modelId,val:(s.id?s.id:s),type:$event})" :inputValue="props.filterVal.includes(s.id?s.id:s.toString())">-->
                                    <!--<span slot="label" :title="s.name?s.name:s">-->
                                    <!--{{s.name?s.name:s | capitalize}}-->
                                    <!--</span>-->
                                <!--</sb-checkbox>-->
                                <!--<sb-checkbox :inputValue="true" :label="s.name?s.name:s"></sb-checkbox>-->
                                <!--<sb-checkbox :id="(s.id?s.id:s)" :checked="props.filterVal.includes(s.id?s.id:s.toString())"
                                       @change="props.filterCallback({id:k.modelId,val:(s.id?s.id:s),type:$event})"></sb-checkbox>-->
                                <input type="checkbox" :id="(s.id?s.id:s)" :checked="props.filterVal.includes(s.id?s.id:s.toString())"
                                       @change="props.filterCallback({id:k.modelId,val:(s.id?s.id:s),type:$event})" />

                                <span class="checkmark"></span>
                                <label :title="s.name?s.name:s" :for="(s.id?s.id:s)">
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
<style src="./SortAndFilter.less" lang="less"></style>


