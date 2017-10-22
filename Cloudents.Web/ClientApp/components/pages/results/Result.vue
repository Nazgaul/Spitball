<template>
    <v-flex offset-xs1>
        <div class="loader" v-if="isLoading">
            <v-progress-circular indeterminate v-bind:size="50" color="amber"></v-progress-circular>
        </div>
        <div class="sec-result" v-else>
            <h5>
                <span v-if="isEmpty" class="empty" v-html="page.emptyText.replace('$subject', term)"></span>
                <span v-else v-html="page.title"></span>{{dynamicHeader}}
            </h5>
            <slot name="options">
                <v-container class="pa-0 mb-3">
                    <v-layout row>
                        <radio-list class="search" :values="page.filter" @click="$_changeFilter" model="filter" :value="filterOptions"></radio-list>
                        <div class="s-divider"></div>
                        <radio-list v-if="page.sort" :values="page.sort" @click="$_updateSort" model="sort" class="search sort" :value="$_defaultSort(page.sort[0].id)"></radio-list>
                    </v-layout>
                    <radio-list :values="subFilters" @click="$_changeSubFilter" class="sub-search" model="subFilter" :value="subFilter"></radio-list>
                </v-container>
            </slot>
            <v-container class="pa-0">
                <v-layout row>
                    <slot name="data">
                        <scroll-list :loadMore="!isEmpty" v-if="pageData.data" @scroll="value => {pageData.data=pageData.data.concat(value) }">
                            <v-container fluid grid-list-sm v-for="(item,index) in pageData.data" :key="index">
                                <component :is="'result-'+item.template" :item="item" :key="index" class="cell"></component>
                            </v-container>
                        </scroll-list>
                    </slot>
                    <div class="pa-2" style="width:320px; height:240px;">
                        <img src="http://lorempixel.com/320/240/" />
                        <img src="http://lorempixel.com/320/240/" />

                        <adsense ad-client="ca-pub-1215688692145777"
                                 ad-slot="3866041406"
                                 ad-style="display: block; width:320px; height:240px;"
                                 ad-format="auto">
                        </adsense>
                        <!--<adsense ad-client="ca-pub-1215688692145777"
                                 ad-slot="3866041406"
                                 ad-style="display: block"
                                 ad-format="auto">
                        </adsense>-->


                    </div>
                </v-layout>
            </v-container>

            <!--<slot name="emptyState" v-show="isEmpty"></slot>-->

        </div>
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
<style lang="less">
    @import "../../../mixin.less";

    .overlay {
        top: 128px;
        background: rgba(0,0,0,0.5);
        pointer-events: all;
    }

    .loader {
        .inTheMiddle();
    }
</style>