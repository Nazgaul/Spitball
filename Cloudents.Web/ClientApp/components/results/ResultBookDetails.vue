<template>
    <general-page :title="(pageData.details?pageData.details.title:'')">
        <!--v-if="pageData"-->
        <div slot="data" class="book-detail elevation-1">
            <result-book :item="pageData.details" :isDetails="true"></result-book>
            <div class="sort-filter">
                    <radio-list class="search" :values="page.filter" v-model="filter" :value="filterOptions"></radio-list>
                    <!--<div class="s-divider"></div>-->
                    <radio-list v-if="page.sort" :values="page.sort" model="sort" class="search sort" value="price"></radio-list>
            </div>
            <div class="ma-2">
                <table >
                    <tr v-for="(item,index) in filteredList" class="border-row">
                        <td>
                            <img v-if="item.image" :src="item.image" />
                            <span v-else>{{item.name}}</span>
                        </td>
                        <td>
                            {{item.condition}}
                        </td>
                        <td>
                            <div>
                                ${{item.price}}
                            </div>
                        </td>
                    </tr>
                </table>
            </div>
            <!--<v-list v-for="(item,index) in filteredList">
                <v-list-tile>
                    <v-list-tile-content>
                        <v-list-tile-sub-title>
                            <component :is="'result-'+item.template" :item="item"></component>
                        </v-list-tile-sub-title>
                    </v-list-tile-content>
                </v-list-tile>
                <v-divider v-if="(index< filteredList.length - 1)"></v-divider>
            </v-list>-->
        </div>

    </general-page>
</template>
<script src="./ResultBookDetails.js"></script>
<style src="./ResultBookDetail.less" lang="less"></style>