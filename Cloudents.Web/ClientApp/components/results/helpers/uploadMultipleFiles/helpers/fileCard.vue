<template>
    <v-card class="file-item-card mb-3">
        <v-layout row class="px-3 py-2 pt-4" v-bind="gridBreakpoint">
            <v-flex xs12 sm7 md7>
                <v-text-field solo class="sb-field  bg-greyed"
                              :class="$vuetify.breakpoint.xsOnly ? 'mr-0' : ' mr-2'"
                              v-model="item.name"
                              :disabled="true"
                              placeholder="sdfsdfsdf"></v-text-field>
            </v-flex>
            <v-flex xs12 sm7 md7 :class="$vuetify.breakpoint.xsOnly ? 'mb-3' : ''">
                <v-combobox class="sb-field sb-combo"
                            :class="$vuetify.breakpoint.xsOnly ? 'mr-0' : ' mr-2'"
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
            <v-flex xs6 sm5 md5  :class="$vuetify.breakpoint.xsOnly ? 'pr-1' : ''">
                <vue-numeric currency="SBL"
                             :placeholder="emptyPricePlaceholder"
                             class="numeric-input px-2"
                             :min="1"
                             :precision="2"
                             :max="99"
                             :currency-symbol-position="'suffix'"
                             separator=","
                             v-model="item.price"></vue-numeric>
            </v-flex>
            <v-flex xs6 sm5 md5>
                <v-select
                        class="sb-field elevation-0"
                        :items="docTypes"
                        item-value="id"
                        item-text="title"
                        :placeholder="placeholderDocType"
                        v-model="item.type"
                        solo
                        :append-icon="'sbf-arrow-down'"></v-select>
            </v-flex>
            <v-flex xs12 sm1 md1 align-center  :class="$vuetify.breakpoint.xsOnly ? 'closeIcon-row-mobile' : ''">
                <v-icon class="delete-close-icon d-flex mt-3" @click="deleteFile()">sbf-close</v-icon>
            </v-flex>
        </v-layout>
        <v-progress-linear
                style="width: 100%; position: absolute; bottom:0; left:0;"
                :height="'8px'"
                :indeterminate="true"
                v-show="fileItem.progress !==100"
                :color="'#5cbbf6'"
                class="sb-steps-progress ma-0"
        >
        </v-progress-linear>
    </v-card>
</template>

<script>
    import { mapActions, mapGetters } from 'vuex';
    import { documentTypes } from "./consts";
    import { LanguageService } from "../../../../../services/language/languageService";

    export default {
        name: "fileCard",
        data() {
            return {
                tags: [],
                docTypes: documentTypes,
                formattedPrice: '',
                emptyPricePlaceholder: LanguageService.getValueByKey("upload_multiple_price_placeholder"),
                placeholderTags: LanguageService.getValueByKey("upload_multiple_keywords_optional"),
                placeholderDocType: LanguageService.getValueByKey("upload_multiple_select_filetype"),
                price: 0
            }
        },
        props: {
            fileItem: {
                type: Object,
                default: {}
            },
            singleFileIndex: {
                type: Number,
                required: true
            }
        },

        watch: {
            item: {
                deep: true,
                handler(newVal, oldVal) {
                    console.log(newVal)
                    let fileObj = {
                        index: this.singleFileIndex,
                        data: newVal
                    };
                    this.changeFileByIndex(fileObj);
                }

            }
        },
        computed: {
            ...mapGetters(['getFileData']),
            item() {
                return this.getFileData[this.singleFileIndex]
            },
            docType() {
                return this.item.docType
            },
            gridBreakpoint () {
                const gridBreakpoint = {};

                if (this.$vuetify.breakpoint.smAndUp){
                    gridBreakpoint.row = true;
                    gridBreakpoint.wrap = true;
                }else{
                    gridBreakpoint.row = true;
                    gridBreakpoint.wrap = true;
                }
                return gridBreakpoint
            },
            selectedTags: {
                get() {
                    return this.item.tags;
                },
                set(value) {
                    let arrValidData = [];
                    if (value.length > 0) {
                        arrValidData = value.filter(tag => {
                            return tag.length > 1;
                        })
                    }
                    this.item.tags = arrValidData;
                    let fileObj = {
                        index: this.singleFileIndex,
                        data: this.item
                    };
                    this.changeFileByIndex(fileObj);
                }
            }
        },

        methods: {
            ...mapActions(['changeFileByIndex', 'deleteFileByIndex']),
            deleteFile() {
                this.deleteFileByIndex(this.singleFileIndex)
            },
            removeTag(item) {
                this.selectedTags.splice(this.selectedTags.indexOf(item), 1);
                this.selectedTags = [...this.selectedTags];
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
        width: 660px;
        max-width: 660px;
        border-radius: 4px;
        box-shadow: 0 2px 6px 0 rgba(0, 0, 0, 0.23);
        background-color: @color-white;
        @media (max-width: @screen-xs) {
            width: 100%;
            min-width: 360px;
            max-width: 360px;
        }
        .closeIcon-row-mobile{
            @media(max-width: @screen-xs){
                .v-icon{
                    position: absolute;
                    top: -10px;
                    right: 8px;
                }
            }

        }
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
            max-width: 390px;
            max-height: 48px;
            .v-text-field__details{
                @media(max-width: @screen-xs){
                    margin-bottom: 0;
                }
            }
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