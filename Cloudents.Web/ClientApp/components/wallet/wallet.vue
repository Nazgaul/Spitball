<template>
    <div class="wallet-page">

        <div class="flex data-section">
            <div class="header">
                <span class="header-title">My Wallet</span>
                <button class="back-button wallet" @click=" $router.go(-1)">
                    <v-icon right>sbf-close</v-icon>
                </button>
            </div>
            <v-tabs :grow="$vuetify.breakpoint.xsOnly" slider-color="white" v-model="active">
                <!--<div class="tabs-wrapper">-->
                <!--<v-tabs-bar fixed>-->
                <v-tab @click="changeActiveTab(1)" :href="'#tab-1'" :key="1">Balances</v-tab>
                <v-tab @click="changeActiveTab(2)" :href="'#tab-2'" :key="2">Transaction</v-tab>
                <v-tab @click="changeActiveTab(3)" :href="'#tab-3'" :key="3">Cash Out</v-tab>

                <!--</v-tabs-bar>-->
                <!--</div>-->

                <v-tab-item :key="'1'" :id="'tab-1'">
                    <!--<v-tabs-content :key="'1'" :id="'tab-1'">-->
                    <v-flex xs12>
                        <v-data-table
                                :headers="headers.balances"
                                :items="items"
                                :cash="cash"
                                hide-actions
                                class="balance-table wallet-table">
                            <template slot="headerCell" slot-scope="props">
                                <span :class="props.header.text+'-header table-header'">{{ props.header.text }}</span>
                            </template>
                            <template slot="items" slot-scope="props">
                                <td class="text-xs-left">{{ props.item.type }}</td>
                                <td class="text-xs-right">{{ props.item.points | currencyLocalyFilter}} SBL</td>
                                <td class="text-xs-right bold">$ {{ props.item.value }}</td>
                            </template>
                        </v-data-table>
                    </v-flex>
                </v-tab-item>
                <!--</v-tabs-content>-->

                <v-tab-item :key="'2'" :id="'tab-2'" class="tab-padding">
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
                                <td class="text-xs-left">{{ props.item.date | dateFromISO}}</td>
                                <td class="text-xs-left">{{ props.item.action }}</td>
                                <td class="text-xs-left" v-if="!$vuetify.breakpoint.xsOnly">{{ props.item.type }}
                                </td>
                                <td class="text-xs-right">{{ props.item.amount}} SBL</td>
                                <td class="text-xs-right bold" v-if="!$vuetify.breakpoint.xsOnly">{{
                                    props.item.balance }} SBL
                                </td>
                            </template>
                        </v-data-table>
                    </v-flex>
                    <div class="bottom-area" v-if="!cashOut">
                        <v-pagination total-visible=7 v-model="pagination.page" :length="pages"
                                      next-icon="sbf-arrow-right"
                                      prev-icon="sbf-arrow-right left"></v-pagination>
                    </div>
                </v-tab-item>

                <v-tab-item :key="'3'" :id="'tab-3'" class="tab-padding">
                    <div class="cash-out-wrapper">
                        <div class="text-wrap">
                            <div class="main-text">The more SBL you have, the more valuable they are.</div>
                            <div class="points-text">You have <span>{{ earnedPoints | currencyLocalyFilter}}</span>
                                redeemable SBL
                            </div>
                        </div>
                        <cash-out-card class="cash-out-option" v-for="(cashOutOption,index) in cashOutOptions"
                                       :key="index"
                                       :points-for-dollar="cashOutOption.pointsForDollar"
                                       :cost="cashOutOption.cost"
                                       :image="cashOutOption.image"
                                       :available="earnedPoints >= cashOutOption.cost">
                        </cash-out-card>
                    </div>
                </v-tab-item>
            </v-tabs>

        </div>
        <div class="cash-out-wrap" v-if="active==='tab-1'">
            <div class="cash-out">
                <span>Cash out</span>
                <div class="btn-wrap">
                    <div class="cash-out-val-wrap">
                        <span class="cash-out-value">$ {{cash}}</span>
                    </div>
                    <div class="button-wrap">
                        <button class="cash-out-btn" @click="gotToCashOutTab">Cash Out</button>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<style src="./wallet.less" lang="less"></style>
<script src="./wallet.js"></script>
