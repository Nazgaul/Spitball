<template v-once>
    <a class="d-block pa-2" :target="$vuetify.breakpoint.xsOnly?'_self':'_blank'" :href="item.url">
        <v-container class="pa-0">
            <v-layout row>
                <avatar class="ma-1" :fullname="item.name" :image="item.image" :radius="0" :size="88"></avatar>
                <v-flex>
                    <v-container class="pa-0 full-height">
                        <v-layout wrap column justify-content-space-between align-item-stretch class="full-height ma-0">
                            <v-flex class="pa-0">
                                <v-container class="pa-0">
                                    <v-layout row>
                                        <v-flex xs8 class="title">{{item.name}}</v-flex>
                                        <v-flex xs4 v-if="item.fee"><div class="price text-xs-right">${{item.fee}} / hour</div></v-flex>
                                    </v-layout>
                                    <div class="location" v-if="item.city">{{item.city}}, {{item.state}}</div>
                                </v-container>
                            </v-flex>
                            <div class="pa-0 location" v-if="item.online">
                                Offers online lessons
                            </div>
                        </v-layout>
                    </v-container>
                </v-flex>
            </v-layout>
        </v-container>
    </a>
</template>
<script>
    import Avatar from 'vue-avatar-component'
    export default {
        components: {
            Avatar
        },
        props: { item: { type: Object, required: true } }
    }
</script>
<style src="./ResultTutor.less" lang="less" scoped>
   
</style>
<!--<style scoped>
    .container-box {
        padding: 17px;
    }

    .box-main {
        background: #fff;
        border-radius: 4px;
        -webkit-box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.2);
        -moz-box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.2);
        box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.2);
        min-height: 176px;
        padding: 8px 9px;
        position: relative;
        max-width: 504px;
    }

        .box-main .box-wrapper {
            padding-left: 127px;
        }

        .box-main .title {
            font-family: "OpenSans-Bold";
            font-size: 14px !important;
            line-height: 1.29 !important;
            letter-spacing: -0.3px !important;
            margin-bottom: 4px;
            text-overflow: ellipsis;
            overflow: hidden;
            white-space: nowrap;
        }

        .box-main p {
            font-size: 14px;
            height: 72px;
            line-height: 1.29;
            letter-spacing: -0.3px;
            overflow: hidden;
            max-width: 324px;
        }

        .box-main .img {
            border: 1px solid #320161;
            box-sizing: border-box;
            height: 160px;
            overflow: hidden;
            position: absolute;
            width: 118px;
        }

            .box-main .img .up {
                display: block;
                height: 100%;
                position: absolute;
                text-align: center;
                top: 50%;
                width: 100%;
            }

            .box-main .img img {
                display: inline-block;
                transform: translateY(-50%);
                -ms-transform: translateY(-50%);
                -webkit-transform: translateY(-50%);
                max-width: 100%;
                height: 10em;
            }

        .box-main .bottom {
            bottom: 9px;
            color: #888b8e;
            font-size: 11px;
            left: 137px;
            letter-spacing: -0.3px;
            position: absolute;
        }

    .box-user {
        min-height: 128px !important;
    }

        .box-user .box-wrapper {
            padding-left: 98px;
        }

        .box-user .title {
            color: #52ab15;
            font-family: "OpenSans-Regular";
            font-size: 17px !important;
            letter-spacing: -0.4px !important;
            margin-bottom: 3px;
        }

        .box-user .price {
            color: #ff6f00;
            font-size: 17px !important;
            letter-spacing: -0.4px !important;
            text-overflow: ellipsis;
            overflow: hidden;
            padding-right: 4px;
            white-space: nowrap;
        }

        .box-user .img {
            border: 1px solid #fff;
            height: 112px;
            width: 88px;
        }

        .box-user .sub-title {
            color: #000;
            font-family: "OpenSans-Semibold";
            font-size: 14px;
            line-height: 17px;
            letter-spacing: -0.3px;
            margin-bottom: 4px;
            text-overflow: ellipsis;
            overflow: hidden;
            white-space: nowrap;
        }

        .box-user p {
            color: #000;
            font-size: 12px;
            height: 30px;
            line-height: 1.25;
            letter-spacing: -0.3px;
            margin-bottom: 8px;
        }

        .box-user .author,
        .box-user .state {
            color: #888b8e;
            font-size: 11px;
            letter-spacing: -0.3px;
        }

        .box-user .author {
            margin-bottom: 7px;
        }
</style>-->
