<template>
    <div>
        <div class="title">{{pageData.details.title}}</div>
        <result-book :item="pageData.details" :isDetails="true"></result-book>
        <v-container grid-list-md>
            <v-layout row wrap>
                <v-flex>
                    <radio-list class="search" :values="page.filter" @click="$_changeFilter" model="filter" :value="filterOptions"></radio-list>
                </v-flex>
                <v-flex>
                    <radio-list v-if="page.sort" :values="page.sort" @click="$_updateSort" model="sort" class="search sort" :value="$_defaultSort(page.sort[0].id)"></radio-list>
                </v-flex>
            </v-layout>
        </v-container>
        <scroll-list :loadMore="!isEmpty" v-if="pageData.data" @scroll="value => {pageData.data=pageData.data.concat(value) }">
            <v-list>
                <template v-for="(item,index) in pageData.data">
                    <v-list-tile>
                        <v-list-tile-content>
                            <v-list-tile-sub-title>
                                <component :is="'result-'+item.template" :item="item"></component>
                            </v-list-tile-sub-title>                        </v-list-tile-content>
                    </v-list-tile>
                    <v-divider></v-divider>
                </template>    
               
    </v-list>
</scroll-list>
    </div>
</template>
<script>
    import { pageMixin } from './mixins'
    export default {
        mixins: [pageMixin]
    }
</script>