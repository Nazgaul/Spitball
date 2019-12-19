<template>
    <div class="wallet-page">

        <div class="flex data-section">
            <div class="header ">
                <span class="header-title" v-language:inner>wallet_My_Wallet</span>
                <button sel="close_wallet" class="back-button wallet" @click="$router.go(-1)">
                    <v-icon right>sbf-close</v-icon>
                </button>
            </div>
            <v-tabs v-if="!cashOut" >
                <v-tab sel="balances" @click="changeActiveTab(1)" :href="'#tab-1'" :key="1"><span v-language:inner>wallet_Balances</span>  </v-tab>
                <v-tab sel="transactions" @click="changeActiveTab(2)" :href="'#tab-2'" :key="2"><span v-language:inner>wallet_Transaction</span> </v-tab>
                <v-tab sel="cashout" @click="changeActiveTab(3)" :href="'#tab-3'" :key="3"><span v-language:inner>wallet_Cash_Out</span> </v-tab>
                <v-tab-item :key="'1'" :value="'tab-1'" v-if="activeTab === 1">
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
                                <td class="text-xs-left">{{ props.item.name }}</td>
                                <td class="text-xs-left">{{ props.item.points | currencyLocalyFilter}}</td>
                                <td class="text-xs-left bold" :style="props.item.value < 0 ? `direction:ltr;` : ''">{{ props.item.value | currencyFormat(props.item.symbol) }}</td>
                            </template>
                        </v-data-table>
                    </v-flex>
                </v-tab-item>

                <v-tab-item :key="'2'" :value="'tab-2'" class="tab-padding" v-if="activeTab === 2">
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
                                <td class="text-xs-left" v-if="!$vuetify.breakpoint.xsOnly">{{ props.item.type }}</td>
                                <td class="text-xs-left">{{ props.item.amount | currencyLocalyFilter}}</td>
                                <td class="text-xs-left bold"  v-if="!$vuetify.breakpoint.xsOnly">{{ props.item.balance | currencyLocalyFilter }}</td>
                            </template>
                        </v-data-table>
                    </v-flex>
                    <div class="bottom-area" v-if="!cashOut">
                        <v-pagination total-visible=7 v-model="pagination.page" :length="pages"
                                      :next-icon="`sbf-arrow-right ${ isRtl ? 'left': '' }`"
                                      :prev-icon="`sbf-arrow-right ${ isRtl ? '': 'left' }`"></v-pagination>
                    </div>
                </v-tab-item>
                <v-tab-item :key="'3'" :value="'tab-3'" class="tab-padding" v-if="activeTab === 3">
                    <div class="cash-out-wrapper">
                        <div class="text-wrap">
                            <!--<div class="main-text" v-language:inner>wallet_more_SBL_more_valuable</div>-->
                            <div class="points-text">
                    <span>
                        <span v-language:inner>wallet_You_have</span>
                                <bdi>
                        <span>
                            {{Math.round(accountUser.balance)}}
                            <!-- {{calculatedEarnedPoints ? `${calculatedEarnedPoints.toLocaleString(undefined,
                            { minimumFractionDigits: 2, maximumFractionDigits: 2 })} ` || '0.00' : '0.00'}} -->

                            <span v-language:inner="'cashoutcard_SBL'"/>
                            </span>
                                    </bdi>
                        <span v-language:inner>wallet_you_have_redeemable_sbl</span>
                    </span>
                            </div>
                        </div>
                        <cash-out-card class="cash-out-option" v-for="(cashOutOption,index) in cashOutOptions"
                                       :key="index"
                                       :points-for-dollar="cashOutOption.pointsForDollar"
                                       :cost="cashOutOption.cost"
                                       :image="cashOutOption.image"
                                       :available="accountUser.balance >= cashOutOption.cost"
                                       :updatePoint="recalculate">
                        </cash-out-card>
                    </div>
                </v-tab-item>
            </v-tabs>

        </div>
        <div class="cash-out-wrap" v-if="active==='tab-1'">
            <div class="cash-out">
                <span v-language:inner>wallet_Cash_Out</span>
                <div class="btn-wrap">
                    <div class="cash-out-val-wrap">
                        <span class="cash-out-value"><span v-language:inner>wallet_currency</span>{{cash}}</span>
                    </div>
                    <div class="button-wrap">
                        <button class="cash-out-btn" @click="gotToCashOutTab" v-language:inner>wallet_Cash_Out</button>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<style src="./wallet.less" lang="less"></style>
<script src="./wallet.js"></script>
