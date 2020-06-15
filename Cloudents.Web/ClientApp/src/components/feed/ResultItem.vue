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
<style lang="less" scoped>
@import "../../styles/mixin.less";



.cell-title {
    font-size: 14px;
    font-weight: bold;
    letter-spacing: -0.3px;
    .giveMeEllipsisOne();
    max-width: ~"calc(100% - 5px)";
    display: block;

    &.text-note {
        //TODO ui change
        //color: @color-dark-purple;
        color: @color-main-purple
    }

    //.giveMeEllipsis(1,1.14);
    //margin-right: 40px;
}

.right-section {
    min-width: 0; // elipsis casus issue this is the solution
    margin-left: 2px;
    margin-right: 36px;

    @media(max-width: 599px) {
        margin-right: 0;
    }

    .item-data {
        max-width: 100%;
    }
}

.snippet-wrap {
    margin-top: 4px;
    margin-bottom: 8px;

    .snippet {
        font-size: 14px;
        color: #000;
        height: 72px;
        //margin-right: 40px;
        line-height: 1.29;
        letter-spacing: -0.3px;
        overflow: hidden;
        //.giveMeEllipsis(4,1.29);
        .giveEllipsisUpdated(2, 1.29, 4);

    }
}



.bottom-wrap {
    flex-grow: 0;
    .flexSameSize();

    .bottom {
        color: #9b9b9b;
        font-size: 12px;
        letter-spacing: -0.03em;
    }
}

.imageWrapper(118px,160px,39px);

@media only screen and (max-device-width : 600px) {
    .imageWrapper(88px,120px, 32);
}

.img-wrap {
    .flexSameSize();
    box-shadow: 0 0 1px black;
}
</style>
