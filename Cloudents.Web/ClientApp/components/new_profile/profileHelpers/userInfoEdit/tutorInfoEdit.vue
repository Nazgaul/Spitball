<template>
    <v-card class="tutor-edit-wrap pb-3" >
        <v-layout class="header px-3 py-3 mb-3">
            <v-flex>
                <v-icon class="edit-icon mr-2">sbf-edit-icon</v-icon>
                <span v-language:inner>profile_edit_tutor_title</span>
            </v-flex>
        </v-layout>
        <v-layout  class="px-3 mt-2 align-stretch" align-center justify-space-between v-bind="xsColumn">
            <v-flex xs12 sm4 md4 :class="{'mr-2': $vuetify.breakpoint.smAndUp}">
                <v-layout  column >
                    <v-flex xs12 sm6 md6 class="pl-2 mb-2" >
                        <span class="subtitle" v-language:inner>profile_personal_details</span>
                    </v-flex>
                   <v-flex xs12>
                       <v-text-field
                               :label="firstNameLabel"
                               v-model="firstName"
                               outline
                               hide-details
                       ></v-text-field>
                   </v-flex>
                </v-layout>
            </v-flex>
            <v-flex  xs12 sm4 md4 :class="[ $vuetify.breakpoint.xsOnly ? 'mt-2 mr-0' : 'mr-2']">
                <v-layout  column >
                    <v-flex  v-if="$vuetify.breakpoint.smAndUp" xs12 sm6 md6  class="mb-2 pl-2" grow>
                        <span class="subtitle" style="visibility: hidden">hidden</span>
                    </v-flex>
                    <v-flex>
                        <v-text-field
                                :label="lastNameLabel"
                                v-model="lastName"
                                outline
                                hide-details
                        ></v-text-field>
                    </v-flex>
                </v-layout>
            </v-flex>
            <v-flex  xs12 sm4 md4 :class="{'mt-4': $vuetify.breakpoint.xsOnly}">
                <v-layout  column  >
                    <v-flex  xs12 sm6 md6  class="mb-2 pl-2">
                        <span class="subtitle" v-language:inner>profile_pricing</span>
                    </v-flex>
                    <v-flex>
                        <v-text-field class="disabled-background"
                                :label="priceLabel"
                                v-model="priceHour"
                                outline
                                prefix="₪"
                                readonly
                                hide-details
                        ></v-text-field>
                    </v-flex>
                </v-layout>
            </v-flex>
        </v-layout>

        <v-layout class="px-3" column :class="[$vuetify.breakpoint.xsOnly ? 'mt-5' : 'mt-3']">
            <v-flex class="mb-2 pl-2">
                <span class="subtitle" v-language:inner>profile_aboutme</span>
            </v-flex>
            <v-flex>
                <v-textarea
                        outline
                        v-model="description"
                        name="input-about"
                        :label="titleLabel"
                ></v-textarea>
            </v-flex>
        </v-layout>
        <v-layout class="px-3">
            <v-flex>
                <v-textarea
                        outline
                        v-model="bio"
                        name="input-bio"
                        :label="bioLabel"
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
    </v-card>
</template>

<script>
    import accountService from '../../../../services/accountService';
    import { mapGetters } from 'vuex';
    import { LanguageService } from "../../../../services/language/languageService";
    export default {
        name: "tutorInfoEdit",
        data() {
            return {
                firstNameLabel: LanguageService.getValueByKey("profile_firstName_label"),
                lastNameLabel: LanguageService.getValueByKey("profile_lastName_label"),
                priceLabel: LanguageService.getValueByKey("profile_price_label"),
                bioLabel: LanguageService.getValueByKey("profile_bio_label"),
                titleLabel: LanguageService.getValueByKey("profile_description_label"),
                editedAbout: '',
                editedDescription: '',
                firstName: '',
                lastName: '',
                priceHour: 50

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
            xsColumn(){
                const xsColumn = {};
                if (this.$vuetify.breakpoint.xsOnly){
                    xsColumn.column = true;
                }
                return xsColumn
            },
            bio: {
                get(){
                    return this.getProfile.about.bio
                },
                set(newVal){
                    console.log('new val::', newVal)
                    this.editedAbout = newVal;
                }
            },
            description: {
                get(){
                    return this.getProfile.user.description
                },
                set(newVal){
                    console.log('new val::', newVal)
                    this.editedDescription = newVal;
                }
            }
        },
        methods: {
            saveChanges(){
                accountService.saveTutorInfo({bio: this.bio, description: this.description}).then((success)=>{
                    this.closeDialog()
                })
            },
            closeDialog(){
                this.closeCallback ? this.closeCallback() : ''
            },
            updateAbout() {
                console.log('setting new bio', this.editedAbout)
            }
        },
    }
</script>

<style lang="less">
    @import '../../../../styles/mixin.less';
    .tutor-edit-wrap {
        .align-stretch{
            @media(max-width: @screen-xs){
                align-items: stretch;
            }
        }
        .disabled-background{
            .v-input__slot{
                background-color: #f5f5f5!important;
            }
        }
        .shallow-blue{
            border: 1px solid  #4452fc;
            color: #4452fc;
            @media(max-width: @screen-xs){
                min-width:180px;
                border-radius: 0;
            }
        }
        //vuetify overwite
        .blue-btn{
            background-color: #4452fc!important;
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