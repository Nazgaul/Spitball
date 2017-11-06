<template>
    <a class="elevation-1 d-block" target="_blank" :href="url" @click="(isSpitball?$_spitball($event):'')">
        <v-container class="pa-2">
            <v-layout row fluid>
                <v-flex class="img-wrap mr-2 pa-0" :class="['border-'+$route.path.slice(1)]">
                    <img  :src="item.image" alt="">
                </v-flex>
                <v-flex class="right-section">
                    <v-container class="pa-0 full-height" >
                        <v-layout  wrap column justify-content-space-between align-item-stretch class="full-height ma-0">
                            <v-flex class="pa-0" style="flex-grow:1">
                                <div class="cell-title" :class="'text-'+$route.path.slice(1)">{{item.title}}</div>
                                <p>{{item.snippet}}</p>
                            </v-flex>
                            <v-flex class="pa-0 bottom">
                                Source: {{item.source}}
                            </v-flex>
                        </v-layout>
                    </v-container>-
                </v-flex>
            </v-layout>
        </v-container>
    </a>
</template>
<script>
    export default {
        props: { item: { type: Object, required: true } },
        computed: {
            isSpitball() { return this.item.source.includes('spitball')},
            url: function () {
                return this.isSpitball ? this.item.url.split('.co/')[1]: this.item.url
            }
        },

        methods: {
            $_spitball(event) {
                event.preventDefault();
                this.$router.push(this.url)
            }
        }
    }
</script>
<style src="./ResultItem.less" lang="less" scoped></style>
