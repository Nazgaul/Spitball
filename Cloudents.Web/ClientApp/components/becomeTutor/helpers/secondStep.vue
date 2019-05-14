<template>
    <div class="become-second-wrap">
        <v-layout row wrap>
            <v-form v-model="validBecomeSecond" ref="becomeFormSecond" class="become-second-form">
            <v-flex xs12 class="text-xs-center mb-4">
                <span class="sharing-text" v-language:inner>becomeTutor_sharing</span>
            </v-flex>
            <v-flex xs12 :class="{'mt-3' : $vuetify.breakpoint.xsOnly}">
                <v-textarea
                        rows="2"
                        class="sb-text-area"
                        outline
                        no-resize
                        v-model="description"
                        name="input-about"
                        :rules="[rules.maximumChars]"
                        :placeholder="placeDescription"
                        :label="labelDescription"
                ></v-textarea>
            </v-flex>
            <v-flex xsw12>
                <v-textarea
                        no-resize
                        class="sb-text-area"
                        rows="5"
                        outline
                        :rules="[rules.maximumChars]"
                        v-model="bio"
                        name="input-bio"
                        :placeholder="placeBio"
                        :label="labelBio"
                ></v-textarea>
            </v-flex>
            </v-form>
        </v-layout>
        <v-layout  class="mt-2 px-1" :class="[$vuetify.breakpoint.smAndUp ? 'align-end justify-end' : 'align-center justify-center']">
            <v-btn   @click="goToPreviousStep()" class="cancel-btn elevation-0" round outline flat>
                <span v-language:inner>becomeTutor_btn_back</span>
            </v-btn>
            <v-btn
                    color="#4452FC"
                    class="white-text"
                    round
                    :loading="btnLoading"
                    :disabled="btnDisabled"
                    @click="submitData()">
                <span v-language:inner>becomeTutor_btn_done</span>
            </v-btn>
        </v-layout>
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
                rules: {
                    maximumChars:(value)=> validationRules.maximumChars(value, 1000)
                },
            };
        },
        computed: {
            ...mapGetters(['becomeTutorData']),
            btnDisabled() {
                return !this.description || !this.bio || !this.becomeTutorData.firstName || !this.becomeTutorData.lastName || !this.becomeTutorData.price
            }
        },
        methods: {
            ...mapActions(['updateTutorInfo', 'sendBecomeTutorData', 'updateTutorDialog', 'updateAccountUserToTutor']),
            // closeDialog(){
            //     this.updateTutorDialog(false);
            // },
            goToPreviousStep(){
                    this.$root.$emit('becomeTutorStep', 1);
            },
            submitData() {
                if(this.$refs.becomeFormSecond.validate()) {
                    let self = this;
                    let descriptionAbout = {bio: self.bio, description: self.description};
                    let data = {...this.becomeTutorData, ...descriptionAbout};
                    self.updateTutorInfo(data);
                    self.btnLoading = true;
                    self.sendBecomeTutorData()
                        .then((resp) => {
                            self.btnLoading = false;
                            self.$root.$emit('becomeTutorStep', 3);
                            self.updateAccountUserToTutor(true);
                        }, (error) => {
                            console.log('erorr sending data become tutor', error);
                            self.btnLoading = false;
                        }).finally(() => {
                        self.btnLoading = false;
                    });
                }

            }
        },
    };
</script>

<style lang="less">
    @import '../../../styles/mixin.less';

    .become-second-wrap {
        .become-second-form{
            width: 100%;
        }
        .sb-text-area{
            textarea{
                padding: 12px 0 8px;
            }
        }
        .sharing-text {
            font-size: 14px;
            color: @textColor;
        }
        .v-text-field--outline > .v-input__control > .v-input__slot {
            border: solid 1px rgba(0, 0, 0, 0.19);
            &:hover {
                border: 1px solid rgba(0, 0, 0, 0.19) !important;
            }
        }
    }
</style>