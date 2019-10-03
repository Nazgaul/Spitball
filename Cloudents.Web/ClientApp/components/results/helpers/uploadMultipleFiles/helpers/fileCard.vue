<template>
    <v-card class="file-item-card mb-3 elevation-0" :class="{'error-card': item.error}">
        <v-container :class="{'pr-3 pl-3 py-0': $vuetify.breakpoint.smAndUp}">
            <v-layout row class="py-3" v-bind="gridBreakpoint">
                <v-flex xs12 sm7  order-sm1 >
                    <v-text-field solo class="sb-field  bg-greyed"
                                  :class="$vuetify.breakpoint.xsOnly ? 'mr-0 mb-2' : ' mr-2 mb-2'"
                                  v-model="item.name"
                                  dir="ltr"
                                  readonly></v-text-field>
                    <div class="error-card-error-msg" v-show="item.error">
                        <span>{{item.error && item.errorText ? item.errorText : ''}}</span>
                        <!--<span v-language:inner>upload_multiple_error_upload_single_file</span>-->
                    </div>
                </v-flex>
                <v-flex xs12 sm7  order-sm3  :class="$vuetify.breakpoint.xsOnly ? 'mb-2' : ''">
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
                                hide-detail
                                :allow-overflow="false">
                        <template slot="selection" slot-scope="data" class="sb-selection">
                            <v-chip :class="{'selected': data.selected}" class="sb-chip-tag"
                                    v-if="data.item.length > 1">
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
                <v-flex xs6 sm5  order-sm2  :class="$vuetify.breakpoint.xsOnly ? 'pr-1' : ''">
                    <vue-numeric :currency="currentCurrency"
                                 :placeholder="emptyPricePlaceholder"
                                 class="numeric-input px-2"
                                 :min="1"
                                 :minus="false"
                                 :precision="2"
                                 :max="1000"
                                 :currency-symbol-position="'suffix'"
                                 separator=","
                                 v-model="item.price"></vue-numeric>
                </v-flex>

                <v-flex xs6 sm5  order-sm4 >
                    <v-text-field
                            solo
                            class="sb-field"
                            :placeholder="placeholderDocType"
                            v-model="item.type"></v-text-field>
                </v-flex>
                <v-icon v-if="quantity >1 && !item.error" class="delete-close-icon d-flex mt-3" @click="deleteFile()">
                    sbf-close
                </v-icon>
            </v-layout>
            <v-progress-linear
                    :height="'8px'"
                    :indeterminate="true"
                    v-show="progressActive"
                    :color="'#5cbbf6'"
                    class="sb-file-card-progress ma-0">
            </v-progress-linear>
        </v-container>
    </v-card>
</template>

<script>
    import { mapActions, mapGetters } from 'vuex';
    import { LanguageService } from "../../../../../services/language/languageService";

    export default {
        name: "fileCard",
        data() {
            return {
                tags: [],
                formattedPrice: '',
                fileNamePlaceholder: LanguageService.getValueByKey("upload_multiple_fileName_placeholder"),
                emptyPricePlaceholder: LanguageService.getValueByKey("upload_multiple_price_placeholder"),
                placeholderTags: LanguageService.getValueByKey("upload_multiple_keywords_optional"),
                placeholderDocType: LanguageService.getValueByKey("upload_multiple_select_filetype"),
                currentCurrency: LanguageService.getValueByKey("app_currency_dynamic"),
                price: 0,
                rules: {
                    required: value => !!value || LanguageService.getValueByKey("formErrors_required"),
                }
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
            },
            quantity: {
                type: Number,
                required: false
            }
        },

        watch: {
            item: {
                deep: true,
                handler(newVal, oldVal) {
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
            progressActive(){
                if( this.fileItem && this.fileItem.progress){
                    return this.fileItem.progress !== 100 || this.fileItem.progress === '0.00'
                }
            },
            docType() {
                return this.item.docType
            },
            gridBreakpoint() {
                const gridBreakpoint = {};
                if (this.$vuetify.breakpoint.smAndUp) {
                    gridBreakpoint.row = true;
                    gridBreakpoint.wrap = true;
                } else {
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
                            // valid tag to send to server is less than 3 spaces and without commas
                            let spaceCount = (tag.split(" ").length - 1) <= 2;
                            return tag.length > 1 && tag.indexOf(',') === -1 && spaceCount
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
    @chipActiveColor: @color-blue-new;
    .file-item-card {
        //progress
        .progress-line {
            background-color: #b3d4fc;
            display: flex;
            height: 8px;
            position: absolute;
            bottom: 0;
            left: 0;
            right: 0;
        }

        display: flex;
        width: 660px;
        max-width: 660px;
        border-radius: 4px;
        box-shadow: 0 2px 6px 0 rgba(0, 0, 0, 0.23);
        background-color: @color-white;
        &.error-card {
            -webkit-box-shadow: 0px 0px 0px 2px rgba(255, 101, 101, 0.54) !important; //vuetify
            -moz-box-shadow: 0px 0px 0px 2px rgba(255, 101, 101, 0.54) !important;
            box-shadow: 0px 0px 0px 2px rgba(255, 101, 101, 0.54) !important;

        }
        @media (max-width: @screen-xs) {
            width: 100%;
            min-width: unset;
            max-width: unset;
            &:last-child {
                margin-bottom: 112px !important; //last child offset
            }
        }
        .error-card-error-msg {
            position: absolute;
            box-shadow: 0 3px 8px 0 rgba(0, 0, 0, .19);
            background-color: #ff6565;
            top: 70px;
            left: 6px;
            z-index: 1;
            padding: 5px 18px;
            border-radius: 4px;
            font-size: 13px;
            font-weight: 600;
            letter-spacing: -.4px;
            color: #fff;
            &:after {
                content: '';
                position: absolute;
                top: 0;
                left: 50%;
                width: 0;
                height: 0;
                border: 8px solid transparent;
                border-bottom-color: #ff6565;
                border-top: 0;
                margin-left: -20px;
                margin-top: -8px;
            }

        }
        .sb-file-card-progress {
            width: 100%;
            position: absolute;
            bottom: 0;
            left: 0;
            z-index: 1;
        }
        .delete-close-icon {
            position: absolute;
            top: 36px;
            right: -28px;
            color: #ffffff;
            font-size: 10px;
            @media (max-width: @screen-xs) {
                font-size: 12px;
                position: absolute;
                top: -6px;
                right: 14px;
                color: @textColor;
            }
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
            .v-text-field__details {
                @media (max-width: @screen-xs) {
                    margin-bottom: 0;
                }
            }
            .v-input__slot {
                max-height: 48px;
                overflow-y: scroll;
                .v-text-field__details {
                    display: none;
                }
            }
            //fix for Edge bug placeholder is visible when input has val
            &.v-input--is-dirty {
                .v-input__slot {
                    input{
                        .placeholder-color(transparent, null, null, null);

                    }
                }
            }
            .v-select__selections {
                max-height: 42px;
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
            height: 48px;
            border-radius: 4px;
            border: 1px solid @colorGreyNew; 
            font-size: 14px;
            font-weight: 600;
            letter-spacing: -0.7px;
            color: @textColor;
        }
        .sb-field {
            &:not(.error--text) {
                max-height: 48px;
            }
            //fix for Edge bug placeholder is visible when input has val
            &.v-input--is-dirty {
                .v-input__slot {
                    input{
                        .placeholder-color(transparent, null, null, null);
                    }
                }
            }
            &.v-text-field--solo {
                .v-input__slot {
                    box-shadow: none !important; //vuetify
                    border-radius: 4px; //vuetify
                    border: 1px solid @colorGreyNew;                 
                    font-size: 14px;
                    letter-spacing: -0.7px;
                    color: @textColor;
                }
            }
        }
    }

</style>