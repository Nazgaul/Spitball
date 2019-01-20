<template>
  <div class="q-regular-container">
    <div class="q-regular-center-wrapper">
      <div class="q-regular-left-container">
        <span><v-icon>sbf-offer-i</v-icon></span>
        <span class="q-regular-offer">Offer a Reward</span>
      </div>
      <div class="q-regular-right-container">
          <button class="q-regular-select-button" :class="{'q-regular-selected': amountPicked === sblPrices.first, 'q-regular-disabled': limitOfferRange < sblPrices.first}" @click="cheackStaticValue(sblPrices.first)">{{sblPrices.first}} SBL</button>
          <button class="q-regular-select-button" :class="{'q-regular-selected': amountPicked === sblPrices.second, 'q-regular-disabled': limitOfferRange < sblPrices.second}" @click="cheackStaticValue(sblPrices.second)">{{sblPrices.second}} SBL</button>
          <button class="q-regular-select-button" :class="{'q-regular-selected': amountPicked === sblPrices.third, 'q-regular-disabled': limitOfferRange < sblPrices.third}" @click="cheackStaticValue(sblPrices.third)">{{sblPrices.third}} SBL</button>
          <input type="number" :class="{'q-custom-selected': customValue !== null}" class="q-regular-custom-input" placeholder="Other" @click="checkCustomValue" @input="checkCustomInputValidity" v-model="customValue">
          <!-- this invokes the computed property --> <span style="display:none;">{{selectedPrice}}</span> <!-- this invokes the computed property -->
      </div>
    </div>
  </div>
</template>

<script>
import { mapGetters } from 'vuex'
export default {
    props:{
        callback:{
            type: Function,
            required: true
        }
    },
    data(){
        return {
            amountPicked: null,
            customValue: null,
            sblPrices: {
                first: 10,
                second: 40,
                third: 60,
            },
            limitOfferRange: parseFloat(this.accountUser().balance * 30 / 100),
            maxLimitOffer: 100,
            errors: {
                passedOfferLimit: 'offer limit reached',
                passedMaxLimit: 'max limit reached',
                invalidValue: 'invalid value'
            },
            returnedValue: {
                hasError: false,
                message: '',
                result: 0
            }
        }
    },
    methods:{
        ...mapGetters(['accountUser']),
        checkCustomValue(){
            this.amountPicked = null;
        },
        cheackStaticValue(value){
            this.amountPicked = value;
            this.customValue = null;
        },
        checkCustomInputValidity(){
            //if user removes the number we should reset the default null value
            if(this.customValue === ''){
                this.customValue = null;
            }
        },
        checkValidity(val){
            this.returnedValue.message = '';
            if(val === null){
                this.returnedValue.hasError = true;
                this.returnedValue.message = this.errors.invalidValue;
                return this.returnedValue;
            }else if(Number(val) > this.limitOfferRange){
                this.returnedValue.hasError = true;
                this.returnedValue.message = this.errors.passedOfferLimit;
                return this.returnedValue;
            }else if(Number(val) > this.maxLimitOffer){
                this.returnedValue.hasError = true;
                this.returnedValue.message = this.errors.passedMaxLimit;
                return this.returnedValue;
            }else{
                this.returnedValue.hasError = false;
                return this.returnedValue;
            }
        }
    },
    computed:{
        selectedPrice(){
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
