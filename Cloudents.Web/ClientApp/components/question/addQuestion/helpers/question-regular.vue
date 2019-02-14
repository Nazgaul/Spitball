<template>
    <div class="q-regular-container">
        <div class="q-regular-center-wrapper">
            <div class="q-regular-left-container">
                <span><v-icon>sbf-offer-i</v-icon></span>
                <span class="q-regular-offer" v-language:inner>addQuestion_regular_offer_reward</span>
            </div>
            <div class="q-regular-right-container">
                <button class="q-regular-select-button"
                        :class="{'q-regular-selected': amountPicked === sblPrices.first, 'q-regular-disabled': limitOfferRange < sblPrices.first}"
                        @click="cheackStaticValue(sblPrices.first)">{{sblPrices.first}} <span v-language:inner>app_currency_dynamic</span>
                </button>
                <button class="q-regular-select-button"
                        :class="{'q-regular-selected': amountPicked === sblPrices.second, 'q-regular-disabled': limitOfferRange < sblPrices.second}"
                        @click="cheackStaticValue(sblPrices.second)">{{sblPrices.second}} <span v-language:inner>app_currency_dynamic</span>
                </button>
                <button class="q-regular-select-button"
                        :class="{'q-regular-selected': amountPicked === sblPrices.third, 'q-regular-disabled': limitOfferRange < sblPrices.third}"
                        @click="cheackStaticValue(sblPrices.third)">{{sblPrices.third}} <span v-language:inner>app_currency_dynamic</span>
                </button>
                <input type="number" :class="{'q-custom-selected': customValue !== null}" class="q-regular-custom-input"
                       :placeholder="dictionary.other" @click="checkCustomValue" @input="checkCustomInputValidity"
                       v-model="customValue">
                <!-- this invokes the computed property --> <span style="display:none;">{{selectedPrice}}</span>
                <!-- this invokes the computed property -->
            </div>
        </div>
    </div>
</template>

<script>
    import { mapGetters } from 'vuex'
    import { LanguageService } from "../../../../services/language/languageService";

    export default {
        props: {
            callback: {
                type: Function,
                required: true
            }
        },
        data() {
            return {
                isRtl: global.isRtl,
                amountPicked: null,
                customValue: null,
                sblPrices: {
                    first: 10,
                    second: 40,
                    third: 60,
                },
                dynamicCurr: LanguageService.getValueByKey("app_currency_dynamic"),
                //limit to 30%
                // limitOfferRange: parseFloat(this.accountUser().balance * 30 / 100),
                limitOfferRange: this.accountUser().balance,
                maxLimitOffer: 100,
                minLimitOffer: 1,
                errors: {
                    passedOfferLimit: LanguageService.getValueByKey('addQuestion_regular_error_offerLimit'),
                    passedMaxLimit: LanguageService.getValueByKey('addQuestion_regular_error_maxLimit'),
                    passedMinLimit: LanguageService.getValueByKey('addQuestion_regular_error_minRequired'),
                    invalidValue: LanguageService.getValueByKey('addQuestion_regular_error_invalidValue')
                },
                returnedValue: {
                    hasError: false,
                    message: '',
                    result: 0
                },
                dictionary: {
                    other: LanguageService.getValueByKey('addQuestion_regular_other')
                }
            }
        },
        methods: {
            ...mapGetters(['accountUser']),
            checkCustomValue() {
                this.amountPicked = null;
            },
            cheackStaticValue(value) {
                this.amountPicked = value;
                this.customValue = null;
            },
            checkCustomInputValidity() {
                //if user removes the number we should reset the default null value
                if (this.customValue === '') {
                    this.customValue = null;
                }
            },
            checkValidity(val) {
                this.returnedValue.message = '';
                if (val === null) {
                    this.returnedValue.hasError = true;
                    this.returnedValue.message = this.errors.invalidValue;
                    return this.returnedValue;
                } else if (Number(val) > this.limitOfferRange) {
                    this.returnedValue.hasError = true;
                    this.returnedValue.message = this.errors.passedOfferLimit;
                    return this.returnedValue;
                } else if (Number(val) > this.maxLimitOffer) {
                    this.returnedValue.hasError = true;
                    this.returnedValue.message = this.errors.passedMaxLimit;
                    return this.returnedValue;
                } else if (Number(val) < this.minLimitOffer) {
                    this.returnedValue.hasError = true;
                    this.returnedValue.message = this.errors.passedMinLimit;
                    return this.returnedValue;
                } else {
                    this.returnedValue.hasError = false;
                    return this.returnedValue;
                }
            }
        },
        computed: {
            selectedPrice() {
                //TODO callback function for valid and invalid
                let isValid = this.checkValidity(this.amountPicked || this.customValue);
                isValid.result = this.amountPicked || this.customValue;
                this.callback(isValid);
                return isValid;
            }
        }
    };
</script>

<style lang="less" src="./question-regular.less"></style>
