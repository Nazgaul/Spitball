<template>
    <a class="d-block" :target="($vuetify.breakpoint.xsOnly)?'_self':'_blank'"
       @click="(isOurs ? $_spitball($event):'')" :href="url" :class="'cell-'+$route.path.slice(1)">
        <v-container class="pa-0"
                     @click="$ga.event('Search_Results', $route.path.slice(1),`#${index+1}_${item.source}`)">
            <v-layout fluid class="result-cell-content">

                <v-flex class="img-wrap mr-2 pa-0"
                        :class="['border-'+$route.path.slice(1),'spitball-bg-'+$route.path.slice(1)]">
                    <template v-if="!item.skelaton"><img :src="item.image" v-if="item.image" alt=""
                                                         class="image-from-source">
                        <component v-else :is="$route.path.slice(1)+'-default'"
                                   :class="'spitball-bg-'+$route.path.slice(1) + 'spitball-border-'+$route.path.slice(1)"
                                   class="defaultImage"></component>
                    </template>
                </v-flex>
                <v-flex class="right-section">
                    <v-layout wrap column justify-content-space-between align-item-stretch class="full-height ma-0">
                        <v-flex class="pa-0 item-data" style="flex-grow:1">
                            <div class="cell-title-wrap">
                                <span class="cell-title" :class="'text-'+$route.path.slice(1)"
                                      v-html="item.title"></span>
                            </div>
                            <div class="snippet-wrap">
                                <span class="snippet" v-html="item.snippet"></span>
                            </div>
                        </v-flex>
                        <v-flex class="bottom-wrap pa-0">
                            <span class="bottom">{{item.source}}</span>
                        </v-flex>
                    </v-layout>
                </v-flex>
            </v-layout>
        </v-container>
    </a>
</template>
<script>
    export default {
        props: {item: {type: Object, required: true}, index: {Number}},
        computed: {
            isOurs() {
                return this.item.source.includes('Cloudents') || this.item.source.includes('Spitball')
            },
            isCloudents(){
                return this.item.source.includes('Cloudents')
            },
            isSpitball(){
                return this.item.source.includes('Spitball')
            },
            url: function () {
                if(this.isCloudents){
                    return this.item.url.split('.co/')[1]
                }else{
                    return this.item.url
                }
            },

        },
        methods: {
            $_spitball(event) {
                event.preventDefault();
                this.$router.push(this.url)
            }
        },
    }
</script>
<style src="./ResultItem.less" lang="less" scoped></style>
