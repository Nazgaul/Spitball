<template>
    <v-flex offset-xs1 class="mt-5">
        <result-personalize v-show="isfirst" :show="showSearch" v-if="isfirst||showCourses"></result-personalize>
        <div class="loader" v-if="isLoading">
            <v-progress-circular indeterminate v-bind:size="50" color="amber"></v-progress-circular>
        </div>
        <v-container v-else>
            <v-layout row>
                <v-flex class="sec-result">
                    <h5>
                        <span v-if="isEmpty" class="empty" v-html="page.emptyText.replace('$subject', term)"></span>
                        <span v-else v-html="titleText"></span> {{dynamicHeader}}
                    </h5>
                    <slot name="options" v-if="pageData.data">
                        <v-container class="pa-0 mb-3">
                            <v-layout row>
                                <radio-list class="search" :values="page.filter" @click="$_changeFilter" model="filter" :value="filterOptions"></radio-list>
                                <div class="s-divider"></div>
                                <radio-list v-if="page.sort" :values="page.sort" @click="$_updateSort" model="sort" class="search sort" :value="$_defaultSort(page.sort[0].id)"></radio-list>
                            </v-layout>
                            <radio-list :values="subFilters" @click="$_changeSubFilter" class="sub-search" model="subFilter" :value="subFilter"></radio-list>
                        </v-container>
                    </slot>
                </v-flex>
            </v-layout>
            <v-layout row>
                <v-flex class="sec-result">
                    <slot name="data">
                        <scroll-list :loadMore="!isEmpty" v-if="items" @scroll="value => {items=items.concat(value) }" :token="pageData.token">
                            <v-container fluid grid-list-sm v-for="(item,index) in items" :key="index" @click="(hasExtra?selectedItem=item.placeId:'')">
                                <component :is="'result-'+item.template" :item="item" :key="index" class="cell"></component>
                                <div v-if="$_showSuggest(index)&&flowNode" v-for="(child,flowIndex) in flowNode.children" class="suggest" @click="$_updateCurrentFlow(flowIndex)">
                                    <suggest-card :name="child.name"></suggest-card>
                                </div>
                            </v-container>
                            <div v-if="!items.length&&flowNode" v-for="(childo,index1) in flowNode.children" class="suggest" @click="$_updateCurrentFlow(index1)">
                                {{index1}}
                                {{childo.name}}
                                <suggest-card :name="childo.name"></suggest-card>
                            </div>
                        </scroll-list>
                    </slot>
                    <slot :name="name" v-if="hasExtra&&!isEmpty">
                        <component :is="name+'-extra'" :place="selectedItem"></component>
                    </slot>
                </v-flex>
                <v-flex class="ml-1 mt-1 hidden-md">

                    <adsense ad-client="ca-pub-1215688692145777"
                             ad-slot="3866041406"
                             ad-style="display: block; width:336px; height:280px;"
                             ad-format="auto" class="mb-2">
                    </adsense>
                    <adsense ad-client="ca-pub-1215688692145777"
                             ad-slot="5547053037"
                             ad-style="display: block; width:336px;height:280px">
                    </adsense>
                </v-flex>
            </v-layout>
        </v-container>
    </v-flex>
</template>
<script>
    import { pageMixin } from './mixins'
    export default {
        mixins: [pageMixin]
    }
</script>
<style src="./Result.less" lang="less">
</style>