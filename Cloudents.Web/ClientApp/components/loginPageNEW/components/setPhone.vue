<template>
    <form class="setPhone" @submit.prevent="sendSms">
        <p v-language:inner="'loginRegister_setphone_title'"/>
        <v-select 
                class="widther"
                v-model="localCode"
                :label="localCode"
                :items="countryCodesList"
                item-text="name"
                solo text
                :append-icon="'sbf-arrow-down'"
                item-value="callingCode">
            <template slot="selection" slot-scope="data">
                <v-list-item-content>
                    <v-list-item-title>{{getCode(data.item)}}</v-list-item-title>
                </v-list-item-content>
            </template>
            <template slot="item" slot-scope="data">
                {{getCode(data.item)}}
            </template>
        </v-select>

        <sb-input 
                :errorMessage="errorMessages.phone"
                v-model="phoneNumber"
                class="phone widther"
                icon="sbf-phone" 
                :bottomError="true"
                :autofocus="true" 
                placeholder="loginRegister_setphone_input" 
                name="phone" :type="'number'"
                v-language:placeholder/>

        <v-btn  type="submit"
                :loading="smsLoading" 
                large rounded 
                class="white--text btn-login" >
                <span v-language:inner="'loginRegister_setphone_btn'"></span>
                </v-btn>
    </form> 
</template>

<script>
import { mapActions, mapGetters, mapMutations } from 'vuex'
import SbInput from "../../question/helpers/sbInput/sbInput.vue";

export default {
    name: 'setPhone',
    components:{
        SbInput
    },
    data() {
        return {
            phoneNumber: ''
        }
    },
    watch: {
        phoneNumber: function(val){
            this.setErrorMessages({})
        }
    },
    computed: {
        ...mapGetters(['getCountryCodesList','getLocalCode','getGlobalLoading','getErrorMessages']),
        countryCodesList(){
            return this.getCountryCodesList
        },
        errorMessages(){
			return this.getErrorMessages
		},
        localCode: {
            get(){
                return this.getLocalCode
            },
            set(val){
                this.updateLocalCode(val)
            }
        },
        smsLoading(){
            return this.getGlobalLoading
        }
    },
    methods: {
        ...mapActions(['updatePhone','updateLocalCode','sendSMScode']),
        ...mapMutations(['setErrorMessages']),
        sendSms(){
            this.updatePhone(this.phoneNumber)
            this.sendSMScode()
        },
        getCode(item){
            return global.isRtl? `(${item.callingCode}) ${item.name}` : `${item.name} (${item.callingCode})`;
        }
    },
    created() {
        this.updateLocalCode()
    },
}
</script>

<style lang='less'>
@import '../../../styles/mixin.less';
@import '../../../styles/colors.less';

.setPhone{
      @media (max-width: @screen-xs) {
        display: flex;
        flex-direction: column;
        align-items: center;
      }
    p{
        .responsive-property(font-size, 28px, null, 22px);
        .responsive-property(letter-spacing, -0.51px, null, -0.4px);
        .responsive-property(margin-bottom, 56px, null, 38px);
        padding: 0;
        text-align: center;
        color: @color-login-text-title;
    }
    .v-input{
        margin-bottom: -5px;
        .v-input__control{
            .v-input__slot{
                border-radius: 4px;
                border: solid 1px rgba(55, 81, 255, 0.29);
                box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.26);
            }
        }
    }
    .phone{
        input[type=number]::-webkit-inner-spin-button, 
        input[type=number]::-webkit-outer-spin-button { 
        -webkit-appearance: none; 
        margin: 0; 
        }
        input {
        .login-inputs-style();
        padding-left: 40px !important;
            ~ i {
                position: absolute;
                top: 14px;
                left: 12px;
            }
        }
    }
    button{
        .responsive-property(margin, 66px 0 0, null, 48px);
        .responsive-property(width, 100%, null, 72%);
        font-size: 16px;
        font-weight: 600;
        letter-spacing: -0.42px;
        text-align: center;
        text-transform: none !important;
    }

}

</style>
