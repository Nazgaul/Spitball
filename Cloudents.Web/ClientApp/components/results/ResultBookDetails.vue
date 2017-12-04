<template>
    <general-page :title="(pageData.details?pageData.details.title:'')">
        <!--v-if="pageData"-->
        <template slot="options" v-if="page">
        </template>
        <div slot="data" class="book-detail elevation-1">
            <result-book :item="pageData.details" :isDetails="true"></result-book>
            <!--<div class="sort-filter">
                    <radio-list class="search" :values="page.filter" v-model="filter" :value="filterSelection"></radio-list>
                    <radio-list v-if="page.sort" :values="page.sort" model="sort" class="search sort" value="price"></radio-list>
            </div>-->
            <div class="ma-2">
                <table>
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
        </div>
    </general-page>
</template>
<script src="./ResultBookDetails.js"></script>
<style src="./ResultBookDetail.less" lang="less"></style>