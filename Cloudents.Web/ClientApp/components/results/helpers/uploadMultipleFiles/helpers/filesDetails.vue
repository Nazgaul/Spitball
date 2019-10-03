<template>
    <v-card elevation="0" class="sb-step-card files-details-container">
        <v-flex xs12 sm12  class="error-block align-center justify-center d-flex" v-if="showError">
            <div class="error-container" style="height: 90px;">
                <span class="error-title">{{errorText}}</span>
            </div>

        </v-flex>
        <v-layout v-bind="gridBreakpoint" row wrap v-show="!showError"
                  :class="['top-block', $vuetify.breakpoint.smAndUp ? 'px-4' : 'px-2', 'py-3', {'justify-center' : isMultiple }] ">

            <v-flex xs12 sm12  class="mb-2" grow>
                <span class="selected-class-label" v-language:inner>upload_multiple_selected_class_label</span>
                <span>:</span>
                <span class="selected-class-val ml-2">{{selectedCourse}}</span>
            </v-flex>
            <v-layout wrap>
                <v-flex xs12 sm3 >
                    <v-text-field solo
                                  :placeholder="profPlaceholder"
                                  @change="updateProfessorName()"
                                  v-model="professor"
                                  :class="[$vuetify.breakpoint.smAndUp ? 'mr-2': ' mt-3']"
                                  class="professor-input sb-field max-heigth-limit"></v-text-field>
                </v-flex>
                <v-flex xs12  sm9 class="multiple-controls" style="display: flex; flex-grow: 0;" v-if="isMultiple">
                    <v-flex xs12 sm6 >
                        <div :class="['all-wrap',  $vuetify.breakpoint.xsOnly ? 'mr-0 mt-3' : 'mr-1' ]">
                            <vue-numeric :currency="currentCurrency"
                                         class="price-for-all"
                                         :minus="false"
                                         :min="0"
                                         :precision="2"
                                         :placeholder="emptySetToAll"
                                         :max="1000"
                                         :currency-symbol-position="'suffix'"
                                         separator=","
                                         v-model="priceForAll"></vue-numeric>
                            <v-btn class="all-btn elevation-0"
                                   :disabled="!priceForAll" @click="updatePrice()">
                                <span v-language:inner>upload_multiple_bnt_apply</span>
                            </v-btn>
                        </div>
                    </v-flex>
                    <v-flex xs12 sm6 >
                        <div :class="['all-wrap', $vuetify.breakpoint.smAndUp ? 'ml-1' : 'mt-2']">
                            <v-text-field
                                    solo
                                    class="sb-field doc-type-select"
                                    :placeholder="placeholderTypeToAll"
                                    v-model="docType"></v-text-field>
                            <v-btn class="all-btn elevation-0"
                                   :disabled="!docType" @click="updateDocsType()">
                                <span v-language:inner>upload_multiple_bnt_apply</span>
                            </v-btn>
                        </div>
                    </v-flex>
                </v-flex>
            </v-layout>
        </v-layout>
        <v-layout justify-start align-center column class="bottom-block py-3"
                  :class="{'px-2': $vuetify.breakpoint.xsOnly }">
            <v-flex xs12 sm6  row class="justify-center align-center upload-options">
                <transition-group name="slide-x-transition">
                    <file-card
                            v-for="(fileItem, index) in fileItems"
                            :fileItem="fileItem"
                            :quantity="fileItems.length"
                            :singleFileIndex="index" :key="index"></file-card>
                </transition-group>
            </v-flex>
        </v-layout>
    </v-card>
</template>
<script>
    import { LanguageService } from "../../../../../services/language/languageService";
    import fileCard from './fileCard.vue';
    import { mapGetters, mapActions } from 'vuex'

    export default {
        name: "filesDetails",
        components: {fileCard},
        data() {
            return {
                profPlaceholder: LanguageService.getValueByKey("upload_multiple_professor_placeholder"),
                emptySetToAll: LanguageService.getValueByKey("upload_multiple_placeholder_setAll"),
                placeholderTypeToAll: LanguageService.getValueByKey("upload_multiple_placeholder_doctype_all"),
                currentCurrency: LanguageService.getValueByKey("app_currency_dynamic"),
                someVal: '',
                docType: '',
                professor: '',
                priceForAll: '',
                fileItems: this.getFileData()
            }
        },
        props: {
            propName: '',
            showError: {
                type: Boolean,
                default: false
            },
            errorText: 'testin error'
        },
        computed: {
            isMultiple() {
                let files = this.getFileData();
                return files && files.length > 1;
            },
            selectedCourse() {
                if (this.fileItems && this.fileItems[0]) {
                    return this.fileItems[0].course
                }
            },
            gridBreakpoint() {
                const gridBreakpoint = {};
                if (this.$vuetify.breakpoint.smAndUp) {
                    gridBreakpoint.row = true
                } else {
                    gridBreakpoint.column = true
                }
                return gridBreakpoint
            }

        },
        methods: {
            ...mapGetters(['getFileData']),
            ...mapActions(['setAllPrice', 'setAllDocType', 'setAllProfessor']),
            updatePrice() {
                this.setAllPrice(this.priceForAll)
            },
            updateDocsType() {
                this.setAllDocType(this.docType)
            },
            updateProfessorName() {
                this.setAllProfessor(this.professor)
            }
        },
    }
</script>

<style lang="less">
    @import "../../../../../styles/mixin.less";

    @purpleOnBoard: #5158af;
    .files-details-container {
        .selected-class-label {
            font-size: 16px;
            font-weight: normal;
            letter-spacing: -0.8px;
            color: @textColor;
        }
        .selected-class-val {
            font-size: 16px;
            font-weight: bold;
            letter-spacing: -0.8px;
            color: @textColor;
        }
        .max-heigth-limit {
            max-height: 40px;
        }
        .all-wrap {
            display: flex;
            flex-direction: row;
            border-radius: 4px;
            border: 1px solid @colorGreyNew;
            height: 48px;
            /*max-width: 301px;*/
            @media (max-width: @screen-xs) {
                max-width: unset;
            }
            .sb-field {
                //fix for Edge bug placeholder is visible when input has val
                &.v-input--is-dirty {
                    .v-input__slot {
                        input{
                            .placeholder-color(transparent, null, null, null);

                        }
                    }
                }
                .v-input__slot {
                    border-right: none;
                    border-radius: 0 4px 0 4px;
                }
            }
        }
        .multiple-controls {
            flex-grow: 1;
            @media (max-width: @screen-xs) {
                flex-direction: column;
            }
        }

        .all-btn {
            &.v-btn {
                height: 46px;
                margin: 0;
                box-shadow: none;
                border-radius: 0 4px 0 0;
                color: @color-white;
                background-color: @color-blue-new !important;
                &[disabled] {
                    background-color: #b8b8b8 !important;
                }
            }
        }
        .price-for-all {
            display: flex;
            width: 100%;
            height: 48px;
            border-right: none;
            font-size: 14px;
            font-weight: 600;
            letter-spacing: -0.7px;
            color: @textColor;
            outline: none;
            padding: 0 12px;
            .placeholder-color(@textColor, 0.7, 100, 14px);

            @media (max-width: @screen-xs) {
                min-width: unset;
            }
        }
        .v-select__slot {
            label {
                color: fade(@textColor, 70%);
                font-weight: 100;
                font-size: 14px;
            }
        }
        .sb-field {
            &.professor-input {
                /*max-width: 220px;*/
                @media (max-width: @screen-xs) {
                    max-width: 100%;
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
                    input {
                        .placeholder-color(@textColor, 0.7, 100, 14px);
                    }

                }
            }
            &.doc-type-select {
                @media (max-width: @screen-xs) {
                    max-width: 100%;
                }

                .v-input__slot {
                    box-shadow: none !important; //vuetify
                    border-radius: 0 4px 0 4px; //vuetify     
                    background: transparent;
                    border: none;
                    font-size: 14px;
                    letter-spacing: -0.7px;
                    color: @textColor;

                }
            }
        }
        .top-block {
            border-radius: 0 0 4px 4px;
            background-color: @color-white;
            box-shadow: 0 3px 8px 0 rgba(0, 0, 0, 0.26);
        }
        .bottom-block {
            max-height: 540px;
            overflow-y: scroll;
            box-shadow: 0 2px 4px 0 rgba(0, 0, 0, 0.07);
            background-color: @purpleOnBoard;
            @media (max-width: @screen-xs) {
                overflow-y: unset;
                max-height: unset;
            }
        }

    }


</style>