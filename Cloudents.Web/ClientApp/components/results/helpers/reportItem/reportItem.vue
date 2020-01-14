<template>
    <div class="report-comp-container">
        <v-layout wrap>
           
            <v-flex xs12>
                <div class="report-head">
                    <v-icon class="flag-icon">sbf-flag</v-icon>
                    <span class="text-head" v-language:inner>reportItem_report_title</span>
                    <v-icon class="report-close-icon" @click.prevent="closeReportPop()">sbf-close</v-icon>
                </div>
                <div class="reasons-wrap">
                    <div class="heading-container">
                        <span class="report-heading-text" v-language:inner>reportItem_report_subtitle</span>
                    </div>
                    <v-list>
                       <v-list-item-group>
                            <v-list-item  v-for="(reason, i) in reasons"  :key="i">
                                <v-list-item-content >
                                    <v-list-item-title @click="selectReason(reason.title)">{{
                                        reason.title }}
                                    </v-list-item-title>
                                </v-list-item-content>
                            </v-list-item>
                       
                        <v-list-item class="reason-item" @click="toogleOtherInput()">
                            <v-list-item-content class="reason-item-content">
                                <v-list-item-title class="reason-name" v-language:inner>reportItem_report_other
                                </v-list-item-title>
                            </v-list-item-content>
                        </v-list-item>
</v-list-item-group>
                    </v-list>
                    <transition name="slide-y-transition">
                        <v-text-field autofocus solo v-show="isOtherInputVisible" class="mx-2"
                                      v-model="customReason"></v-text-field>
                    </transition>

                </div>
            </v-flex>
            <v-layout justify-center align-content-center wrap class="report-footer">
                <button :disabled="isBtnDisabled" class="report-submit" @click="sendItemReport()" v-language:inner>
                    reportItem_report_btn
                </button>
            </v-layout>
          
        </v-layout>
    </div>


</template>

<script>
    import { mapActions } from 'vuex';
    import { LanguageService } from "../../../../services/language/languageService";
    
    import storeService from "../../../../services/store/storeService";
    import feedStore from '../../../../store/feedStore';

    export default {
        name: "reportItem",
        components: {},
        data() {
            return {
                reasons: [
                    {
                        title: LanguageService.getValueByKey("reportItem_reason_inappropriateContent"),
                        id: "inappropriateContent"
                    },
                    {
                        title: LanguageService.getValueByKey("reportItem_reason_inappropriateLanguage"),
                        id: "inappropriateLanguage"
                    },
                    {
                        title: LanguageService.getValueByKey("reportItem_reason_plagiarism"),
                        id: "plagiarism"
                    },
                    {
                        title: LanguageService.getValueByKey("reportItem_reason_spam"),
                        id: "spam"
                    },
                ],
                preDefinedReason: '',
                customReason: '',
                isOtherInputVisible: false,

            }

        },
        props: {
            itemId: {
                required: true,
            },
            closeReport: {
                required: true,
                type: Function
            },
            itemType: {
                type: String,
                required: true,
            },
            //only for type answer, passing additional props(question id and answer id)
            answerDelData: {
                type: Object,
                required: false
            }
        },
        computed: {
            isBtnDisabled() {
                return !this.preDefinedReason && !this.customReason
            },
        },
        methods: {
            ...mapActions(['Feeds_reportQuestion', 'reportDocument', 'Feeds_reportAnswer', 'answerRemoved']),
            callRelevantAction(type, data) {
                let actions = {
                    "Question": this.Feeds_reportQuestion,
                    "Document": this.reportDocument,
                    "answer": this.Feeds_reportAnswer
                };
                return actions[type](data);
            },
            isSelected(reason) {
                return (this.preDefinedReason === reason) && !this.isOtherInputVisible
            },
            selectReason(reason) {
                this.isOtherInputVisible = false;
                this.customReason = '';
                this.preDefinedReason = reason;
            },
            toogleOtherInput() {
                this.preDefinedReason = '';
                this.isOtherInputVisible = !this.isOtherInputVisible;
            },
            sendItemReport() {
                let reasonToSend = this.preDefinedReason !== '' ? this.preDefinedReason : this.customReason;
                let data = {
                    "id": this.itemId,
                    "flagReason": reasonToSend
                };                
                let self = this;
                this.callRelevantAction(this.itemType, data).then(() => {                    
                    //in case answer is flaged
                    if (self.itemType === "answer") {
                        //after successfull flag remove answer from client side
                        self.answerRemoved(this.answerDelData);
                    }
                    self.closeReportPop()
                    self.$router.push({name : '/feed' });
                })

            },
            closeReportPop() {
                this.closeReport();
            }
        },
        created() {
            storeService.lazyRegisterModule(this.$store, 'feeds', feedStore);
        }
    }
</script>

<style lang="less">
    @import "../../../../styles/mixin.less";

    .report-comp-container {
        background: @color-white;
        padding: 0;
        .report-head {
            display: flex;
            flex-direction: row;
            align-items: center;
            justify-content: flex-start;
            background-color: @color-blue-new;
            color: @color-white;
            height: 38px;
            font-size: 13px;
            padding: 0 16px;
            .text-head {
                display: flex;
                flex-grow: 1;
            }
            .flag-icon {
                color: @color-white;
                font-size: 16px;
                margin-right: 8px;
            }
            .report-close-icon {
                color: fade(@color-white, 80%);
                font-size: 14px;
                cursor: pointer;
                width: 20px;
                height: 20px;
            }
        }
        .reasons-wrap {
            padding: 16px 0;
            .heading-container {
                display: flex;
                flex-direction: row;
                align-items: center;
                justify-content: flex-start;
                padding: 16px 16px;
                .report-heading-text {
                    font-size: 16px;
                    letter-spacing: -0.3px;
                    color: @textColor;
                }
            }
            .reason-item {
                border-bottom: 1px solid fade(@newGreyColor, 12%);
                cursor: pointer;
                &:hover {
                    background-color: rgba(0, 0, 0, 0.04);
                }
                &.selected {
                    // background-color: @color-blue-new;
                    .v-list__tile {
                        .reason-item-content {
                            .reason-name {
                                color: @color-blue-new;
                            }
                        }
                    }
                }
                &:first-child {
                    border-top: 1px solid fade(@newGreyColor, 12%);
                }
                .v-list__tile {
                    padding-left: 0;
                    padding-right: 0;
                    .reason-item-content {
                        .reason-name {
                            font-size: 16px;
                            letter-spacing: -0.3px;
                            color: @colorBlackNew;
                            padding-left: 16px;
                        }
                    }
                }
            }
            // .input-reason {
            //     padding-top: 30px;
            //     padding-left: 16px;
            //     padding-right: 16px;
            //     font-size: 13px;
            //     color: @textColor;
            //     .v-input__slot {
            //         box-shadow: none;
            //         border: 1px solid fade(@color-black, 12%);
            //         border-radius: 4px;
            //     }
            // }
        }
        .report-submit {
            .sb-rounded-medium-btn();
            box-shadow: none;
            min-width: 180px;
            justify-content: center;
            font-size: 16px;
            font-weight: normal;
            &[disabled] {
                background-color: fade(@color-black, 12%);
                color: fade(@color-black, 12%)
            }
        }
        .report-footer {
            padding-bottom: 32px;
            justify-content: center;
        }
    }

    //content end
</style>