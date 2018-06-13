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
                            <table class="custom-table balances" cellspacing="0" cellpadding="0">
                                <thead>
                                    <tr>
                                        <th width="38px"></th>
                                        <th width="380px"></th>
                                        <th width="110px"><strong>Points</strong></th>
                                        <th width="140px"><strong>$ value</strong></th>
                                        <th width="189px"></th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <tr>
                                        <td class="number">1</td>
                                        <td>Awarded</td>
                                        <td>250 pt</td>
                                        <td>
                                            <strong>$ 25.00</strong>
                                        </td>
                                        <td></td>
                                    </tr>
                                    <tr>
                                        <td class="number">2</td>
                                        <td>Earned (Withdrawable)</td>
                                        <td>987 pt</td>
                                        <td>
                                            <strong>$ 98.70</strong>
                                        </td>
                                        <td>
                                            <button class="cash-out">Cash Out</button>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="number">3</td>
                                        <td>Pending</td>
                                        <td>10 pt</td>
                                        <td>
                                            <strong>$ 1.00</strong>
                                        </td>
                                        <td></td>
                                    </tr>
                                    <tr class="total">
                                        <td></td>
                                        <td>Total</td>
                                        <td>
                                            <strong>1.244 pt</strong>
                                        </td>
                                        <td>
                                            <strong>$ 124.40</strong>
                                        </td>
                                        <td></td>
                                    </tr>
                                    <tr>
                                        <td></td>
                                        <td>Staked</td>
                                        <td>-25 pt</td>
                                        <td>
                                            <strong>-$ 2.50</strong>
                                        </td>
                                        <td></td>
                                    </tr>
                                </tbody>
                            </table>
                            <div class="bottom-btn">
                                <button class="cash-out-big">Cash Out</button>
                            </div>
                        </v-flex>
                    </v-tabs-content>

                    <v-tabs-content :key="'2'" :id="'tab-2'" class="tab-padding">
                        <v-flex xs12>
                                 <!-- <v-data-table
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
                                    </v-data-table> -->

                                    <table class="custom-table transaction" cellspacing="0" cellpadding="0">
                                        <thead>
                                            <tr>
                                                <th width="38px"></th>
                                                <th width="110px">Date</th>
                                                <th width="110px">Action</th>
                                                <th width="110px">Type</th>
                                                <th width="110px">Amount</th>
                                                <th width="110px"><strong>Balance</strong></th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            <tr v-for="(item,key) in items">
                                                <td class="number">{{key+1}}</td>
                                                <td>{{item.fat}}</td>
                                                <td>{{item.carbs}}</td>
                                                <td>{{item.protein}}</td>
                                                <td>{{item.sodium}} pt</td>
                                                <td>
                                                    <strong>{{item.sodium}} pt</strong>
                                                </td>
                                            </tr>                                            
                                        </tbody>
                                    </table>
                                    <div class="text-xs-center pt-2" :class="{'bottom-btn':$vuetify.breakpoint.xsOnly}">
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
