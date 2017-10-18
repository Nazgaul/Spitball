<template v-once>
    <a class="elevation-1 d-block pa-2 place-cell">
        <v-container class="pa-2">
            <v-layout row>
                <div class="img-wrap">
                    <img :src="item.image" alt="">
                </div>
                <v-flex class="pa-0 ml-2">
                    <v-container class="pa-0 full-height">
                        <v-layout wrap column justify-content-space-between align-item-stretch class="full-height ma-0">
                            <v-flex class="pa-0">
                                <div class="cell-title" >{{item.name}}</div>
                                <div class="rate">
                                    <span>{{item.rating}}</span>
                                    
                                    <star-rating :inline="true" :star-size="12" 
                                                 :read-only="true" :show-rating="false" 
                                                 active-color="#f6a623"
                                                 inactive-color="#ddd"
                                                 :increment="0.1"
                                                 :rating="item.rating"></star-rating>
                                </div>
                                <div class="address">{{item.address}}</div>
                            </v-flex>
                            <div class="pa-0 bottom" v-if="item.open">Open Now</div>
                        </v-layout>
                    </v-container>
                </v-flex>
            </v-layout>
        </v-container>
    </a>
    <!--<div class="wrapper">
            <div class="container-box">
                <div class="box-main box-food">
                    <div class="img">
                        <span class="up">
                            <img :src="item.image" alt="">
                        </span>
                    </div>
                    <div class="box-wrapper">
                        <div class="title">{{item.name}}</div>
                        <div class="rate">
                            {{item.rating}} <star-rating :inline="true" :star-size="20" :read-only="true" :show-rating="false" :increment="0.1" :rating="item.rating"></star-rating>
                        </div>
                        <p>
                            {{item.address}}
                        </p>
                        <div class="bottom" v-if="item.open">Open Now</div>
                    </div>
                </div>
            </div>
        </div>-->
</template>
<script>
    import StarRating from 'vue-star-rating'
    export default {
        props: { item: { type: Object, required: true } },
        components: { StarRating }
    }
</script>
<style src="./ResultFood.less" lang="less"></style>
