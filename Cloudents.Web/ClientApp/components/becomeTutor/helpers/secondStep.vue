<template>
    <div class="become-second-wrap pl-4">
        <div>
            <v-form v-model="validBecomeSecond" ref="becomeFormSecond" class="become-second-form">
                <v-flex xs12 class="mb-4 span-cont">
                    <span class="sharing-text" v-language:inner="'becomeTutor_sharing_step_2'"></span>
                </v-flex>
                <v-flex xs12 class="mb-2" :class="{'mt-3' : $vuetify.breakpoint.xsOnly}">
                    <v-textarea
                            :height="$vuetify.breakpoint.xsOnly? 134 :100"
                            rows="2"
                            class="sb-text-area"
                            outline
                            no-resize
                            v-model="description"
                            name="input-about"
                            :rules="[rules.maximumChars, rules.descriptionMinChars]"
                            :placeholder="placeDescription"
                            :label="labelDescription"
                    ></v-textarea>
                </v-flex>
                <v-flex xsw12>
                    <v-textarea
                            :height="$vuetify.breakpoint.xsOnly? 176:156"
                            no-resize
                            class="sb-text-area"
                            rows="5"
                            outline
                            :rules="[rules.maximumChars, rules.descriptionMinChars]"
                            v-model="bio"
                            name="input-bio"
                            :placeholder="placeBio"
                            :label="labelBio"
                    ></v-textarea>
                </v-flex>
            </v-form>
        </div>
        <div class="mt-2 px-1 btns-second"
                  :class="[$vuetify.breakpoint.smAndUp ? 'align-end justify-end' : 'align-center justify-center']">
            <v-btn @click="goToPreviousStep()" class="cancel-btn elevation-0" round outline flat>
                <span v-language:inner>becomeTutor_btn_back</span>
            </v-btn>
            <v-btn
                    color="#4452FC"
                    class="white-text elevation-0"
                    round
                    :loading="btnLoading"
                    @click="submitData()">
                <span v-language:inner>becomeTutor_btn_next</span>
            </v-btn>
        </div>
    </div>
</template>

<script>
    import { mapActions, mapGetters } from 'vuex';
    import { LanguageService } from "../../../services/language/languageService";
    import { validationRules } from "../../../services/utilities/formValidationRules";

    export default {
        name: "secondStep",
        data() {
            return {
                placeDescription: LanguageService.getValueByKey("becomeTutor_placeholder_description"),
                placeBio: LanguageService.getValueByKey("becomeTutor_placeholder_bio"),
                labelDescription: LanguageService.getValueByKey("becomeTutor_label_description"),
                labelBio: LanguageService.getValueByKey("becomeTutor_label_bio"),
                description: '',
                bio: '',
                btnLoading: false,
                validBecomeSecond: false,
                errorFromServer: '',
                rules: {
                    maximumChars: (value) => validationRules.maximumChars(value, 1000),
                    descriptionMinChars: (value) => validationRules.minimumChars(value, 15),
                },
            };
        },
        computed: {
            ...mapGetters(['becomeTutorData']),
            btnDisabled() {
                return !this.description || !this.bio || !this.becomeTutorData.firstName || !this.becomeTutorData.lastName || !this.becomeTutorData.price;
            }
        },
        methods: {
            ...mapActions(['updateTutorInfo']),
            goToPreviousStep() {
                this.$root.$emit('becomeTutorStep', 1);
            },
            submitData() {
                if(this.$refs.becomeFormSecond.validate()) {
                    let descriptionAbout = {bio: this.bio, description: this.description};
                    let data = {...this.becomeTutorData, ...descriptionAbout};
                    this.updateTutorInfo(data);
                    this.$root.$emit('becomeTutorStep', 3);
                }
            }
        },
    };
</script>

<style lang="less">
    @import '../../../styles/mixin.less';
    .become-second-wrap {
        @media (max-width: @screen-xs) {
            display: flex;
            flex-direction: column;
            align-items: center;
            padding: 0 !important;
            height: 100%;
            justify-content: space-between;
        }
        
        .btns-second{
            display: flex;
            @media (max-width: @screen-xs) {
                padding: 10px;
                align-items: flex-start;
                }
            .v-btn {
                @media (max-width: @screen-xs) {
                  height: 40px;
                  padding: 0 20px;
                  text-transform: capitalize;
              }  
            }
        }
        .become-second-form {
            width: 100%;
        }
        .sb-text-area {
            textarea {
                padding: 10px 0 8px;
                line-height: 22px;
            }
                    .v-messages__message {
                        line-height: normal;
                    }
        }
        .v-input {
            .v-label{
                height: 22px;
            }
        }
        
        .v-input__slot{
            .v-text-field__slot label{
                font-size: 16px;
                color: @global-purple;
            }
            textarea::placeholder{
                    font-size: 16px;
                    letter-spacing: -0.3px;
                }
        }
        .span-cont{
            @media (max-width: @screen-xs) {
                text-align: center;
            }
            .sharing-text {
                font-size: 16px;        
                color: @global-purple;
                @media (max-width: @screen-xs) {
                    font-size: 16px;
                    font-weight: 600;
                    line-height: 1.5;
                    letter-spacing: 0.3px;
                }
            }
        }
        .v-text-field--enclosed {
            .v-text-field__details{
                @media (max-width: @screen-xs) {
                    margin: 0
                }
            }
        } 
        .v-text-field--outline > .v-input__control > .v-input__slot {
            border-radius: 6px;
            border: solid 1px rgba(67, 66, 93, 0.39);
            &:hover {
                border: 1px solid rgba(0, 0, 0, 0.19) !important;
            }
        }
    }
</style>