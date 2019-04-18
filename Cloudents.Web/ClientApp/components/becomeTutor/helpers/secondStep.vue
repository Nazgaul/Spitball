<template>
    <div class="become-second-wrap">
        <v-layout row wrap>
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
                        v-model="bio"
                        name="input-bio"
                        :placeholder="placeBio"
                        :label="labelBio"
                ></v-textarea>
            </v-flex>
        </v-layout>
        <v-layout  class="mt-2 px-1" :class="[$vuetify.breakpoint.smAndUp ? 'align-end justify-end' : 'align-center justify-center']">
            <v-btn   @click="closeDialog()" class="cancel-btn elevation-0" round outline flat>
                <span v-language:inner>becomeTutor_btn_cancel</span>
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
                btnLoading: false
            };
        },
        computed: {
            ...mapGetters(['becomeTutorData']),
            btnDisabled() {
                return !this.description || !this.bio;
            }
        },
        methods: {
            ...mapActions(['updateTutorInfo', 'sendBecomeTutorData', 'updateTutorDialog', 'updateAccountUserToTutor']),
            closeDialog(){
                this.updateTutorDialog(false)
            },
            submitData() {
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
        },
    };
</script>

<style lang="less">
    @import '../../../styles/mixin.less';

    .become-second-wrap {
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