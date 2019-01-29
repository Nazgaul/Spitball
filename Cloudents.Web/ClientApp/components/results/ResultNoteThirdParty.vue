<template>
    <div    :href="url"
            class="d-block note-block"
            :target="'_blank'"
            @click="$_thirdPartyEvent($event, url)"
    >
        <v-container
                class="pa-0"
                @click="$ga.event('Search_Results', $route.path.slice(1),`#${index+1}_${item.source}`)"
        >
            <v-flex class="wrapper">
                <div class="document-header-container">
                    <div class="document-header-large-sagment">
                        <div class="rank-date-container">
                            <div class="rank-area  thirdParty">
                                <span class="doc-type-text">{{item.source}}</span>
                            </div>
                        </div>
                    </div>
                    <div class="document-header-small-sagment">
                        <div class="price-area"
                             v-language:inner>resultNote_free
                        </div>
                    </div>
                </div>
                <v-flex grow class="top-row">
                    <div class="type-wrap thirdPartyItem">
                        <v-flex grow class="data-row thirdParty">
                            <div class="content-wrap type-document">
                                <div class="title-wrap">
                                    <p :class="['doc-title', isFirefox ? 'foxLineClamp' : '']"
                                       v-line-clamp:20="$vuetify.breakpoint.xsOnly ? 2 : 2 "
                                    >{{item.title}}</p>
                                    <v-icon class="doc">sbf-document-note</v-icon>
                                </div>
                                <div class="content-text" v-show="item.snippet">
                                    <span v-line-clamp="2">{{item.snippet}}</span>
                                </div>
                            </div>
                        </v-flex>
                    </div>
                </v-flex>
            </v-flex>
        </v-container>
    </div>
</template>

<script>
    export default {
        data() {
            return {
                isFirefox: global.isFirefox,
                itemId: 0,
                isRtl: global.isRtl,
            };
        },
        props: {item: {type: Object, required: true}, index: {Number}},
        computed: {
            type() {
                return {
                    id: "document",
                    title: self.item.source,
                    icon: "sbf-document-note"
                };

            },
            url() {
                return this.item.url;
            },
        },
        methods: {
            $_thirdPartyEvent(event, url) {
                event.preventDefault();
                global.open(url, '_blank');
            },
        }
    };
</script>


<style scoped lang="less">
    @import "../../styles/mixin.less";

    @colorUploadDate: rgba(0, 0, 0, 0.38);
    @placeholderGrey: rgba(74, 74, 74, 0.25);
    @colorPrice: rgba(74, 74, 74, 0.87);
    .relative-pos {
        position: relative;
    }

    .note-block {
        min-height: auto;
        box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.24);
        background: @color-white;
        border-radius: 4px;
        width: 100%;
        cursor: pointer;
        .doc-type-text {
            font-size: 13px;
            font-weight: 600;
            font-family: @fontFiraSans;
        }
        i, .doc-type-text {
            color: @colorTypeDocThirdParty;

        }

        .document-header-container {
            display: flex;
            flex-direction: row;
            line-height: 35px;
            white-space: nowrap;
            .document-header-large-sagment {
                display: flex;
                width: 90%;
                flex-grow: 1;
                .rank-date-container {
                    display: flex;
                    @media (max-width: @screen-xs) {
                        flex-direction: column-reverse;
                        margin-left: 12px;
                        margin-top: -12px;
                        line-height: 2;
                    }

                }

            }
            .document-header-small-sagment {
                display: flex;
                .price-area {
                    color: @color-blue-new;
                    font-family: @fontOpenSans;
                    font-size: 16px;
                    font-weight: 600;
                    &.sold {
                        color: rgba(0, 0, 0, 0.38);
                    }
                    span {
                        font-size: 10px;
                    }
                }

            }
        }
        .wrapper {
            .top-row {
                display: flex;
                flex-direction: row;
                align-items: flex-start;
                padding-top: 12px;
                .type-wrap {
                    flex-grow: 1;
                    display: flex;
                    flex-direction: column;
                    padding-left: 12px;
                    &.thirdPartyItem {
                        padding-left: 0;
                    }
                }
            }
            .data-row {
                display: flex;
                flex-direction: row;
                align-items: center;
                height: 100%;
                margin: 20px 0 30px 0;
                &.thirdParty {
                    margin-top: 12px;
                }
                .content-wrap {
                    padding: 12px 8px;
                    background: @colorContentBackground;
                    display: flex;
                    flex-direction: column;
                    width: 100%;
                    height: auto;
                    border-radius: 4px;
                    @media (max-width: @screen-xs) {
                        align-items: stretch;
                    }
                    &.type-document {
                        background: fade(@colorTypeDocDefault, 7%);
                    }
                    .title-wrap {
                        display: flex;
                        align-items: flex-start;
                        word-break: break-all;
                        word-break: break-word;
                        i.doc {
                            color: @colorBlackNew;
                            font-size: 14px;
                            padding-top: 3px;
                            padding-right: 4px;
                        }
                        .doc-title {
                            font-size: 13px;
                            font-weight: 600;
                            color: @textColor;
                            margin-bottom: 0;
                            flex-grow: 1;
                            display: inline-flex;
                            //firefox fix for line clamp, needs specific max-height
                            &.foxLineClamp {
                                max-height: 56px !important;
                                display: block !important;
                            }
                            @media (max-width: @screen-xs) {
                                line-height: 20px;
                            }
                        }
                    }
                    .content-text {
                        padding-top: 9px;
                        span {
                            color: @colorBlackNew;
                            font-size: 13px;
                            line-height: 1.5;
                            letter-spacing: -0.2px;
                            word-break: break-all;
                            word-break: break-word;
                        }
                    }
                }
            }

        }
    }
</style>