<template>
    <v-card class="tutor-edit-wrap pb-4">
        <v-form v-model="valid" ref="formTutor">
            <v-layout class="header pa-4 mb-4">
                <v-flex>
                    <v-icon class="edit-icon mr-2">sbf-edit-icon</v-icon>
                    <span v-language:inner>profile_edit_tutor_title</span>
                </v-flex>
            </v-layout>
            <v-layout class="px-3 mt-4" wrap>
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
                                    outlined
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
                                    outlined
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
                                        outlined
                                        :prefix="accountUser.currencySymbol"
                                        class="tutor-edit-pricing"
                                        type="number"
                                        :hide-details="$vuetify.breakpoint.xsOnly"
                            ></v-text-field>
                        </v-flex>
                    </v-layout>
                </v-flex>
            </v-layout>

            <v-layout class="px-3" column :class="[$vuetify.breakpoint.xsOnly ? 'mt-4' : '']">
                <v-flex class="mb-2 pl-2">
                    <span class="subtitle" v-language:inner>profile_aboutme</span>
                </v-flex>
                <v-flex>
                    <v-textarea
                            rows="2"
                            outlined
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
                            outlined
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
                <!-- <v-flex xs5 sm2 > -->
                    <v-btn :disabled="btnLoading" width="120" depressed color="#4452fc" class="shallow-blue ml-0" rounded outlined primary @click="closeDialog">
                        <span v-language:inner>profile_btn_cancel</span>
                    </v-btn>
                <!-- </v-flex> -->
                <!-- <v-flex xs5 sm2 :class="{'mr-4': $vuetify.breakpoint.smAndUp}"> -->
                    <v-btn class="blue-btn ml-sm-4" width="120" depressed color="#4452fc" rounded @click="saveChanges()" :loading="btnLoading">
                        <span v-language:inner>profile_btn_save_changes</span>
                    </v-btn>
                <!-- </v-flex> -->
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
                    minimum: (value) => validationRules.minVal(value, this.tutorMinPrice),
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
        computed: {
            ...mapGetters(['getProfile','accountUser', 'isFrymo']),
            tutorMinPrice(){
                return this.$store.getters.getTutorMinPrice;
            },
            bio: {
                get() {
                    return this.getProfile.user.tutorData.bio
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
                    this.editedLastName = newVal;
                }
            },
            description: {
                get() {
                    return this.getProfile.user.tutorData.description;
                },
                set(newVal) {
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
                    let price = this.editedPrice || this.price;
                    let bio = this.editedBio || this.bio;
                    let description = this.editedDescription || this.description;
                    let editsData = {
                        name: `${firstName} ${lastName}`,
                        lastName,
                        firstName,
                        price,
                        bio,
                        description,
                    };
                    this.btnLoading = true;
                    let serverFormat = {
                        firstName,
                        lastName,
                        price,
                        bio,
                        description,
                    };
                    accountService.saveUserInfo(serverFormat)
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
                this.updateEditDialog(false);
            }
        },
        created() {
            this.editedBio = this.getProfile.user.tutorData.bio || '';
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
                padding: 0 16px ;
            }
        }
        .blue-btn {
            color: @color-white;
            @media (max-width: @screen-xs) {
                padding: 0 16px ;
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
        // .tutor-edit-pricing, .tutor-edit-firstname, .tutor-edit-lastname, .tutor-edit-description, .tutor-edit-bio {
        //     .v-messages__message {
        //         line-height: normal;
        //     }
        // }
    }


</style>