<template>
    <a class="elevation-1 d-block" target="_blank" :href="item.url">
        <v-container class="pa-2">
            <v-layout row>
                <v-flex>
                    <img :class="[item.image?'border-'+$route.name:'bg-'+$route.name]" :src="item.image" alt="">
                </v-flex>
                <v-flex>
                    <v-container class="pa-0 full-height" >
                        <v-layout wrap column justify-content-space-between align-item-stretch class="full-height ma-0">
                            <v-flex class="pa-0" style="flex-grow:1">
                                <div class="title" :class="'text-'+$route.name">{{item.title}}</div>
                                <p>{{item.snippet}}</p>
                            </v-flex>
                            <v-flex class="pa-0 bottom" style="flex-grow:0">
                                Source: {{item.source}}
                            </v-flex>
                        </v-layout>
                    </v-container>
                </v-flex>
            </v-layout>
        </v-container>
    </a>
    <!--<a class="wrapper" target="_blank" :href="item.url">
        <div class="container-box">
            <div class="box-main" :class="[item.image?'box-mask-1':'box-mask-2']">
                <div class="img" :class="[item.image?'border-'+$route.name:'bg-'+$route.name]">
                    <span class="up">
                        <img :src="item.image" alt="">
                    </span>
                </div>
                <div class="box-wrapper">
                    <div class="title" :class="'text-'+$route.name">{{item.title}}</div>
                    <p>{{item.snippet}}</p>
                    <div class="bottom">Source: {{item.source}}</div>
                </div>
            </div>
        </div>
    </a>-->
</template>
<script>
    export default {
        props: { item: { type: Object, required: true } }
    }
</script>
<style src="./ResultItem.less" lang="less" scoped></style>
