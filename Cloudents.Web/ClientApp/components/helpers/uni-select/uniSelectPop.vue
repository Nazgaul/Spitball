<template>
    <div>
       <div class="exit-button">
           <v-icon @click="closePopup">sbf-close</v-icon>
       </div>
       <div class="title-text">Set your school & classes</div>
       <div class="explain-text">By setting this up you will get the best questions & study info fit for your shool & classes</div>
       <div class="input-container">
           <input type="text" class="input-area" placeholder="Select Your School">
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
        ...mapActions([ 'changeSelectPopUpUniState']),
        closePopup(){
            console.log(this.popUpType)
            this.$root.$emit('closePopUp', this.popUpType);
            this.changeSelectPopUpUniState(false);
        }
    }
}
</script>

<style lang="less">
    //this class is injected to the popup parent
    .pop-uniselect-container{
        position: relative;
        padding-left: 32px;
        padding-right: 32px;
        width: 423px;
        height: 334px;
        opacity: 0.91;
        border-radius: 4px;
        box-shadow: 0 1px 17px 0 rgba(16, 27, 147, 0.39);
        background-image: linear-gradient(133deg, #4b8ffe, #4452fc);
        .exit-button{
            position: absolute;
            right: 0;
            margin-right: 20px;
            margin-top: 20px;
            cursor: pointer;
            i{
                font-size: 13px;
                color: #fff;
                opacity: .45;
            }
        }
        .title-text{
            margin-top: 48px;
            width: 333px;
            height: 37px;
            font-family: 'Fira Sans', sans-serif;
            font-size: 26px;
            font-weight: bold;
            font-style: normal;
            font-stretch: normal;
            line-height: normal;
            letter-spacing: normal;
            color: #ffffff;
        }
        .explain-text{
            margin-top: 53px;
            width: 360px;
            height: 50px;
            font-family: 'Fira Sans', sans-serif;
            font-size: 16px;
            font-weight: normal;
            font-style: normal;
            font-stretch: normal;
            line-height: 1.56;
            letter-spacing: normal;
            color: rgba(255, 255, 255, 0.54);
        }
        .input-container{
            margin-top: 9px;
            .input-area{
                width: 359px;
                height: 62px;
                border-radius: 4px;
                box-shadow: 0 8px 9px 0 rgba(0, 0, 0, 0.16);
                background-color: #ffffff;
                padding-left: 22px;
                padding-right: 22px;
            }
        }
        .skip-text{
            margin-top: 25px;
            width: 73px;
            height: 18px;
            font-size: 13px;
            font-weight: normal;
            font-style: normal;
            font-stretch: normal;
            line-height: normal;
            letter-spacing: -0.3px;
            color: rgba(255, 255, 255, 0.54);
            font-family: 'Open Sans';
            cursor: pointer;
        }
    }
</style>

