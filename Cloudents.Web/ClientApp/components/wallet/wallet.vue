<template>
    <div class="wallet-page">

        <div class="flex data-section">
            <div class="header ">
                <span class="header-title" v-language:inner>wallet_My_Wallet</span>
                <button sel="close_wallet" class="back-button wallet" @click="$router.go(-1)">
                    <v-icon right>sbf-close</v-icon>
                </button>
            </div>
            <v-tabs v-if="!cashOut" background-color="#514f7d" color="#fff" hide-slider dark :prev-icon="''">
                <v-tab class="tabs_text" @click="changeActiveTab(1)" :href="'#tab-1'" :key="1"><span v-language:inner>wallet_Balances</span>  </v-tab>
                <v-tab class="tabs_text" @click="changeActiveTab(2)" :href="'#tab-2'" :key="2"><span v-language:inner>wallet_Transaction</span> </v-tab>
                <v-tab class="tabs_text" @click="changeActiveTab(3)" :href="'#tab-3'" :key="3"><span v-language:inner>wallet_Cash_Out</span> </v-tab>
                <v-tab-item :key="'1'" :id="'tab-1'" v-if="activeTab === 1">
                    <v-flex xs12>
                        <v-data-table
                                :headers="headers.balances"
                                :items="items"
                                :cash="cash"
                                hide-default-footer
                                hide-default-header
                                class="balance-table wallet-table">
                            <template v-slot:header="{props}">
                                <thead>
                                    <tr>
                                        <th v-for="(header, index) in props.headers" :key="index">
                                            {{header.text }}
                                        </th>
                                    </tr>
                                </thead>
                                <span :class="props.headers+'-header table-header'"></span>
                            </template>
                            <template v-slot:item="{item}">
                                <tr>
                                    <td class="text-left">{{ item.name }}</td>
                                    <td class="text-left">{{ item.points | currencyLocalyFilter}}</td>
                                    <td class="text-left bold" :style="item.value < 0 ? `direction:ltr;` : ''">{{ item.value | currencyFormat(item.symbol) }}</td>
                                </tr>
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
                                :options.sync="pagination"
                                hide-default-footer
                                hide-default-header
                                class="transaction-table wallet-table">
                            <template v-slot:header="{props}">
                                <thead>
                                    <tr>
                                        <th v-for="(header, index) in props.headers" :key="index">
                                            {{header.text }}
                                        </th>
                                    </tr>
                                </thead>
                            </template>
                            <template v-slot:item="{item}">
                                <tr>
                                    <td class="text-left">{{ item.date | dateFromISO}}</td>
                                    <td class="text-left">{{ item.action }}</td>
                                    <td class="text-left">{{ item.type }}</td>
                                    <td class="text-left">{{ item.amount | currencyLocalyFilter}}</td>
                                    <td class="text-left bold">{{ item.balance | currencyLocalyFilter }}</td>
                                </tr>
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
