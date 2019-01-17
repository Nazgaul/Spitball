<template>
  <div class="q-regular-container">
    <div class="q-regular-center-wrapper">
      <div class="q-regular-left-container">
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
export default {
    data(){
        return {
            amountPicked: null,
            customValue: null,
            sblPrices: {
                first: 10,
                second: 40,
                third: 60,
            },
            limitOfferRange: 39
        }
    },
    methods:{
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
            if(val === null){
                return false;
            }else if(Number(val) > this.limitOfferRange){
                return false;
            }else{
                return true;
            }
        }
    },
    computed:{
        selectedPrice(){
            //TODO callback function for valid and invalid
            if(this.checkValidity(this.amountPicked || this.customValue)){
                console.log(this.amountPicked || this.customValue);
                return this.amountPicked || this.customValue
            }else{
                console.log("null");
                return null
            }
        }
    }

};
</script>

<style lang="less" src="./question-regular.less"></style>
