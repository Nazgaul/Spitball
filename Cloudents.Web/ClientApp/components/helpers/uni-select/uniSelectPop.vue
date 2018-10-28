<template>
    <div style="width:100%;">
       <div class="exit-button">
           <v-icon @click="closePopup">sbf-close</v-icon>
       </div>
       <div class="title-text">Set your school & classes</div>
       <div class="explain-text">By setting this up you will get the best questions & study info fit for your shool & classes</div>
       <div class="input-container">
           <input type="text" class="input-area" @click="openUniInterface()" placeholder="Select Your School">
       </div>
       <div class="skip-text" @click="closePopup">I’ll do it Later</div>
    </div>
</template>

<script>
import { mapGetters, mapActions } from 'vuex'
export default {
    props:{
        popUpType:{
            type:String,
            required: true
        },
        showDialog: {
            type: Boolean,
            default: false
        }
    },
    methods:{
        ...mapActions([ 'changeSelectPopUpUniState', 'changeSelectUniState']),
        closePopup(){
            //console.log(this.popUpType)
            this.$root.$emit('closePopUp', this.popUpType);
            this.changeSelectPopUpUniState(false);
        },
        openUniInterface(){
            this.changeSelectUniState(true);
            this.closePopup();
        }
    }
}
</script>

<style lang="less" src="./uniSelectPop.less"></style>

