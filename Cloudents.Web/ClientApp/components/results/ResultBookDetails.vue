<template>
    <general-page v-if="pageData.details" :title="(pageData.details?pageData.details.title:'')" :filterSelection="[]">
        <app-menu slot="verticalNavbar" :$_calcTerm="$_calcTerm"></app-menu>
        <template slot="options" v-if="page">
            <sort-and-filter :sortOptions="sortOptions" :sortCallback="$_updateSort" :sortVal="sortVal"
                             :filterOptions="filterOptions" :filterCallback="$_updateFilter" :filterVal="[filter]"
                             ></sort-and-filter>
        </template>
        <div slot="data" class="book-detail elevation-1">
            <result-book :item="pageData.details" :isDetails="true"></result-book>

            <div class="ma-2">
                <a :href="item.link" :target="$vuetify.breakpoint.xsOnly?'_self':'_blank'" class="price-line" v-for="(item,index) in filteredList" :key="index">
                    <v-layout row justify-space-between>
                        <v-flex xs3 class="text-xs-left">
                            <img v-if="item.image" :src="item.image" />
                            <span v-else>{{item.name}}</span>
                        </v-flex>
                        <v-flex xs3 class="text-xs-center">
                            {{item.condition}}
                        </v-flex>
                        <v-flex xs3 class="text-xs-right">
                            <div>
                                ${{item.price}}
                            </div>
                        </v-flex>

                    </v-layout>
                    <v-divider></v-divider>
                </a>
                <!--<table>
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
                </table>-->
            </div>
        </div>
    </general-page>
</template>
<script src="./ResultBookDetails.js"></script>
<style src="./ResultBookDetail.less" lang="less"></style>