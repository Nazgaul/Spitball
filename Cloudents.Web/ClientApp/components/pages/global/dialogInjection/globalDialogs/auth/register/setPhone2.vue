<template>
    <div class="setPhoneWrap text-center">
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
    </div>
</template>

<script>
import { validationRules } from '../../../../../../../services/utilities/formValidationRules2';

import registrationService from '../../../../../../../services/registrationService2.js'
import codesJson from './CountryCallingCodes';

export default {
    props: {
        errors: {
            type: Object
        }
    },
    data() {
        return {
            localCode: '',
            phoneNumber: '',
            rules: {
                phone: phone => validationRules.phone(phone)
            }
        }
    },
    watch: {
        phoneNumber() {
            if(this.errors.phone) {
                this.errors.phone = ''
            }
        }
    },
    computed: {
        countryCodesList(){
            return codesJson.sort((a, b) => a.name.localeCompare(b.name))
        },
        showError() {
            // TODO: need to retrive from server error type to know which error should show
            let phoneError = this.errors.phone
            if(phoneError) {
                if(phoneError === 'InvalidPhoneNumber') {
                    return this.$t('loginRegister_invalid_phone')
                }
                return this.$t('loginRegister_already_used_number')
            }
            return ''
        }
    },
    methods: {
        getCode(item){
            return global.isRtl ? `(${item.callingCode}) ${item.name}` : `${item.name} (${item.callingCode})`;
        },
        setLocalCode() {
            let self = this
            registrationService.getLocalCode().then(({ data }) => {
                self.localCode = data.code
            }).catch(ex => {
                self.$appInsights.trackException({exception: new Error(ex)});
            })
        }
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
