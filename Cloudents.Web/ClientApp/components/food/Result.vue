﻿
<template>
    <general-result ref="resultPage" name="food" :query="query" :params="params" :isPromo="isPromo" @dataUpdated="(val)=>selectedItem=val?val.placeId:''">
        <template slot-scope="props" slot="resultData">
            <v-flex class="result-cell elevation-1 mb-2" xs-12 v-for="(item,index) in props.items"  :key="index" @click="selectedItem=item.placeId" order-xs2 >
            <result-food :ref="`item${index}`" :item="item" :key="index" :index="index" class="cell"></result-food>
            </v-flex>
        </template>
        <template slot="rightSide">
        <food-map :place="selectedItem" v-if="selectedItem"></food-map>
            <span v-else></span>
        </template>
        <template slot="suggestCell"></template>
    </general-result>
</template>
<script>
    import GeneralResult from '../results/Result.vue'
    import FoodMap from './foodExtra.vue'
    import ResultFood from './ResultFood.vue'

    export default {
        components: { GeneralResult,FoodMap,ResultFood },
        data:()=>({selectedItem:""}),
        props:{name:String,params:{},query:{},isPromo:Boolean},
        beforeRouteUpdate(to,from,next){
            this.$refs.resultPage.updatePageData(to,from,next);
        },
        beforeRouteLeave(to, from, next) {
            this.$refs.resultPage.leavePage(to,from,next);
        }
    }
</script>
<style src="./Result.less" lang="less">
</style>
