<template>
    <v-card class="user-edit-wrap pb-3">
        <v-form v-model="validUserForm" ref="formUser" @submit.prevent>
        <v-layout class="header pa-3 mb-3">
            <v-flex>
                <v-icon class="edit-icon mr-2">sbf-edit-icon</v-icon>
                <span v-language:inner>profile_edit_user_profile_title</span>
            </v-flex>
        </v-layout>
    
            <v-layout class="px-3 mt-3" row wrap>
                <v-flex xs12 sm6  :class="{'pr-2': $vuetify.breakpoint.smAndUp}">
                    <v-layout column>
                        <v-flex xs12 sm6  class="pl-2 mb-2">
                            <span class="subtitle" v-language:inner>profile_personal_details</span>
                        </v-flex>
                        <v-flex xs12>
                            <v-text-field
                                    :rules="[rules.required, rules.minimumChars]"
                                    :label="firstNameLabel"
                                    class="tutor-edit-firstname"
                                    v-model.trim="firstName"
                                    outline
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
                                    outline
                            ></v-text-field>
                        </v-flex>
                    </v-layout>
                </v-flex>
            </v-layout>

        <v-layout class="px-3 prev-grow" column>
            <v-flex class="mb-2 pl-2">
                <span class="subtitle" v-language:inner>profile_aboutme</span>
            </v-flex>
            <v-flex>
                <v-textarea
                        rows="2"
                        outline
                        :rules="[rules.maximumChars, rules.descriptionMinChars]"
                        v-model="userDescription"
                        class="user-edit-description"
                        name="input-about"
                        :label="titleLabel"
                ></v-textarea>
            </v-flex>
        </v-layout>
        <v-layout  align-center class="px-3" :class="[$vuetify.breakpoint.xsOnly ? 'justify-space-between ' : 'justify-end']">
            <v-flex xs5 sm2  >
                <v-btn class="shallow-blue ml-0" round outline primary @click="closeDialog">
                    <span v-language:inner>profile_btn_cancel</span>
                </v-btn>
            </v-flex>
            <v-flex xs5 sm2  :class="{'mr-3': $vuetify.breakpoint.smAndUp}">
                <v-btn class="blue-btn  ml-0" round @click="saveChanges()" :loading="btnLoading">
                    <span v-language:inner>profile_btn_save_changes</span>
                </v-btn>
            </v-flex>
        </v-layout>
        </v-form>
    </v-card>
</template>

<script>
    import accountService from '../../../../services/accountService';
    import { mapGetters, mapActions } from 'vuex';
    import { LanguageService } from "../../../../services/language/languageService";
    import { validationRules } from "../../../../services/utilities/formValidationRules";

    export default {
        name: "userInfoEdit",
        data() {
            return {
                firstNameLabel: LanguageService.getValueByKey("profile_firstName_label"),
                lastNameLabel: LanguageService.getValueByKey("profile_lastName_label"),
                titleLabel: LanguageService.getValueByKey("profile_description_label"),
                editedDescription: '',
                editedLastName:'',
                editedFirstName:'',
                rules: {
                    required:(value)=> validationRules.required(value),
                    maximumChars:(value) => validationRules.maximumChars(value, 255),
                    minimumChars: (value) => validationRules.minimumChars(value, 2),
                    descriptionMinChars: (value) => validationRules.minimumChars(value, 15),
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
            userDescription: {
                get() {
                    return this.getProfile.user.description
                },
                set(newVal) {
                    this.editedDescription = newVal;
                }
            }
        },
        methods: {
            ...mapActions(['updateEditedProfile']),
            saveChanges() {
                if(this.$refs.formUser.validate()) {
                   let firstName = this.editedFirstName || this.firstName  ;
                   let lastName = this.editedLastName|| this.lastName
                    let editsData = {
                        name: `${firstName} ${lastName}` ,
                        description: this.editedDescription,
                        firstName,
                        lastName,
                        };
                    this.btnLoading = true;
                    accountService.saveUserInfo(editsData).then((success) => {
                        this.updateEditedProfile(editsData);
                        this.btnLoading = false;
                        this.closeDialog();
                    })
                }
            },
            closeDialog() {
                this.closeCallback ? this.closeCallback() : ''
            },
        },
        created(){
            this.editedDescription =  this.getProfile.user.description || ''
        }
    }
</script>

<style lang="less">
    @import '../../../../styles/mixin.less';

    .user-edit-wrap {
        @media(max-width: @screen-xs){
            overflow-x: hidden;
        }
        .prev-grow{
            .user-edit-name, .user-edit-description {
                .v-messages__message {
                    line-height: normal;
                }
            }
            
            @media(max-width: @screen-xs){
                flex-grow: 0;
            }
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
    }


</style>