<template>
    <v-card class="file-item-card">
        <v-layout row class="px-3 py-2 pt-4">
            <v-flex xs12 sm7 md7>
                <v-text-field solo class="sb-field mr-2 bg-greyed"
                              v-model="fileItem.name"
                              :disabled="true"
                              placeholder="sdfsdfsdf"></v-text-field>
                <v-combobox class="sb-field sb-combo  mr-2"
                            v-language:placeholder
                            v-model="selectedTags"
                            height="'48px'"
                            append-icon=""
                            prepend-icon=""
                            :placeholder="placeholderTags"
                            color="'#979797'"
                            multiple
                            chips
                            solo
                            :allow-overflow="false">
                    <template slot="selection" slot-scope="data" class="sb-selection">
                        <v-chip :class="{'selected': data.selected}" class="sb-chip-tag" v-if="data.item.length > 1">
                                                   <span class="chip-button px-2">
                                                       {{!!data.item ? data.item : ''}}
                                                   </span>
                            <v-icon class="chip-close ml-3" @click="removeTag(data.item)">
                                sbf-close
                            </v-icon>
                        </v-chip>
                    </template>
                </v-combobox>
            </v-flex>
            <v-flex xs12 sm5 md5>
                <vue-numeric  currency="SBL"
                            :placeholder="emptyPricePlaceholder"
                             class="numeric-input px-2"
                             :min="1"
                             :precision="2"
                             :max="99"
                             :currency-symbol-position="'suffix'"
                             separator=","
                             v-model="fileItem.price"></vue-numeric>
                <v-select
                        class="sb-field elevation-0"
                        :items="docTypes"
                        :placeholder="placeholderDocType"
                        v-model="docType"
                        solo
                        :append-icon="'sbf-arrow-down'"></v-select>
            </v-flex>
            <v-flex xs12 sm1 md1 align-center>
                <v-icon class="delete-close-icon d-flex mt-3">sbf-close</v-icon>
            </v-flex>
        </v-layout>
        <!--<v-progress-linear-->
        <!--:height="'3px'"-->
        <!--v-show="true"-->
        <!--:color="'#4452fc'"-->
        <!--v-model="50"-->
        <!--class="sb-steps-progress ma-0"-->
        <!--:active="true">-->
        <!--</v-progress-linear>-->
    </v-card>
</template>

<script>
    import { documentTypes } from "./consts";
    import { LanguageService } from "../../../../../services/language/languageService";

    export default {
        name: "fileCard",
        data() {
            return {
                docType: '',
                tags: [],
                docTypes: documentTypes,
                formattedPrice: '',
                emptyPricePlaceholder: LanguageService.getValueByKey("upload_multiple_price_placeholder"),
                placeholderTags: LanguageService.getValueByKey("upload_multiple_keywords_optional"),
                placeholderDocType: LanguageService.getValueByKey("upload_multiple_select_filetype")
            }
        },
        props: {
            fileItem: {
                type: Object,
                default: {}
            },
            singleFileIndex:{
                type: Number
            }
        },

        computed: {
            selectedTags: {
                get() {
                    return this.tags;
                },
                set(value) {
                    let arrValidData = [];
                    if (value.length > 0) {
                        arrValidData = value.filter(tag => {
                            return tag.length > 1;
                        })
                    }
                }
            }
        },

        methods: {
            removeTag(item) {
                this.selectedTags.splice(this.selectedTags.indexOf(item), 1);
                this.selectedTags = [...this.selectedTags];
                // this.updateFile({'tags': this.selectedTags})
            },
        },
    }
</script>

<style lang="less">
    @import '../../../../../styles/mixin.less';

    @uploadGreyBackground: rgba(68, 82, 252, 0.09);
    @chipActiveColor: #4452FC;
    .file-item-card {
        display: flex;
        min-width: 666px;
        border-radius: 4px;
        box-shadow: 0 2px 6px 0 rgba(0, 0, 0, 0.23);
        background-color: @color-white;
        .delete-close-icon {
            font-size: 10px;
            cursor: pointer;
        }
        .sb-chip-tag {
            background-color: @uploadGreyBackground;
            &.selected {
                -webkit-box-shadow: 0 2px 4px 0 rgba(0, 0, 0, .2);
                box-shadow: 0 2px 4px 0 rgba(0, 0, 0, .2);
            }
            .chip-button {
                color: @chipActiveColor;
            }
        }

        .sb-combo {
            max-width: 430px;
            max-height: 48px;
            .v-input__slot {
                max-height: 48px;
                overflow-y: scroll;
            }
            .chip-button {
                color: @chipActiveColor;
                display: flex;
                text-transform: capitalize;
            }
            .chip-close {
                font-size: 8px;
            }
        }
        .numeric-input {
            display: flex;
            width: 100%;
            margin-bottom: 28px;
            height: 48px;
            border-radius: 4px;
            border: 1px solid @colorGreyNew;
            font-family: @fontOpenSans;
            font-size: 14px;
            letter-spacing: -0.7px;
            color: @textColor;

        }
        .sb-field {
            &.v-text-field--solo {
                .v-input__slot {
                    box-shadow: none !important; //vuetify
                    border-radius: 4px; //vuetify
                    border: 1px solid @colorGreyNew;
                    font-family: @fontOpenSans;
                    font-size: 14px;
                    letter-spacing: -0.7px;
                    color: @textColor;
                }
                &.bg-greyed {
                    .v-input__slot {
                        background-color: #f9f9f9;
                    }
                }
            }
        }
    }

</style>