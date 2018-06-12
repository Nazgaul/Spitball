<template>
    <div class="wallet-page">        

        <div class="flex">
            <v-tabs>
                <div class="header">
                    <span class="header-title">My Wallet</span>
                    <button class="back-button" @click="$router.go(-1)">
                        <v-icon right>sbf-close</v-icon>
                    </button>
                    <v-tabs-bar fixed>
                        <v-tabs-slider color="white"></v-tabs-slider>
                        <v-tabs-item @click="activeTab = 1" :href="'#tab-1'" :key="1">Balances</v-tabs-item>
                        <v-tabs-item @click="activeTab = 2" :href="'#tab-2'" :key="2">Transaction</v-tabs-item>
                    </v-tabs-bar>
                </div>   

                <v-tabs-items>
                     <v-tabs-content :key="'1'" :id="'tab-1'">
                        <v-flex xs12>
                            <p>Tab1</p>
                        </v-flex>
                    </v-tabs-content>

                    <v-tabs-content :key="'2'" :id="'tab-2'" class="tab-padding">
                        <v-flex xs12>
                                 <v-data-table
                                    v-bind:headers="headers"
                                    v-bind:items="items"
                                    v-bind:search="search"
                                    v-bind:pagination.sync="pagination"
                                    hide-actions
                                    >
                                        <template slot="headerCell" slot-scope="props">
                                            {{ props.header.text }}
                                        </template>

                                        <template slot="items" slot-scope="props">
                                            <td>{{ props.item.name }}</td>
                                            <td  class="text-xs-right">{{ props.item.calories }}</td>
                                            <td  class="text-xs-right">{{ props.item.fat }}</td>
                                            <td  class="text-xs-right">{{ props.item.carbs }}</td>
                                            <td  class="text-xs-right">{{ props.item.protein }}</td>
                                            <td  class="text-xs-right">{{ props.item.sodium }}</td>
                                        </template>
                                    </v-data-table>
                                    <div class="text-xs-center pt-2">
                                        <v-pagination total-visible=4 v-model="pagination.page" :length="pages" next-icon="sbf-arrow-right" prev-icon="sbf-arrow-right left"></v-pagination>
                                    </div>
                        </v-flex>
                    </v-tabs-content>

                   

                </v-tabs-items>

            </v-tabs>
        </div>

    </div>
</template>

<style src="./wallet.less" lang="less"></style>
<script src="./wallet.js"></script>
