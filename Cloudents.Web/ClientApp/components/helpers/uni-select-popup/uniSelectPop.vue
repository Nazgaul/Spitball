<template>
    <div style="width:100%;">
       <div class="exit-button">
           <v-icon @click="closePopup">sbf-close</v-icon>
       </div>
       <div class="title-text" v-language:inner>uniSelect_pop_set_school</div>
       <div class="explain-text" v-language:inner>uniSelect_pop_setting_up_explain</div>
       <div class="input-container">
           <input type="text" class="input-area" @click="openUniInterface()" v-language:placeholder placeholder="uniSelect_select_school_placeholder">
       </div>
       <div class="skip-text" @click="closePopup" v-language:inner>uniSelect_pop_later</div>
    </div>
</template>

<script>
import { mapActions } from 'vuex'
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
        ...mapActions([ 'changeSelectPopUpUniState']),
        closePopup(){
            this.$root.$emit('closePopUp', this.popUpType);
            this.changeSelectPopUpUniState(false);
        },
        openUniInterface(){
            this.$router.push({name:'addUniversity'});
            this.closePopup();
        }
    }
}
</script>

<style lang="less" src="./uniSelectPop.less"></style>

