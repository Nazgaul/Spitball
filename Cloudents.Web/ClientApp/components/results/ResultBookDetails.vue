<template>
    <general-page :title="(pageData.details?pageData.details.title:'')">
        <app-menu slot="verticalNavbar" :$_calcTerm="$_calcTerm"></app-menu>
        <template slot="options" v-if="page">
            <sort-and-filter :sortOptions="sortOptions" :sortCallback="$_updateSort" :sortVal="sortVal" :isExtended="false"
                             :filterOptions="filterOptions" :filterCallback="$_updateFilter" :filterVal="[filter]"
                             :version="$version"></sort-and-filter>
        </template>
        <div slot="data" class="book-detail elevation-1">
            <result-book :item="pageData.details" :isDetails="true"></result-book>
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