<template>
    <form class="setPhone" @submit.prevent="sendSms">
        <p v-language:inner="'loginRegister_setphone_title'"/>
        <v-select 
                class="widther countryCode"
                v-model="localCode"
                :label="countryCodeLabel"
                :items="countryCodesList"
                item-text="name"
                outlined
                dense
                height="44"
                :append-icon="'sbf-triangle-arrow-down'"
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

        <v-text-field
            v-model="phoneNumber"
            class="phone widther"
            prepend-inner-icon="sbf-phone"
            name=""
            type="number"
            dense
            height="44"
            :label="phoneNumberLabel"
            placeholder=" "
            outlined
        ></v-text-field>

        <!-- <sb-input 
            :errorMessage="errorMessages.phone"
            v-model="phoneNumber"
            class="phone widther"
            icon="sbf-phone" 
            :bottomError="true"
            :autofocus="true" 
            outlined
            :label="phoneNumberLabel"
            name="phone" :type="'number'"
        /> -->

        <v-btn  
            type="submit"
            :loading="smsLoading"
            large rounded
            class="white--text btn-login">
                <span v-language:inner="'loginRegister_setphone_btn'"></span>
        </v-btn>
    </form> 
</template>

<script>
import { mapActions, mapGetters, mapMutations } from 'vuex'
import SbInput from "../../question/helpers/sbInput/sbInput.vue";
import { LanguageService } from '../../../services/language/languageService';

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
        phoneNumber: function(){
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
        countryCodeLabel() {
            return LanguageService.getValueByKey('loginRegister_countryCode')
        },
        phoneNumberLabel() {
            return LanguageService.getValueByKey('loginRegister_setphone_input') 
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
    .countryCode {
        .v-list-item__title {
            color: #43425d;
            font-weight: 600;
            line-height: normal;
        }
        i {
            font-size: 8px;
            color: #43425d;
            margin-top: 10px;
        }
    }
    .phone{
        .v-input__icon--prepend-inner {
            i {
                color: #4a4a4a;
                margin-top: 10px;
            }
        }
    }
    .widther {
        .v-input__slot {
            min-height: 50px;
        }
        .v-select__selections {
            padding: 0 !important;
        }
    }
    button{
        .responsive-property(margin, 20px 0 0, null, 48px);
        .responsive-property(width, 100%, null, 72%);
        font-size: 16px;
        font-weight: 600;
        letter-spacing: -0.42px;
        text-align: center;
        text-transform: none !important;
    }

}

</style>
