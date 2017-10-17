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
                                <div class="cell-title" :class="'text-'+$route.name">{{item.title}}</div>
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
</template>
<script>
    export default {
        props: { item: { type: Object, required: true } }
    }
</script>
<style src="./ResultItem.less" lang="less" scoped></style>
