<template functional>
    <v-layout row class="place-cell result-cell-content">
        <div class="img-wrap spitball-bg-food">
            <img :src="props.item.image" alt="" v-if="props.item.image">
            <food-default v-else class="defaultImage spitball-bg-food"></food-default>
        </div>
        <v-flex class="cell-data pa-0 ml-2">
            <v-container class="pa-0 full-height">
                <v-layout wrap column justify-content-space-between align-item-stretch class="full-height ma-0">
                    <v-flex class="pa-0">
                        <div class="cell-title-wrap">
                        <span class="cell-title">{{props.item.name}}</span></div>
                        <div class="rate mb-2">
                            <span class="rate-num">{{props.item.rating}}</span>
                            <star-rating :inline="true" :star-size="12"
                                         :read-only="true" :show-rating="false"
                                         active-color="#f6a623"
                                         inactive-color="#ddd"
                                         :increment="0.1"
                                         :rating="props.item.rating"></star-rating>
                        </div>
                        <div class="address-wrap">
                            <span class="address">{{props.item.address}}</span>
                        </div>
                    </v-flex>
                    <div class="pa-0 bottom" v-if="props.item.open">Open Now</div>
                    <div class="hooked-container" v-if="props.item.hooked">
                        <hooked-logo class="logo"></hooked-logo>
                        <div class="text">Discount</div>
                    </div>
                </v-layout>
            </v-container>
        </v-flex>
    </v-layout>
</template>

<style lang="less" src="./foodCell.less"></style>