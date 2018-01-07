<template functional>
    <v-layout row class="place-cell">
        <div class="img-wrap spitball-bg-food">
            <img :src="props.item.image" alt="" v-if="props.item.image">
            <food-default v-else class="defaultImage spitball-bg-food"></food-default>
        </div>
        <v-flex class="pa-0 ml-2">
            <v-container class="pa-0 full-height">
                <v-layout wrap column justify-content-space-between align-item-stretch class="full-height ma-0">
                    <v-flex class="pa-0">
                        <div class="cell-title">{{props.item.name}}</div>
                        <div class="rate mb-2">
                            <span>{{props.item.rating}}</span>
                            <star-rating :inline="true" :star-size="12"
                                         :read-only="true" :show-rating="false"
                                         active-color="#f6a623"
                                         inactive-color="#ddd"
                                         :increment="0.1"
                                         :rating="props.item.rating"></star-rating>
                        </div>
                        <div class="address">{{props.item.address}}</div>
                    </v-flex>
                    <div class="pa-0 bottom" v-if="props.item.open">Open Now</div>
                </v-layout>
            </v-container>
        </v-flex>
    </v-layout>
</template>
<style lang="less" src="./foodCell.less"></style>