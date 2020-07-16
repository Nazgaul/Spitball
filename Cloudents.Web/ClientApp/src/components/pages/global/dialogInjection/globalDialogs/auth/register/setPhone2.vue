<template>
    <v-form @submit.prevent="submit" ref="form" class="setPhoneWrap text-center pa-4 d-flex flex-column justify-space-between">
        <div class="setPhoneTop">

            <div class="closeIcon" v-if="!isStudyRoomRoute">
                <v-icon size="12" color="#aaa" @click="closeRegister">sbf-close</v-icon>
            </div>

            <template v-if="setPhoneState">
                <div class="setphone_title mb-8" v-t="'loginRegister_setphone_main_title'"></div>
                <div class="setphone_sub_title mb-9" v-t="'loginRegister_setphone_sub_title'"></div>

                <v-select 
                    v-model="localCode"
                    :items="countryCodesList"
                    class="countryCode"
                    color="#304FFE"
                    :label="$t('loginRegister_countryCode')"
                    append-icon="sbf-triangle-arrow-down"
                    height="44"
                    outlined
                    dense
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
                    type="tel"
                    color="#304FFE"
                    :rules="[rules.phone]"
                    :label="$t('loginRegister_setphone_input')"
                    :error-messages="showError"
                    prepend-inner-icon="sbf-phone"
                    height="44"
                    outlined
                    dense
                    placeholder=" "
                >
                </v-text-field>
            </template>

            <verifyPhone 
                v-else
                :phone="phoneNumber"
                :code="localCode"
                :errors="errors"
                @setConfirmCode="code => smsCode = code"
            />
        </div>

        <div class="setPhoneBottom">
            <v-btn
                type="submit"
                depressed
                height="40"
                :loading="btnLoading"
                block
                class="btns white--text mt-6"
                color="#4452fc"
            >
                <span v-t="'loginRegister_setemailpass_btn'"></span>
            </v-btn>
        </div>
    </v-form>
</template>

<script>
import { validationRules } from '../../../../../../../services/utilities/formValidationRules2';
import analyticsService from '../../../../../../../services/analytics.service.js';

const verifyPhone = () => import('./verifyPhone.vue')

import registrationService from '../../../../../../../services/registrationService2.js'
import codesJson from './CountryCallingCodes';
import authMixin from '../authMixin'

export default {
    mixins: [authMixin],
    components: {
        verifyPhone
    },
    data() {
        return {
            smsCode: '',
            setPhoneState: true,
            rules: {
                phone: phone => validationRules.phone(phone)
            }
        }
    },
    watch: {
        phoneNumber(phone) {
            this.$emit('updatePhone', phone)
            if(this.errors.phone) {
                this.errors.phone = ''
            }
        },
        code(code) {
            this.$emit('updateCode', code)
        }
    },
    computed: {
        countryCodesList(){
            return codesJson;
        },
        showError() {
            let phoneError = this.errors.phone
            if(phoneError) {
                if(phoneError === 'InvalidPhoneNumber') {
                    return this.$t('loginRegister_invalid_phone')
                }
                if (phoneError === "DuplicatePhoneNumber") {
                    return this.$t('loginRegister_already_used_number')
                }
                return phoneError;
            }
            return ''
        }
    },
    methods: {
        submit() {
            let formValidate = this.$refs.form.validate()
            if(formValidate) {
                let self = this
                if(this.setPhoneState) {
                    this.sendSms().then(function () {
                        analyticsService.sb_unitedEvent('Registration', 'Phone Submitted');
                        self.setPhoneState = false
    
                    }).catch(error => {
                        console.error(error);
                        
                        let { response: { data } } = error
                        
                        self.errors.phone = data && data["PhoneNumber"] ? data["PhoneNumber"][0] : ''
                        self.$appInsights.trackException(error);
                    })
                } else {
                    this.verifyPhone(this.smsCode)
                }
            }
        },
        getCode(item){
            return global.isRtl ? `(${item.callingCode}) ${item.name}` : `${item.name} (${item.callingCode})`;
        },
        setLocalCode() {
            let self = this
            registrationService.getLocalCode().then(({ data }) => {
                self.localCode = data.code
                this.$emit('updateCode', data.code)
            }).catch(ex => {
                self.$appInsights.trackException(ex);
            })
        },
        closeRegister() {
            this.$store.commit('setComponent', '')
        },
    },
    created() {
        this.setLocalCode()
    },
}
</script>

<style lang='less'>
@import '../../../../../../../styles/mixin.less';
@import '../../../../../../../styles/colors.less';

.setPhoneWrap {
    position: relative;
    .closeIcon {
        position: absolute;
        right: 16px;
    }
    .setphone_title {
        .responsive-property(font-size, 20px, null, 22px);
            color: @global-purple;
            font-weight: 600;
        }
        .setphone_sub_title {
            .responsive-property(font-size, 16px, null, 22px);
            line-height: 1.63;
            color: @global-purple;
        }
    .countryCode {
        .v-list-item__content {
            flex: auto; // vuetify reason ellipsis text
        }
        .v-list-item__title {
            color: @global-purple;
        }
        i {
            font-size: 8px;
            color: @global-purple;
        }
    }
    .phone{
        i {
            color: #4a4a4a;
            margin-top: 10px;
            margin-right: 10px;
        }
    }
}

</style>
