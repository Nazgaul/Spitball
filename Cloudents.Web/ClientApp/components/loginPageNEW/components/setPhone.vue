<template>
    <div class="setPhone">
        <p v-language:inner="'loginRegister_setphone_title'"/>
        <v-select 
                v-model="localCode"
                :label="localCode"
                :items="countryCodesList"
                item-text="name"
                solo
                flat
                :append-icon="'sbf-arrow-down'"
                item-value="callingCode">
            <template slot="selection" slot-scope="data">
                <v-list-tile-content>
                    <v-list-tile-title>{{ `${data.item.name} (${data.item.callingCode})`}}</v-list-tile-title>
                </v-list-tile-content>
            </template>
            <template slot="item" slot-scope="data">
                {{ `${data.item.name} (${data.item.callingCode})`}}
            </template>
        </v-select>

        <sb-input 
                :errorMessage="errorMessages.phone"
                v-model="phone"
                class="phone"
                icon="sbf-phone" 
                :bottomError="true" 
                placeholder="loginRegister_setphone_input" 
                name="phone" :type="'number'"
                v-language:placeholder/>

        <v-btn  @click="sendSms" 
                :loading="smsLoading" 
                :disabled="!isformValid" 
                color="#304FFE" large round 
                class="white--text" >
                <span v-language:inner="'loginRegister_setphone_btn'"></span>
                </v-btn>
    </div> 
</template>

<script>
import { mapActions, mapGetters } from 'vuex'
import SbInput from "../../question/helpers/sbInput/sbInput.vue";

export default {
    name: 'setPhone',
    components:{
        SbInput
    },
    computed: {
        ...mapGetters(['getCountryCodesList','getLocalCode','getGlobalLoading','getErrorMessages','getIsPhoneFormValid']),
        countryCodesList(){
            return this.getCountryCodesList
        },
        errorMessages(){
			return this.getErrorMessages
		},
        isformValid(){
            return this.getIsPhoneFormValid
        },
        phone: {
            get(){},
            set(val){
                this.updatePhone(val)
            }
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
        sendSms(){
            this.sendSMScode()
        }
    },
    created() {
        this.updateLocalCode()
    },
}
</script>

<style lang='less'>
@import '../../../styles/mixin.less';

.setPhone{
    p{
        padding: 0;
        margin: 0 0 56px;
        text-align: center;
        font-size: 28px;
        letter-spacing: -0.51px;
        color: #434c5f;
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
        input {
        position: relative;
        border-radius: 4px;
        box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.26);
        border: solid 1px rgba(55, 81, 255, 0.29);
        background-color: #ffffff;
        padding: 10px !important;
        padding-left: 40px !important;
            ~ i {
                position: absolute;
                top: 14px;
                left: 12px;
            }
        }
    }
    button{
        margin: 66px 0 0;
        .responsive-property(width, 100%, null, 90%);
        font-size: 16px;
        font-weight: 600;
        letter-spacing: -0.42px;
        text-align: center;
        text-transform: none !important;
    }
}

</style>
