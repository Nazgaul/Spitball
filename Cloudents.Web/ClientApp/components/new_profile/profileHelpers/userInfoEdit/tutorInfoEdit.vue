<template>
    <v-card class="tutor-edit-wrap pb-3">
        <v-form v-model="valid" ref="formTutor">
            <v-layout class="header pa-3 mb-3">
                <v-flex>
                    <v-icon class="edit-icon mr-2">sbf-edit-icon</v-icon>
                    <span v-language:inner>profile_edit_tutor_title</span>
                </v-flex>
            </v-layout>
            <v-layout class="px-3 mt-3" row wrap>
                <v-flex xs12 sm4  :class="{'pr-2': $vuetify.breakpoint.smAndUp}">
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
                <v-flex xs12 sm4 :class="[ $vuetify.breakpoint.xsOnly ? 'mt-2 mr-0' : 'pr-2']">
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
                <v-flex xs12 sm4 :class="{'mt-4': $vuetify.breakpoint.xsOnly}" v-if="!isFrymo">
                    <v-layout column>
                        <v-flex xs12 sm6  class="mb-2 pl-2">
                            <span class="subtitle" v-language:inner>profile_pricing</span>
                        </v-flex>
                        <v-flex>
                            <v-text-field 
                                        :rules="[rules.required, rules.minimum, rules.maximum,rules.integer]"
                                        :label="priceLabel"
                                        v-model="price"
                                        outline
                                        :prefix="accountUser.currencySymbol"
                                        class="tutor-edit-pricing"
                                        type="number"
                                        :hide-details="$vuetify.breakpoint.xsOnly"
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
                            :rules="[rules.maximumChars, rules.descriptionMinChars]"
                            class="tutor-edit-description"
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
                            :rules="[rules.maximumChars, rules.descriptionMinChars]"
                            v-model="bio"
                            class="tutor-edit-bio"
                            name="input-bio"
                            :label="bioLabel"
                    ></v-textarea>
                </v-flex>
            </v-layout>
            <v-layout align-center class="px-3"
                      :class="[$vuetify.breakpoint.xsOnly ? 'justify-space-between' : 'justify-end']">
                <v-flex xs5 sm2 >
                    <v-btn :disabled="btnLoading" class="shallow-blue ml-0" round outline primary @click="closeDialog">
                        <span v-language:inner>profile_btn_cancel</span>
                    </v-btn>
                </v-flex>
                <v-flex xs5 sm2 :class="{'mr-3': $vuetify.breakpoint.smAndUp}">
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
    import { mapActions, mapGetters } from 'vuex';
    import { LanguageService } from "../../../../services/language/languageService";
    import { validationRules } from '../../../../services/utilities/formValidationRules';

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
                editedPrice: null,
                rules: {
                    required: (value) => validationRules.required(value),
                    minimum: (value) => validationRules.minVal(value, 50),
                    maximum: (value) => validationRules.maxVal(value, 1000),
                    maximumChars: (value) => validationRules.maximumChars(value, 1000),
                    minimumChars: (value) => validationRules.minimumChars(value, 2),
                    descriptionMinChars: (value) => validationRules.minimumChars(value, 15),
                    integer: (value) => validationRules.integer(value)
                },
                valid: false,
                btnLoading: false

            };
        },
        props: {
            closeCallback: {
                type: Function,
                required: false
            },
        },
        computed: {
            ...mapGetters(['getProfile','accountUser', 'isFrymo']),
            bio: {
                get() {
                    return this.getProfile.about.bio;
                },
                set(newVal) {
                    this.editedBio = newVal;
                }
            },
            price: {
                get() {
                    return this.getProfile.user.tutorData.price;
                },
                set(newVal) {
                    this.editedPrice = newVal;
                }
            },
            firstName: {
                get() {
                    return this.getProfile.user.firstName;
                },
                set(newVal) {
                    this.editedFirstName = newVal;
                }
            },
            lastName: {
                get() {
                    // return this.getProfile.user.lastName
                    return this.getProfile.user.lastName;
                },
                set(newVal) {
                    console.log('new val::', newVal);
                    this.editedLastName = newVal;
                }
            },
            description: {
                get() {
                    return this.getProfile.user.description;
                },
                set(newVal) {
                    console.log('new val::', newVal);
                    this.editedDescription = newVal;
                }
            }
        },
        methods: {
            ...mapActions(['updateEditedProfile','updateEditDialog']),
            saveChanges() {
                if(this.$refs.formTutor.validate()) {
                    let firstName = this.editedFirstName || this.firstName;
                    let lastName = this.editedLastName || this.lastName;
                    let editsData = {
                        name: `${firstName} ${lastName}`,
                        lastName,
                        firstName,
                        price: this.editedPrice || this.price,
                        bio: this.editedBio,
                        description: this.editedDescription
                    };
                    this.btnLoading = true;
                    accountService.saveTutorInfo(editsData)
                        .then(() => {
                            //update profile store
                            this.updateEditedProfile(editsData);
                            this.btnLoading = false;
                            this.closeDialog();
                            this.updateEditDialog(false)
                        }, (error) => {
                            console.log('Error', error);
                            this.btnLoading = false;
                            //TODO : error callback
                        });
                }
            },
            closeDialog() {
                this.closeCallback ? this.closeCallback() : '';
            }
        },
        created() {
            this.editedBio = this.getProfile.about.bio || '';
            this.editedDescription = this.getProfile.user.description || '';
        }

    };
</script>

<style lang="less">
    @import '../../../../styles/mixin.less';

    .tutor-edit-wrap {
        @media (max-width: @screen-xs) {
            overflow-x: hidden;
        }
        .disabled-background {
            .v-input__slot {
                background-color: #f5f5f5 !important;
            }
        }
        .shallow-blue {
            border: 1px solid @global-blue;
            color: @color-blue-new;
            @media (max-width: @screen-xs) {
                min-width: 100%;
                padding: 0 16px ;
                border-radius: 0;
            }
        }
        //vuetify overwrite
        .blue-btn {
            background-color: @color-blue-new !important;
            color: @color-white;
            box-shadow: none !important;
            @media (max-width: @screen-xs) {
                min-width: 100%;
                padding: 0 16px ;
                border-radius: 0;
            }
        }
        .header {
            background-color: #f0f0f7;
            width: 100%;
            color: @global-purple;        
            font-size: 18px;
            font-weight: bold;
            letter-spacing: -0.5px;
        }
        .subtitle {
            font-size: 16px;
            font-weight: bold;
            letter-spacing: -0.3px;
            color: @global-purple;
        }
        .edit-icon {
            color: @global-purple;
            font-size: 18px;
        }
        .v-text-field--outline > .v-input__control > .v-input__slot {
            border: 1px solid rgba(0, 0, 0, 0.19);
            &:hover {
                border: 1px solid rgba(0, 0, 0, 0.19) !important;
            }
        }
        .tutor-edit-pricing, .tutor-edit-firstname, .tutor-edit-lastname, .tutor-edit-description, .tutor-edit-bio {
            .v-messages__message {
                line-height: normal;
            }
        }
    }


</style>