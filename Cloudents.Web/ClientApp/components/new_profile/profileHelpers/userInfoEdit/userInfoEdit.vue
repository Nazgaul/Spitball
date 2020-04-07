<template>
    <v-card class="user-edit-wrap pb-4">
        <v-form v-model="validUserForm" ref="formUser" @submit.prevent>
        <v-layout class="header pa-4 pt-3 mb-4">
            <v-flex>
                <v-icon class="edit-icon mr-2">sbf-edit-icon</v-icon>
                <span>{{$t('profile_edit_user_profile_title')}}</span>
            </v-flex>
        </v-layout>
        <v-layout class="px-3 mt-3" wrap>
            <v-flex xs12 sm6 :class="{'pr-2': $vuetify.breakpoint.smAndUp}">
                <v-layout column>
                    <v-flex xs12 sm6  class="pl-2 mb-2">
                        <span class="subtitle">{{$t('profile_personal_details')}}</span>
                    </v-flex>
                    <v-flex xs12>
                        <v-text-field
                                :rules="[rules.required, rules.minimumChars]"
                                :label="firstNameLabel"
                                class="tutor-edit-firstname"
                                v-model.trim="firstName"
                                outlined
                        ></v-text-field>
                    </v-flex>
                </v-layout>
            </v-flex>
            <v-flex xs12 sm6 :class="[ $vuetify.breakpoint.xsOnly ? 'mt-2 mr-0' : 'pr-2']">
                <v-layout column>
                    <v-flex v-if="$vuetify.breakpoint.smAndUp" xs12 sm6  class="mb-2 pl-2" grow>
                        <span class="subtitle" style="visibility: hidden">hidden</span>
                    </v-flex>
                    <v-flex>
                        <v-text-field
                                :rules="[rules.required, rules.minimumChars]"
                                :label="lastNameLabel"
                                class="tutor-edit-lastname"
                                v-model.trim="lastName"
                                outlined
                        ></v-text-field>
                    </v-flex>
                </v-layout>
            </v-flex>
        </v-layout>
        <v-layout  align-center class="bottomActions px-3" :class="[$vuetify.breakpoint.xsOnly ? 'justify-space-between ' : 'justify-end']">
            <v-flex xs5 sm2  >
                <v-btn class="shallow-blue ml-0" rounded outlined primary @click="closeDialog">
                    <span>{{$t('profile_btn_cancel')}}</span>
                </v-btn>
            </v-flex>
            <v-flex xs5 sm2  :class="{'mr-4': $vuetify.breakpoint.smAndUp}">
                <v-btn class="blue-btn  ml-0" rounded @click="saveChanges()" :loading="btnLoading">
                    <span>{{$t('profile_btn_save_changes')}}</span>
                </v-btn>
            </v-flex>
        </v-layout>
        </v-form>
    </v-card>
</template>

<script>
    import accountService from '../../../../services/accountService';
    import { mapGetters, mapActions } from 'vuex';
    import { validationRules } from "../../../../services/utilities/formValidationRules";

    export default {
        name: "userInfoEdit",
        data() {
            return {
                firstNameLabel: this.$t("profile_firstName_label"),
                lastNameLabel: this.$t("profile_lastName_label"),
                editedLastName:'',
                editedFirstName:'',
                rules: {
                    required:(value)=> validationRules.required(value),
                    minimumChars: (value) => validationRules.minimumChars(value, 2),
                },
                validUserForm: false,
                btnLoading: false,

            }
        },
        props: {
            closeCallback: {
                type: Function,
                required: false
            },
        },
        computed: {
            ...mapGetters(['getProfile']),

            firstName:{
              get(){
                 return this.getProfile.user.firstName
              },
              set(newVal){
                  this.editedFirstName = newVal;
              }
            },
            lastName:{
              get(){
                 return this.getProfile.user.lastName
              },
              set(newVal){
                  this.editedLastName = newVal;
              }
            },
        },
        methods: {
            ...mapActions(['updateEditedProfile','updateEditDialog']),
            saveChanges() {
                if(this.$refs.formUser.validate()) {
                   let firstName = this.editedFirstName || this.firstName  ;
                   let lastName = this.editedLastName|| this.lastName
                    let editsData = {
                        name: `${firstName} ${lastName}` ,
                        firstName,
                        lastName,
                        };
                    let serverFormat = {
                        firstName,
                        lastName
                    };
                    this.btnLoading = true;
                    accountService.saveUserInfo(serverFormat).then(() => {
                        this.updateEditedProfile(editsData);
                        this.btnLoading = false;
                        this.closeDialog();
                    })
                }
            },
            closeDialog() {
                this.updateEditDialog(false);
            },
        },
    }
</script>

<style lang="less">
    @import '../../../../styles/mixin.less';

    .user-edit-wrap {
        @media(max-width: @screen-xs){
            overflow-x: hidden;
        }
        .disabled-background{
            .v-input__slot{
                background-color: #f5f5f5!important;
            }
        }
        .shallow-blue{
            border: 1px solid  @color-blue-new;
            color:  @color-blue-new;
            @media(max-width: @screen-xs){
                min-width: 100%;
                padding: 0 16px ;
                border-radius: 0;
            }
        }
        //vuetify overwite
        .blue-btn{
            background-color:  @color-blue-new!important;
            color: @color-white;
            box-shadow: none!important;
            @media(max-width: @screen-xs){
                min-width: 100%;
                padding: 0 16px ;
                border-radius: 0;
            }
        }
        .header{
            background-color: #f0f0f7;
            width: 100%;
            max-height: 50px;
            color: @global-purple;   
            font-size: 18px;
            font-weight: bold;
            letter-spacing: -0.5px;
        }
        .subtitle{
            font-size: 16px;
            font-weight: bold;
            letter-spacing: -0.3px;
            color: @global-purple;
        }
        .edit-icon{
            color: @global-purple;
            font-size: 18px;
        }
        .v-text-field--outline > .v-input__control > .v-input__slot {
            border: 1px solid rgba(0, 0, 0, 0.19);
            &:hover {
                border: 1px solid rgba(0, 0, 0, 0.19) !important;
            }
        }
        .bottomActions {
            margin-top: 60px;
        }
    }


</style>