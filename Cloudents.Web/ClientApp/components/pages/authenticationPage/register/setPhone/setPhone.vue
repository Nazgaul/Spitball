<template>
    <v-form class="setPhone text-center" @submit.prevent="sendSms" ref="form" lazy-validation>
        <div class="setPhoneWrap">
            <p class="setphone_title" v-language:inner="'loginRegister_setphone_title'"></p>
            <v-select 
                v-model="localCode"
                class="widther countryCode"
                color="#304FFE"
                outlined
                height="44"
                dense
                :label="countryCodeLabel"
                :items="countryCodesList"
                :append-icon="'sbf-triangle-arrow-down'"
                item-text="name"
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
                class="phone"
                color="#304FFE"
                outlined
                height="44"
                dense
                type="tel"
                prepend-inner-icon="sbf-phone"
                name=""
                :rules="[rules.phone]"
                :label="phoneNumberLabel"
                :error-messages="errorMessages.phone"
                placeholder=" "
            ></v-text-field>
        </div>
        <v-btn
            type="submit"
            :loading="smsLoading"
            large rounded
            class="white--text btn-login">
                <span v-language:inner="'loginRegister_setphone_btn'"></span>
        </v-btn>
    </v-form> 
</template>

<script>
import { mapActions, mapGetters, mapMutations } from 'vuex'
import { LanguageService } from '../../../../../services/language/languageService';
import { validationRules } from '../../../../../services/utilities/formValidationRules';

export default {
    data() {
        return {
            phoneNumber: '',
            rules: {
                phone: phone => validationRules.phone(phone)
            }
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
            let validate = this.$refs.form.validate()
            if(validate) {
                this.updatePhone(this.phoneNumber)
                this.sendSMScode()
            }
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
@import '../../../../../styles/mixin.less';
@import '../../../../../styles/colors.less';
.setPhone{

    // height: inherit;
    @media (max-width: @screen-xs) {
        display: flex;
        flex-direction: column;
        align-items: center;
        justify-content: space-between;
        .phoneGapFooterFix();
    }
    .setPhoneWrap {
        .setphone_title {
            .responsive-property(font-size, 28px, null, 22px);
            .responsive-property(letter-spacing, -0.51px, null, -0.4px);
            .responsive-property(margin-bottom, 56px, null, 38px);
            .responsive-property(margin-top, null, null, 42px);
            padding: 0;
            text-align: center;
            color: @color-login-text-title;
        }
        .countryCode {
            flex-grow: 0;
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
            flex-grow: 0;
            width: 100%;
            .v-input__icon--prepend-inner {
                i {
                    color: #4a4a4a;
                    margin-top: 10px;
                    margin-right: 10px;
                }
            }
        }
        .widther {
            width: 100%;
            .v-select__selections {
                padding: 0 !important;
            }
        }
    }
    .btn-login{
        .responsive-property(margin, 20px 0 0, null, null);
        .responsive-property(width, 100%, null, @btnDialog);
        font-size: 16px;
        font-weight: 600;
        letter-spacing: -0.42px;
        text-align: center;
        text-transform: none !important;
    }
}

</style>
