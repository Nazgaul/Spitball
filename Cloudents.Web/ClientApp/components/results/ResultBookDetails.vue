<template>
    <general-page :title="(pageData.details?pageData.details.title:'')"  :isLoading="isLoading">

                <v-list slot="data" v-if="pageData">
                    <result-book :item="pageData.details" :isDetails="true"></result-book>
                    <v-container grid-list-md>
                        <v-layout row wrap>
                            <v-flex>
                                <radio-list class="search" :values="page.filter" v-model="filter" :value="filterOptions"></radio-list>
                            </v-flex>
                            <v-flex>
                                <radio-list v-if="page.sort" :values="page.sort" model="sort" class="search sort" value="price"></radio-list>
                            </v-flex>
                        </v-layout>
                    </v-container>
                    <template v-for="(item,index) in filteredList">
                        <v-list-tile>
                            <v-list-tile-content>
                                <v-list-tile-sub-title>
                                    <component :is="'result-'+item.template" :item="item"></component>
                                </v-list-tile-sub-title>
                            </v-list-tile-content>
                        </v-list-tile>
                        <v-divider v-if="(index< filteredList.length - 1)"></v-divider>
                    </template>
                </v-list>

    </general-page>
</template>
<script>
    import { detailsMixin } from './mixins'
    export default {
        mixins: [detailsMixin]
    }
</script>