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
                        <v-tabs-item @click="changeActiveTab(1)" :href="'#tab-1'" :key="1">Balances</v-tabs-item>
                        <v-tabs-item @click="changeActiveTab(2)" :href="'#tab-2'" :key="2">Transaction</v-tabs-item>
                    </v-tabs-bar>
                </div>

                <v-tabs-items>
                    <v-tabs-content :key="'1'" :id="'tab-1'">
                        <v-flex xs12>
                            <v-data-table
                                    :headers="headers.balances"
                                    :items="items"
                                    hide-actions
                                    class="balance-table wallet-table">
                                <template slot="headerCell" slot-scope="props">
                                    <span :class="props.header.text+'-header table-header'">{{ props.header.text }}</span>
                                </template>
                                <template slot="items" slot-scope="props">
                                    <td class="text-xs-left">{{ props.item.type }}</td>
                                    <td class="text-xs-right">{{ props.item.points }} pt</td>
                                    <td class="text-xs-right bold">$ {{ props.item.value }}</td>
                                    <td class="text-xs-center" v-if="!$vuetify.breakpoint.xsOnly"><span v-if="props.item.cashOut" class="cash-out">Cash Out</span></td>
                                </template>
                            </v-data-table>
                            <div class="bottom-btn">
                                <button class="cash-out-big">Cash Out</button>
                            </div>
                        </v-flex>
                    </v-tabs-content>

                    <v-tabs-content :key="'2'" :id="'tab-2'" class="tab-padding">
                        <v-flex xs12>
                            <v-data-table
                                    :headers="headers.transactions"
                                    :items="items"
                                    :search="search"
                                    :pagination.sync="pagination"
                                    hide-actions
                                    class="transaction-table wallet-table">
                                <template slot="headerCell" slot-scope="props">
                                    <span :class="props.header.text+'-header table-header'">{{ props.header.text }}</span>
                                </template>
                                <template slot="items" slot-scope="props">
                                    <td class="text-xs-left">{{ props.item.date }}</td>
                                    <td class="text-xs-left">{{ props.item.action }}</td>
                                    <td class="text-xs-left" v-if="!$vuetify.breakpoint.xsOnly">{{ props.item.type }}</td>
                                    <td class="text-xs-right">{{ props.item.amount}} pt</td>
                                    <td class="text-xs-right bold" v-if="!$vuetify.breakpoint.xsOnly">{{ props.item.balance }} pt</td>
                                </template>
                            </v-data-table>
                            <div class="text-xs-center pt-2" :class="{'bottom-btn':$vuetify.breakpoint.xsOnly}">
                                <v-pagination total-visible=4 v-model="pagination.page" :length="pages"
                                              next-icon="sbf-arrow-right"
                                              prev-icon="sbf-arrow-right left"></v-pagination>
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
