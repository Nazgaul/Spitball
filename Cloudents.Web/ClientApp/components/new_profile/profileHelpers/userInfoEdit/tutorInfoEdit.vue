<template>
    <v-card class="tutor-edit-wrap pb-3"  >
        <v-form v-model="valid" ref="form">
        <v-layout class="header px-3 py-3 mb-3">
            <v-flex>
                <v-icon class="edit-icon mr-2">sbf-edit-icon</v-icon>
                <span v-language:inner>profile_edit_tutor_title</span>
            </v-flex>
        </v-layout>
        <v-layout class="px-3 mt-3" row wrap>
            <v-flex xs12 sm4 md4 :class="{'pr-2': $vuetify.breakpoint.smAndUp}">
                <v-layout column>
                    <v-flex xs12 sm6 md6 class="pl-2 mb-2">
                        <span class="subtitle" v-language:inner>profile_personal_details</span>
                    </v-flex>
                    <v-flex xs12>
                        <v-text-field
                                :rules="[rules.required]"
                                :label="firstNameLabel"
                                v-model="firstName"
                                outline
                        ></v-text-field>
                    </v-flex>
                </v-layout>
            </v-flex>
            <v-flex xs12 sm4 md4 :class="[ $vuetify.breakpoint.xsOnly ? 'mt-2 mr-0' : 'pr-2']">
                <v-layout column>
                    <v-flex v-if="$vuetify.breakpoint.smAndUp" xs12 sm6 md6 class="mb-2 pl-2" grow>
                        <span class="subtitle" style="visibility: hidden">hidden</span>
                    </v-flex>
                    <v-flex>
                        <v-text-field
                                :rules="[rules.required]"
                                :label="lastNameLabel"
                                v-model="lastName"
                                outline
                        ></v-text-field>
                    </v-flex>
                </v-layout>
            </v-flex>
            <v-flex xs12 sm4 md4 :class="{'mt-4': $vuetify.breakpoint.xsOnly}">
                <v-layout column>
                    <v-flex xs12 sm6 md6 class="mb-2 pl-2">
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

        <v-layout class="px-3" column :class="[$vuetify.breakpoint.xsOnly ? 'mt-3' : '']">
            <v-flex class="mb-2 pl-2">
                <span class="subtitle" v-language:inner>profile_aboutme</span>
            </v-flex>
            <v-flex>
                <v-textarea
                        rows="2"
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
                        rows="5"
                        outline
                        v-model="bio"
                        name="input-bio"
                        :label="bioLabel"
                ></v-textarea>
            </v-flex>
        </v-layout>
        <v-layout align-center :class="[$vuetify.breakpoint.xsOnly ? 'justify-space-around px-1' : 'justify-end px-3']">
            <v-flex xs5 sm2 md2>
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
        name: "tutorInfoEdit",
        data() {
            return {
                firstNameLabel: LanguageService.getValueByKey("profile_firstName_label"),
                lastNameLabel: LanguageService.getValueByKey("profile_lastName_label"),
                priceLabel: LanguageService.getValueByKey("profile_price_label"),
                bioLabel: LanguageService.getValueByKey("profile_bio_label"),
                titleLabel: LanguageService.getValueByKey("profile_description_label"),
                editedBio: '',
                editedDescription: '',
                editedFirstName: '',
                editedLastName: '',
                priceHour: 50,
                rules: {
                    required: value => !!value || LanguageService.getValueByKey("formErrors_required"),
                },
                valid: false,

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
            bio: {
                get() {
                    return this.getProfile.about.bio
                },
                set(newVal) {
                    this.editedBio = newVal;
                }
            },
            firstName: {
                get() {
                    return this.getProfile.user.firstName
                },
                set(newVal) {
                    this.editedFirstName = newVal;
                }
            },
            lastName: {
                get() {
                   return  ''
                    // return this.getProfile.user.lastName
                },
                set(newVal) {
                    console.log('new val::', newVal)
                    this. editedLastName = newVal;
                }
            },
            description: {
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
                if(this.$refs.form.validate()){
                    let editsData ={
                        firstName: this.editedFirstName || this.firstName,
                        lastName: this.editedLastName || this.lastName,
                        bio: this.editedBio || this.bio,
                        description: this.editedDescription || this.description
                    };
                    accountService.saveTutorInfo(editsData)
                        .then((success) => {
                            //update profile store
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

    .tutor-edit-wrap {
        .disabled-background {
            .v-input__slot {
                background-color: #f5f5f5 !important;
            }
        }
        .shallow-blue {
            border: 1px solid #4452fc;
            color: @color-blue-new;
            @media (max-width: @screen-xs) {
                min-width: 180px;
                border-radius: 0;
            }
        }
        //vuetify overwrite
        .blue-btn {
            background-color: @color-blue-new!important;
            color: @color-white;
            box-shadow: none !important;
            @media (max-width: @screen-xs) {
                min-width: 180px;
                border-radius: 0;
            }
        }
        .header {
            background-color: #f0f0f7;
            width: 100%;
            color: @profileTextColor;
            font-family: @fontOpenSans;
            font-size: 18px;
            font-weight: bold;
            letter-spacing: -0.5px;
        }
        .subtitle {
            font-size: 16px;
            font-weight: bold;
            letter-spacing: -0.3px;
            color: @profileTextColor;
        }
        .edit-icon {
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