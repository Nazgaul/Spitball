<template>
    <v-card class="user-edit-wrap pb-3">
        <v-form v-model="validUserForm" ref="form">
        <v-layout class="header px-3 py-3 mb-3">
            <v-flex>
                <v-icon class="edit-icon mr-2">sbf-edit-icon</v-icon>
                <span v-language:inner>profile_edit_user_profile_title</span>
            </v-flex>
        </v-layout>
        <v-layout class="px-3 mt-4 prev-grow"  row wrap>
            <v-flex xs12 sm6 md6  :class="{'pr-2' : $vuetify.breakpoint.smAndUp}">
                <v-layout column>
                    <v-flex xs12 sm6 md6 class="mb-2 pl-2">
                        <span class="subtitle" v-language:inner>profile_personal_details</span>
                    </v-flex>
                    <v-flex xs12 sm6 md6 >
                        <v-text-field
                                :rules="[rules.required]"
                                :label="userNameLabel"
                                v-model="userName"
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
                        v-model="userDescription"
                        name="input-about"
                        :label="titleLabel"
                ></v-textarea>
            </v-flex>
        </v-layout>
        <v-layout  align-center :class="[$vuetify.breakpoint.xsOnly ? 'justify-space-around px-1' : 'justify-end px-3']">
            <v-flex xs5 sm2 md2 >
                <v-btn class="shallow-blue ml-0" round outline primary @click="closeDialog">
                    <span v-language:inner>profile_btn_cancel</span>
                </v-btn>
            </v-flex>
            <v-flex xs5 sm2 md2 :class="{'mr-3': $vuetify.breakpoint.smAndUp}">
                <v-btn class="blue-btn  ml-0" round @click="saveChanges()">
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

    export default {
        name: "userInfoEdit",
        data() {
            return {
                userNameLabel: LanguageService.getValueByKey("profile_user_name_label"),
                titleLabel: LanguageService.getValueByKey("profile_description_label"),
                editedDescription: '',
                editedUserName: '',
                rules: {
                    required: value => !!value || LanguageService.getValueByKey("formErrors_required"),
                },
                validUserForm: false,

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

            userName:{
              get(){
                 return this.getProfile.user.name
              },
              set(newVal){
                  this.editedUserName = newVal;
                  console.log(this.editedUserName);
              }
            },
            userDescription: {
                get() {
                    return this.getProfile.user.description
                },
                set(newVal) {
                    console.log('new val::', newVal)
                    this.editedDescription = newVal;
                }
            }
        },
        methods: {
            ...mapActions(['updateEditedProfile']),
            saveChanges() {
                if(this.$refs.form.validate()) {
                    let editsData = {
                        name: this.editedUserName || this.userName,
                        description: this.editedDescription || this.userDescription
                    };
                    accountService.saveUserInfo(editsData).then((success) => {
                        this.updateEditedProfile(editsData);
                        this.closeDialog()
                    })
                }
            },
            closeDialog() {
                this.closeCallback ? this.closeCallback() : ''
            },
        },
    }
</script>

<style lang="less">
    @import '../../../../styles/mixin.less';

    .user-edit-wrap {
        .prev-grow{
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
                min-width:180px;
                border-radius: 0;
            }
        }
        //vuetify overwite
        .blue-btn{
            background-color:  @color-blue-new!important;
            color: @color-white;
            box-shadow: none!important;
            @media(max-width: @screen-xs){
                min-width:180px;
                border-radius: 0;
            }
        }
        .header{
            background-color: #f0f0f7;
            width: 100%;
            max-height: 50px;
            color: @profileTextColor;
            font-family: @fontOpenSans;
            font-size: 18px;
            font-weight: bold;
            letter-spacing: -0.5px;
        }
        .subtitle{
            font-size: 16px;
            font-weight: bold;
            letter-spacing: -0.3px;
            color: @profileTextColor;
        }
        .edit-icon{
            color: @profileTextColor;
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